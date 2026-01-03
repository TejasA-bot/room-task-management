using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomTaskManagement.API.Models.DTOs;
using RoomTaskManagement.API.Models.Requests;
using RoomTaskManagement.API.Models.Responses;
using RoomTaskManagement.API.Services.Interfaces;
using System.Security.Claims;

namespace RoomTaskManagement.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class TasksController : ControllerBase
	{
		private readonly ITaskService _taskService;

		public TasksController(ITaskService taskService)
		{
			_taskService = taskService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllTasks()
		{
			var tasks = await _taskService.GetAllTasksAsync();

			if (!tasks.Any())
			{
				return NotFound(new ApiResponse<object>
				{
					Success = false,
					Message = "No tasks available please add tasks."
				});
			}

			return Ok(new ApiResponse<IEnumerable<TaskDto>>
			{
				Success = true,
				Message = "Tasks retrieved successfully",
				Data = tasks
			});
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetTaskById(int id)
		{
			if(id < 1)
			{
				return BadRequest(new ApiResponse<object>
				{
					Success = false,
					Message = "Invalid task ID"
				});
			}

			var task = await _taskService.GetTaskByIdAsync(id);

			if (task == null)
			{
				return NotFound(new ApiResponse<object>
				{
					Success = false,
					Message = "Task not found"
				});
			}

			return Ok(new ApiResponse<TaskDto>
			{
				Success = true,
				Message = "Task retrieved successfully",
				Data = task
			});
		}

		[HttpPost]
		[Authorize(Roles = "SuperAdmin,Admin")]
		public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
		{
			if (string.IsNullOrWhiteSpace(request.TaskName))
			{
				return BadRequest(new ApiResponse<object>
				{
					Success = false,
					Message = "Task name is required"
				});
			}

			var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
			var task = await _taskService.CreateTaskAsync(request, currentUserId);

			return Ok(new ApiResponse<TaskDto>
			{
				Success = true,
				Message = "Task created successfully",
				Data = task
			});
		}

		[HttpPost("{id}/trigger")]
		public async Task<IActionResult> TriggerTask(int id)
		{
			if (id < 1)
			{
				return BadRequest(new ApiResponse<object>
				{
					Success = false,
					Message = "Invalid task ID"
				});
			}

			var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

			var request = new TriggerTaskRequest
			{
				TaskId = id,
				TriggeredBy = currentUserId
			};

			var result = await _taskService.TriggerTaskAsync(request);

			if (!result)
			{
				return BadRequest(new ApiResponse<object>
				{
					Success = false,
					Message = "Task cannot be triggered. It may already be in progress or inactive."
				});
			}

			return Ok(new ApiResponse<object>
			{
				Success = true,
				Message = "Task triggered successfully. Notification sent to assigned user."
			});
		}


		[HttpPost("{taskId}/complete")]
		public async Task<IActionResult> CompleteTask(int taskId)
		{
			if (taskId < 1)
			{
				return BadRequest(new ApiResponse<object>
				{
					Success = false,
					Message = "Invalid task ID"
				});
			}

			var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
			var result = await _taskService.CompleteTaskAsync(taskId, currentUserId);

			if (!result)
			{
				return BadRequest(new ApiResponse<object>
				{
					Success = false,
					Message = "Task cannot be completed. You may not be assigned to this task."
				});
			}

			return Ok(new ApiResponse<object>
			{
				Success = true,
				Message = "Task completed successfully"
			});
		}

		[HttpPost("{taskId}/approve")]
		public async Task<IActionResult> ApproveTask(int taskId)
		{
			var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
			var result = await _taskService.ApproveTaskAsync(taskId, currentUserId);

			if (!result)
			{
				return BadRequest(new ApiResponse<object>
				{
					Success = false,
					Message = "Cannot approve task. You may not be the triggerer."
				});
			}

			return Ok(new ApiResponse<object>
			{
				Success = true,
				Message = "Task approved successfully"
			});
		}

		[HttpPost("{taskId}/reject")]
		public async Task<IActionResult> RejectTask(int taskId)
		{
			var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
			var result = await _taskService.RejectTaskAsync(taskId, currentUserId);

			if (!result)
			{
				return BadRequest(new ApiResponse<object>
				{
					Success = false,
					Message = "Cannot reject task. You may not be the triggerer."
				});
			}

			return Ok(new ApiResponse<object>
			{
				Success = true,
				Message = "Task rejected - user notified to redo"
			});
		}


		[HttpDelete("{id}")]
		[Authorize(Roles = "SuperAdmin,Admin")]
		public async Task<IActionResult> DeleteTask(int id)
		{
			if (id < 1)
			{
				return BadRequest(new ApiResponse<object>
				{
					Success = false,
					Message = "Invalid task ID"
				});
			}
			var result = await _taskService.DeleteTaskAsync(id);

			if (!result)
			{
				return NotFound(new ApiResponse<object>
				{
					Success = false,
					Message = "Task not found"
				});
			}

			return Ok(new ApiResponse<object>
			{
				Success = true,
				Message = "Task deleted successfully"
			});
		}
	}
}
