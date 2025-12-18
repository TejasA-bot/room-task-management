namespace RoomTaskManagement.API.Models.Requests
{
	/// <summary>Request model for creating a new task.</summary>
	public class CreateTaskRequest
	{
		/// <summary>Name of the task to be created.</summary>
		public string TaskName { get; set; } = string.Empty;

		/// <summary>Description of the task to be created.</summary>
		public string? Description { get; set; }
	}
}
