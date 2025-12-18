namespace RoomTaskManagement.API.Models.Requests
{
	/// <summary>Request model for triggering a task.</summary>
	public class TriggerTaskRequest
	{
		/// <summary>Gets or sets the ID of the task to be triggered.</summary>
		public int TaskId { get; set; }

		/// <summary>Gets or sets the ID of the user who is triggering the task.</summary>
		public int TriggeredBy { get; set; }
	}
}
