using RoomTaskManagement.API.Models.DTOs;

namespace RoomTaskManagement.API.Models.Responses
{
	/// <summary>Response model for user login.</summary>
	public class LoginResponse
	{
		/// <summary>Gets or sets the authentication token.</summary>
		public string Token { get; set; } = string.Empty;

		/// <summary>Gets or sets the user details.</summary>
		public UserDto User { get; set; } = new UserDto();
	}
}
