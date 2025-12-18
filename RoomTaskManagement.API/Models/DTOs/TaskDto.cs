namespace RoomTaskManagement.API.Models.DTOs
{
	/// <summary>Data Transfer Object for Task information</summary>
	public class TaskDto
	{
		/// <summary>Unique identifier for the task</summary>
		public int Id { get; set; }

		/// <summary>Name of the task</summary>
		public string TaskName { get; set; } = string.Empty;

		/// <summary>Description of the task</summary>
		public string? Description { get; set; }

		/// <summary>Indicates whether the task is active</summary>
		public bool IsActive { get; set; }

		/// <summary>Name of the user who created the task</summary>
		public string CreatedByName { get; set; } = string.Empty;

		/// <summary>Timestamp when the task was created</summary>
		public DateTime CreatedAt { get; set; }

		/// <summary>Name of the user currently assigned to the task, if any</summary>
		public string? CurrentAssignedTo { get; set; }

		/// <summary>Current status of the task (e.g., Available, In Progress, Completed)</summary>
		public string CurrentStatus { get; set; } = "Available";

		/// <summary>Indicates whether the task can be triggered</summary>
		public bool CanTrigger { get; set; } = true;
	}
}
