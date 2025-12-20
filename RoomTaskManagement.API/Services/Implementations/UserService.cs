using RoomTaskManagement.API.Models.DTOs;
using RoomTaskManagement.API.Repositories.Interfaces;
using RoomTaskManagement.API.Services.Interfaces;

namespace RoomTaskManagement.API.Services.Implementations
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;

		public UserService(IUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
		{
			var users = await _userRepository.GetActiveUsersAsync();
			return users.Select(u => new UserDto
			{
				Id = u.Id,
				Username = u.Username,
				FullName = u.FullName,
				PhoneNumber = u.PhoneNumber,
				Role = u.Role,
				IsOutOfStation = u.IsOutOfStation
			});
		}

		public async Task<UserDto?> GetUserByIdAsync(int id)
		{
			var user = await _userRepository.GetByIdAsync(id);
			if (user == null)
				return null;

			return new UserDto
			{
				Id = user.Id,
				Username = user.Username,
				FullName = user.FullName,
				PhoneNumber = user.PhoneNumber,
				Role = user.Role,
				IsOutOfStation = user.IsOutOfStation
			};
		}

		public async Task<bool> ToggleOutOfStationAsync(int userId)
		{
			var user = await _userRepository.GetByIdAsync(userId);
			if (user == null)
				return false;

			user.IsOutOfStation = !user.IsOutOfStation;
			user.UpdatedAt = DateTime.UtcNow;

			await _userRepository.UpdateAsync(user);
			return true;
		}

		public async Task<bool> UpdateUserAsync(int userId, UserDto userDto)
		{
			var user = await _userRepository.GetByIdAsync(userId);
			if (user == null)
				return false;

			user.FullName = userDto.FullName;
			user.PhoneNumber = userDto.PhoneNumber;
			user.Role = userDto.Role;
			user.UpdatedAt = DateTime.UtcNow;

			await _userRepository.UpdateAsync(user);
			return true;
		}

		public async Task<bool> DeleteUserAsync(int userId)
		{
			var user = await _userRepository.GetByIdAsync(userId);
			if (user == null || user.Role == "SuperAdmin")
				return false;

			await _userRepository.DeleteAsync(user);
			return true;
		}
	}
}
