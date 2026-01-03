using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RoomTaskManagement.API.BusinessLogic;
using RoomTaskManagement.API.Data;
using RoomTaskManagement.API.Helpers;
using RoomTaskManagement.API.Hubs;
using RoomTaskManagement.API.Models;
using RoomTaskManagement.API.Repositories.Implementations;
using RoomTaskManagement.API.Repositories.Interfaces;
using RoomTaskManagement.API.Services.Implementations;
using RoomTaskManagement.API.Services.Interfaces;
using System.Text;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "Room Task Management API",
		Version = "v1"
	});

	// JWT Bearer configuration
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "Enter: Bearer {your JWT token}"
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			new string[] {}
		}
	});
});

// Configure WhatsApp Settings
builder.Services.Configure<WhatsAppSettings>(builder.Configuration.GetSection("WhatsAppSettings"));

// Add HttpClientFactory
builder.Services.AddHttpClient();

//Database
//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//Database
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//Database
//Database
//Database
var connectionString = builder.Environment.IsProduction()
	? Environment.GetEnvironmentVariable("DATABASE_URL")
	: builder.Configuration.GetConnectionString("DefaultConnection");

// Log for debugging
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"Connection String Length: {connectionString?.Length ?? 0}");
Console.WriteLine($"DATABASE_URL env var: {Environment.GetEnvironmentVariable("DATABASE_URL")?.Substring(0, 20) ?? "NULL"}...");

if (string.IsNullOrEmpty(connectionString))
{
	throw new InvalidOperationException("❌ DATABASE_URL environment variable is not set!");
}

if (builder.Environment.IsProduction())
{
	builder.Services.AddDbContext<ApplicationDbContext>(options =>
		options.UseNpgsql(connectionString));
}
else
{
	builder.Services.AddDbContext<ApplicationDbContext>(options =>
		options.UseSqlServer(connectionString));
}

//Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskAssignmentRepository, TaskAssignmentRepository>();
builder.Services.AddScoped<ITaskHistoryRepository, TaskHistoryRepository>();

//Buisness Logic
builder.Services.AddScoped<TaskTurnCalculator>();
builder.Services.AddScoped<JwtHelper>();
builder.Services.AddScoped<TaskAssignmentRepository>();

//Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IWhatsAppService, WhatsAppService>();

//JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = jwtSettings["Issuer"],
		ValidAudience = jwtSettings["Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(secretKey)
	};
});

//CORS
//builder.Services.AddCors(options =>
//{
//	options.AddPolicy("AllowAngular", policy =>
//	{
//		policy.WithOrigins("http://localhost:4200")
//			   .AllowAnyHeader()
//			   .AllowAnyMethod()
//			   .AllowCredentials();
//	});
//});

//CORS
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAngular", policy =>
	{
		policy.AllowAnyOrigin()
			  .AllowAnyHeader()
			  .AllowAnyMethod();
	});
});


//SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Run migrations and seed data in production
if (app.Environment.IsProduction())
{
	using (var scope = app.Services.CreateScope())
	{
		var services = scope.ServiceProvider;
		try
		{
			var context = services.GetRequiredService<ApplicationDbContext>();

			// Apply pending migrations
			Console.WriteLine("🔄 Applying database migrations...");
			context.Database.Migrate();
			Console.WriteLine("✅ Migrations applied successfully");

			// Seed SuperAdmin if doesn't exist
			if (!context.Users.Any(u => u.Username == "superadmin"))
			{
				Console.WriteLine("🔄 Creating SuperAdmin user...");
				var superAdmin = new RoomTaskManagement.API.Models.Entities.User
				{
					Username = "superadmin",
					PasswordHash = "$2a$11$O9tFsJI8e/6o.rKD7T5Ts.DAnqLBPiXmqNINHDfEnCacey394O1xu",
					FullName = "Super Administrator",
					PhoneNumber = "9999999999",
					Role = "SuperAdmin",
					IsOutOfStation = false,
					CreatedAt = new DateTime(2024, 01, 01),
					UpdatedAt = new DateTime(2024, 01, 01)
				};

				context.Users.Add(superAdmin);
				context.SaveChanges();
				Console.WriteLine("✅ SuperAdmin created: username=superadmin, password=Admin@2001");
			}
			else
			{
				Console.WriteLine("✅ SuperAdmin already exists");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"❌ Database setup failed: {ex.Message}");
			Console.WriteLine($"Stack trace: {ex.StackTrace}");
		}
	}
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();
// Only use HTTPS redirect in development
if (app.Environment.IsDevelopment())
{
	app.UseHttpsRedirection();
}

app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Map SignalR Hub
//app.MapHub<TaskHub>("/taskHub");
// Map SignalR Hub
app.MapHub<TaskHub>("/hubs/task");


app.Run();
