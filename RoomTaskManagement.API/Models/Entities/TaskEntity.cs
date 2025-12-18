namespace RoomTaskManagement.API.Models.Entities
{
	/// <summary>
	/// Represents a task entity in the Room Task Management system.
	/// </summary>
	public class TaskEntity
	{
		#region

		/// <summary>Gets or sets the unique identifier for the task.</summary>
		public int Id { get; set; }

		/// <summary>Gets or sets the name of the task.</summary>
		public string TaskName { get; set; } = string.Empty;

		/// <summary>Gets or sets the description of the task.</summary>
		public string? Description { get; set; }

		/// <summary>Gets or sets a value indicating whether the task is active.</summary>
		public bool IsActive { get; set; } = true;

		/// <summary>Gets or sets the user Id who the task was created by.</summary>
		public int CreatedBy { get; set; }

		/// <summary>Gets or sets the timestamp when the task was created.</summary>
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		#endregion

		//Navigation properties

		/// <summary>Gets or sets the user who created the task.</summary>
		public User? Creator { get; set; }

		/// <summary>Gets or sets the collection of task assignments associated with the task.</summary>
		public ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();

		/// <summary>Gets or sets the collection of task histories associated with the task.</summary>
		public ICollection<TaskHistory> TaskHistories { get; set; } = new List<TaskHistory>();
	}
}
