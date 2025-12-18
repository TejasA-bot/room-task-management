namespace RoomTaskManagement.API.Models.Requests
{
	/// <summary>Request model for user login.</summary>
	public class LoginRequest
	{
		/// <summary>Gets or sets the username.</summary>
		public string Username { get; set; } = string.Empty;

		/// <summary>Gets or sets the password.</summary>
		public string Password { get; set; } = string.Empty;
	}
}
