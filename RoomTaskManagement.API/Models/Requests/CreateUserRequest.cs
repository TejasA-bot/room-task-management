namespace RoomTaskManagement.API.Models.Requests
{
	/// <summary>Request model for creating a new user.</summary>
	public class CreateUserRequest
	{
		/// <summary>Gets or sets the username.</summary>
		public string Username { get; set; } = string.Empty;

		/// <summary>Gets or sets the password.</summary>
		public string Password { get; set; } = string.Empty;

		/// <summary>Gets or sets the full name of the user.</summary>
		public string FullName { get; set; } = string.Empty;

		/// <summary>Gets or sets the phone number of the user.</summary>
		public string PhoneNumber { get; set; } = string.Empty;

		/// <summary>Gets or sets the role of the user.</summary>
		public string Role { get; set; } = "Member";
	}
}
