namespace RoomTaskManagement.API.Models.Entities
{
	/// <summary>Represents the history of actions taken on tasks.</summary>
	public class TaskHistory
	{
		#region Properties

		/// <summary>Gets or sets the unique identifier for the task history entry.</summary>
		public int Id { get; set; }

		/// <summary>Gets or sets the identifier of the task associated with this history entry.</summary>
		public int TaskId { get; set; }

		/// <summary>Gets or sets the identifier of the user who performed the action.</summary>
		public int UserId { get; set; }

		/// <summary>Gets or sets the action performed on the task.</summary>
		public string Action { get; set; } = string.Empty; // Triggered, Completed, Skipped

		/// <summary>Gets or sets the timestamp when the action was performed.</summary>
		public DateTime Timestamp { get; set; } = DateTime.UtcNow;

		#endregion

		// Navigation properties

		/// <summary>Gets or sets the task associated with this history entry.</summary>
		public TaskEntity? Task { get; set; }

		/// <summary>Gets or sets the user who performed the action.</summary>
		public User? User { get; set; }
	}
}
