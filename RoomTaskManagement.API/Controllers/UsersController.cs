using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomTaskManagement.API.Models.DTOs;
using RoomTaskManagement.API.Models.Responses;
using RoomTaskManagement.API.Services.Interfaces;
using System.Security.Claims;

namespace RoomTaskManagement.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class UsersController : ControllerBase
	{
		private readonly IUserService _userService;

		public UsersController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllUsers()
		{
			var users = await _userService.GetAllUsersAsync();

			if (users == null || !users.Any())
			{
				return NotFound("No users found.");
			}

			return Ok(new ApiResponse<IEnumerable<UserDto>>
			{
				Success = true,
				Message = "Users retrieved successfully.",
				Data = users
			});
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetUserById(int id)
		{
			var user = await _userService.GetUserByIdAsync(id);

			if (user == null)
			{
				return NotFound($"User with ID {id} not found.");
			}

			return Ok(new ApiResponse<UserDto>
			{
				Success = true,
				Message = "User retrieved successfully.",
				Data = user
			});
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
		{
			//validate the paramters
			if (userDto == null || id != userDto.Id || id < 1)
			{
				return BadRequest("Invalid user data.");
			}

			var result = await _userService.UpdateUserAsync(id, userDto);
			
			if (!result)
			{
				return NotFound(new ApiResponse<object>
				{
					Success = false,
					Message = "User not found or update failed"
				});
			}
			
			return Ok(new ApiResponse<object>
			{
				Success = true,
				Message = "User updated successfully."
			});
		}

		[HttpPost("{id}/toggle-out-of-station")]
		public async Task<IActionResult> ToggleOutOfStation(int id)
		{
			// Users can toggle their own status, or admin can toggle anyone's
			var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
			var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

			// Check permissions: User can toggle own status OR must be Admin/SuperAdmin to toggle others
			bool canToggle = (currentUserId == id) || (userRole == "SuperAdmin" || userRole == "Admin");

			if (!canToggle)
			{
				return Forbid();
			}

			var result = await _userService.ToggleOutOfStationAsync(id);

			if (!result)
			{
				return NotFound(new ApiResponse<object>
				{
					Success = false,
					Message = "User not found"
				});
			}

			return Ok(new ApiResponse<object>
			{
				Success = true,
				Message = "Out of station status toggled successfully"
			});
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "SuperAdmin, Admin")]
		public async Task<IActionResult> DeleteUser(int id)
		{
			if(id < 1)
			{
				return BadRequest("Invalid user ID.");
			}

			var result = await _userService.DeleteUserAsync(id);

			if (!result)
			{
				return BadRequest(new ApiResponse<object>
				{
					Success = false,
					Message = "User not found or cannot delete SuperAdmin"
				});
			}

			return Ok(new ApiResponse<object>
			{
				Success = true,
				Message = "User deleted successfully"
			});
		}
	}
}
