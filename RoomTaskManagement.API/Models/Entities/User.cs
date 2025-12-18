namespace RoomTaskManagement.API.Models.Entities
{
	/// <summary>
	/// Represents a user in the Room Task Management system.
	/// </summary>
	public class User
	{
		#region Properties
		/// <summary>Gets or sets the unique identifier for the user.</summary>
		public int Id { get; set; }

		/// <summary>Gets or sets the username of the user.</summary>
		public string Username { get; set; } = string.Empty;

		/// <summary>Gets or sets the hashed password of the user.</summary>
		public string PasswordHash { get; set; } = string.Empty;

		/// <summary>Gets or sets the full name of the user.</summary>
		public string FullName { get; set; } = string.Empty;

		/// <summary>Gets or sets the mobile number of the user.</summary>
		public string PhoneNumber { get; set; } = string.Empty;

		/// <summary>Gets or sets the role of the user.</summary>
		public string Role { get; set; } = "Member"; // Default role is "Member", other possible value is "Admin", "SuperAdmin", etc.

		/// <summary>Gets or sets a value indicating whether the user is currently out of station.</summary>
		public bool IsOutOfStation { get; set; } = false;

		/// <summary>Gets or sets the timestamp when the user was created.</summary>
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		/// <summary>Gets or sets the timestamp when the user was last updated.</summary>
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

		#endregion

		//Navigation properties

		/// <summary>Gets or sets the collection of task assignments associated with the user.</summary>
		public ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();

		/// <summary>Gets or sets the collection of task histories associated with the user.</summary>
		public ICollection<TaskHistory> TaskHistories { get; set; } = new List<TaskHistory>();
	}
}
