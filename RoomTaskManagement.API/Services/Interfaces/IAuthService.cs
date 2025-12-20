using Microsoft.AspNetCore.Identity.Data;
using RoomTaskManagement.API.Models.DTOs;
using RoomTaskManagement.API.Models.Requests;
using RoomTaskManagement.API.Models.Responses;

namespace RoomTaskManagement.API.Services.Interfaces
{
	public interface IAuthService
	{
		public Task<LoginResponse?> LoginAsync(Models.Requests.LoginRequest request);

		public Task<UserDto?> CreateUserAsync(CreateUserRequest request, int createdBy);
		public string GenerateJwtToken(UserDto user);
	}
}
