using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RoomTaskManagement.API.BusinessLogic;
using RoomTaskManagement.API.Data;
using RoomTaskManagement.API.Repositories.Implementations;
using RoomTaskManagement.API.Repositories.Interfaces;
using RoomTaskManagement.API.Services.Implementations;
using RoomTaskManagement.API.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Database
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskAssignmentRepository, TaskAssignmentRepository>();
builder.Services.AddScoped<ITaskHistoryRepository, TaskHistoryRepository>();

//Buisness Logic
builder.Services.AddScoped<TaskTurnCalculator>();

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
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAngular", policy =>
	{
		policy.WithOrigins("http://localhost:4200")
			   .AllowAnyHeader()
			   .AllowAnyMethod()
			   .AllowCredentials();
	});
});

//SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
