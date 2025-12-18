namespace RoomTaskManagement.API.Models.Entities
{
	/// <summary>Represents the assignment of a task to a user or entity.</summary>
	public class TaskAssignment
	{
		#region Properties
		/// <summary>Gets or sets the unique identifier for the task assignment.</summary>
		public int Id { get; set; }

		/// <summary>Gets or sets the identifier of the task.</summary>
		public int TaskId { get; set; }

		/// <summary>Gets or sets the identifier of the user to whom the task is assigned.</summary>
		public int UserId { get; set; }

		/// <summary>Gets or sets the timestamp when the task was completed.</summary>
		public DateTime CompletedAt { get; set; } = DateTime.Now;

		/// <summary>Gets or sets the status of the task assignment.</summary>
		public string Status { get; set; } = "Pending"; // Possible values: "Pending", "InProgress", "Completed", "Cancelled"

		/// <summary>Gets or sets the identifier of the user who triggered the assignment.</summary>
		public int? TriggeredBy { get; set; }

		/// <summary>Gets or sets the timestamp when the assignment was triggered.</summary>
		public DateTime? TriggeredAt { get; set; } = DateTime.Now;
		#endregion

		//Navigation properties

		/// <summary>Gets or sets the user associated with the task assignment.</summary>
		public TaskEntity? Task { get; set; }

		/// <summary>Gets or sets the user to whom the task is assigned.</summary>
		public User? User { get; set; }

		/// <summary>Gets or sets the user who triggered the assignment.</summary>
		public User? Triggerer { get; set; }
	}
}
