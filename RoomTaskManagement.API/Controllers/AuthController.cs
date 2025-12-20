using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomTaskManagement.API.Models.Requests;
using RoomTaskManagement.API.Models.Responses;
using RoomTaskManagement.API.Services.Interfaces;

namespace RoomTaskManagement.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		/// <summary>Login endpoint</summary>
		/// <param name="request">The request with the required username and password</param>
		/// <returns></returns>
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			// Validate request
			if ((request == null) || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
			{
				return BadRequest(new ApiResponse<object>
				{
					Success = false,
					Message = "Username and password are required."
				});
			}

			// Attempt to login
			var result = await _authService.LoginAsync(request);

			if (result == null)
			{
				return Unauthorized(new ApiResponse<object>
				{
					Success = false,
					Message = "Invalid username or password."
				});
			}

			return Ok(new ApiResponse<LoginResponse>
			{
				Success = true,
				Message = "Login successful.",
				Data = result
			});
		}

		[Authorize(Roles = "Admin,SuperAdmin")]
		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] CreateUserRequest request)
		{
			// Validate request
			if (request == null ||
				string.IsNullOrWhiteSpace(request.Username) ||
				string.IsNullOrWhiteSpace(request.Password) ||
				string.IsNullOrWhiteSpace(request.FullName) ||
				string.IsNullOrWhiteSpace(request.PhoneNumber))
			{
				return BadRequest(new ApiResponse<object>
				{
					Success = false,
					Message = "All fields are required."
				});
			}

			// Assuming SuperAdmin (Id = 1) is creating the user for now
			var result = await _authService.CreateUserAsync(request, createdBy: 1);

			if(result == null)
			{
				return BadRequest(new ApiResponse<object>
				{
					Success = false,
					Message = "Username already exists or invalid data"
				});
			}

			return Ok(new ApiResponse<object>
			{
				Success = true,
				Message = "User created successfully.",
				Data = result
			});

		}
	}
}
