namespace RoomTaskManagement.API.Models.DTOs
{
	/// <summary>Data Transfer Object for User information.</summary>
	public class UserDto
	{
		/// <summary>Gets or sets the unique identifier of the user.</summary>
		public int Id { get; set; }

		/// <summary>Gets or sets the username of the user.</summary>
		public string Username { get; set; } = string.Empty;

		/// <summary>Gets or sets the full name of the user.</summary>
		public string FullName { get; set; } = string.Empty;

		/// <summary>Gets or sets the phone number of the user.</summary>
		public string PhoneNumber { get; set; } = string.Empty;

		/// <summary>Gets or sets the role of the user.</summary>
		public string Role { get; set; } = string.Empty;

		/// <summary>Gets or sets a value indicating whether the user is out of station.</summary>
		public bool IsOutOfStation { get; set; }
	}
}
