namespace RoomTaskManagement.API.Models.Responses
{
	/// <summary>Generic API response model.</summary>
	public class ApiResponse<T>
	{
		/// <summary>Indicates whether the API call was successful.</summary>
		public bool Success { get; set; }

		/// <summary>Message providing additional information about the API response.</summary>
		public string Message { get; set; } = string.Empty;

		/// <summary>Data returned by the API call.</summary>
		public T? Data { get; set; }
	}
}
