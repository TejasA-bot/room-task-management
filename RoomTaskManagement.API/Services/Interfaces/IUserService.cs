using RoomTaskManagement.API.Models.DTOs;

namespace RoomTaskManagement.API.Services.Interfaces
{
	public interface IUserService
	{
		public Task<IEnumerable<UserDto>> GetAllUsersAsync();
		public Task<UserDto?> GetUserByIdAsync(int id);
		public Task<bool> ToggleOutOfStationAsync(int userId);
		public Task<bool> UpdateUserAsync(int userId, UserDto userDto);
		public Task<bool> DeleteUserAsync(int userId);
	}
}
