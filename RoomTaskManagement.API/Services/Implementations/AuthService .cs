using RoomTaskManagement.API.Models.DTOs;
using RoomTaskManagement.API.Models.Entities;
using RoomTaskManagement.API.Models.Requests;
using RoomTaskManagement.API.Models.Responses;
using RoomTaskManagement.API.Repositories.Interfaces;
using RoomTaskManagement.API.Services.Interfaces;
using RoomTaskManagement.API.Helpers;

namespace RoomTaskManagement.API.Services.Implementations
{
	public class AuthService : IAuthService
	{
		private readonly IUserRepository _userRepository;
		private readonly JwtHelper _jwtHelper;

		public AuthService(IUserRepository userRepository, JwtHelper jwtHelper)
		{
			_userRepository = userRepository;
			_jwtHelper = jwtHelper;
		}

		public async Task<LoginResponse?> LoginAsync(LoginRequest request)
		{
			if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
				return null;

			var user = await _userRepository.GetByUsernameAsync(request.Username);

			if (user == null)
				return null;

			// Verify password
			if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
				return null;

			var userDto = MapToDto(user);
			var token = _jwtHelper.GenerateToken(userDto);

			return new LoginResponse
			{
				Token = token,
				User = userDto
			};
		}

		public async Task<UserDto?> CreateUserAsync(CreateUserRequest request, int createdBy)
		{
			// Basic validation
			if (string.IsNullOrWhiteSpace(request.Username) ||
				string.IsNullOrWhiteSpace(request.Password) ||
				string.IsNullOrWhiteSpace(request.FullName) ||
				string.IsNullOrWhiteSpace(request.PhoneNumber) ||
				string.IsNullOrWhiteSpace(request.Role))
			{
				return null;
			}

			if(createdBy < 1) return null;

			// Check if username already exists
			var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
			if (existingUser != null)
				return null;

			// Validate phone number format (basic validation)
			if (string.IsNullOrWhiteSpace(request.PhoneNumber) || request.PhoneNumber.Length < 10)
				return null;

			var user = new User
			{
				Username = request.Username,
				PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
				FullName = request.FullName,
				PhoneNumber = request.PhoneNumber,
				Role = request.Role,
				IsOutOfStation = false,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow
			};

			var createdUser = await _userRepository.AddAsync(user);
			return MapToDto(createdUser);
		}

		public string GenerateJwtToken(UserDto user)
		{
			return _jwtHelper.GenerateToken(user);
		}

		private UserDto MapToDto(User user)
		{
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
	}
}
