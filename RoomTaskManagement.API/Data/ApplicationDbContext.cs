using Microsoft.EntityFrameworkCore;
using RoomTaskManagement.API.Models.Entities;

namespace RoomTaskManagement.API.Data
{
	/// <summary>Database context for the application.</summary>
	public class ApplicationDbContext : DbContext
	{
		/// <summary>Initializes a new instance of the <see cref="ApplicationDbContext"/> class.</summary>
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}

		/// <summary>Gets or sets the Tasks DbSet.</summary>
		public DbSet<User> Users { get; set; }

		/// <summary>Gets or sets the Tasks DbSet.</summary>
		public DbSet<TaskEntity> Tasks { get; set; }

		/// <summary>Gets or sets the TaskAssignments DbSet.</summary>
		public DbSet<TaskAssignment> TaskAssignments { get; set; }

		public DbSet<TaskHistory> TaskHistories { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// User configuration
			modelBuilder.Entity<User>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.HasIndex(e => e.Username).IsUnique();
				entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
				entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
				entity.Property(e => e.PhoneNumber).HasMaxLength(15).IsRequired();
				entity.Property(e => e.Role).HasMaxLength(20).IsRequired();
			});

			// TaskEntity configuration
			modelBuilder.Entity<TaskEntity>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.TaskName).HasMaxLength(100).IsRequired();
				entity.Property(e => e.Description).HasMaxLength(500);
				entity.HasOne(e => e.Creator)
					.WithMany()
					.HasForeignKey(e => e.CreatedBy)
					.OnDelete(DeleteBehavior.Restrict);
			});

			// TaskAssignment configuration
			modelBuilder.Entity<TaskAssignment>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Status).HasMaxLength(20);

				entity.HasOne(e => e.Task)
					.WithMany(t => t.TaskAssignments)
					.HasForeignKey(e => e.TaskId)
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasOne(e => e.User)
					.WithMany(u => u.TaskAssignments)
					.HasForeignKey(e => e.UserId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.Triggerer)
					.WithMany()
					.HasForeignKey(e => e.TriggeredBy)
					.OnDelete(DeleteBehavior.Restrict);
			});

			// TaskHistory configuration
			modelBuilder.Entity<TaskHistory>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Action).HasMaxLength(50).IsRequired();

				entity.HasOne(e => e.Task)
					.WithMany(t => t.TaskHistories)
					.HasForeignKey(e => e.TaskId)
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasOne(e => e.User)
					.WithMany(u => u.TaskHistories)
					.HasForeignKey(e => e.UserId)
					.OnDelete(DeleteBehavior.Restrict);
			});

			// Seed SuperAdmin user
			modelBuilder.Entity<User>().HasData(
				new User
				{
					Id = 1,
					Username = "superadmin",
					PasswordHash = "$2a$11$O9tFsJI8e/6o.rKD7T5Ts.DAnqLBPiXmqNINHDfEnCacey394O1xu",
					FullName = "Super Administrator",
					PhoneNumber = "9999999999",
					Role = "SuperAdmin",
					IsOutOfStation = false,
					CreatedAt = new DateTime(2024, 01, 01),
					UpdatedAt = new DateTime(2024, 01, 01)
				}
			);
		}

	}
}
