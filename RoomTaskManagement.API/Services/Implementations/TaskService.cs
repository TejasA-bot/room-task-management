using RoomTaskManagement.API.Models.DTOs;
using RoomTaskManagement.API.Models.Entities;
using RoomTaskManagement.API.Models.Requests;
using RoomTaskManagement.API.Repositories.Interfaces;
using RoomTaskManagement.API.Services.Interfaces;
using RoomTaskManagement.API.BusinessLogic;
using Microsoft.AspNetCore.SignalR;
using RoomTaskManagement.API.Hubs;

namespace RoomTaskManagement.API.Services.Implementations
{
	public class TaskService : ITaskService
	{
		private readonly ITaskRepository _taskRepository;
		private readonly ITaskAssignmentRepository _taskAssignmentRepository;
		private readonly ITaskHistoryRepository _taskHistoryRepository;
		private readonly IUserRepository _userRepository;
		private readonly TaskTurnCalculator _turnCalculator;
		private readonly IWhatsAppService _whatsAppService;
		private readonly IHubContext<TaskHub> _hubContext;

		public TaskService(
			ITaskRepository taskRepository,
			ITaskAssignmentRepository taskAssignmentRepository,
			ITaskHistoryRepository taskHistoryRepository,
			IUserRepository userRepository,
			TaskTurnCalculator turnCalculator,
			IWhatsAppService whatsAppService,
			IHubContext<TaskHub> hubContext)
		{
			_taskRepository = taskRepository;
			_taskAssignmentRepository = taskAssignmentRepository;
			_taskHistoryRepository = taskHistoryRepository;
			_userRepository = userRepository;
			_turnCalculator = turnCalculator;
			_whatsAppService = whatsAppService;
			_hubContext = hubContext;	
		}

		public async Task<IEnumerable<TaskDto>> GetAllTasksAsync()
		{
			var tasks = await _taskRepository.GetActiveTasksAsync();
			var taskDtos = new List<TaskDto>();

			foreach (var task in tasks)
			{
				var activeAssignment = await _taskAssignmentRepository.GetActiveAssignmentForTaskAsync(task.Id);
				var nextUser = await _turnCalculator.CalculateNextUserForTaskAsync(task.Id);

				taskDtos.Add(new TaskDto
				{
					Id = task.Id,
					TaskName = task.TaskName,
					Description = task.Description,
					IsActive = task.IsActive,
					CreatedByName = task.Creator?.FullName ?? "Unknown",
					CreatedAt = task.CreatedAt,
					CurrentAssignedTo = activeAssignment?.User?.FullName,
					CurrentStatus = activeAssignment?.Status ?? "Available",
					CanTrigger = activeAssignment == null || activeAssignment.Status == "Completed"
				});
			}

			return taskDtos;
		}

		public async Task<TaskDto?> GetTaskByIdAsync(int id)
		{
			if(id < 1)
				return null;

			var task = await _taskRepository.GetTaskWithAssignmentsAsync(id);
			if (task == null)
				return null;

			var activeAssignment = await _taskAssignmentRepository.GetActiveAssignmentForTaskAsync(task.Id);

			return new TaskDto
			{
				Id = task.Id,
				TaskName = task.TaskName,
				Description = task.Description,
				IsActive = task.IsActive,
				CreatedByName = task.Creator?.FullName ?? "Unknown",
				CreatedAt = task.CreatedAt,
				CurrentAssignedTo = activeAssignment?.User?.FullName,
				CurrentStatus = activeAssignment?.Status ?? "Available",
				CanTrigger = activeAssignment == null || activeAssignment.Status == "Completed"
			};
		}

		public async Task<TaskDto> CreateTaskAsync(CreateTaskRequest request, int createdBy)
		{
			if (string.IsNullOrWhiteSpace(request.TaskName))
				throw new ArgumentException("Task name cannot be empty.");

			if(request == null)
				throw new ArgumentNullException(nameof(request));

			var task = new TaskEntity
			{
				TaskName = request.TaskName,
				Description = request.Description,
				IsActive = true,
				CreatedBy = createdBy,
				CreatedAt = DateTime.UtcNow
			};

			var createdTask = await _taskRepository.AddAsync(task);
			var creator = await _userRepository.GetByIdAsync(createdBy);

			return new TaskDto
			{
				Id = createdTask.Id,
				TaskName = createdTask.TaskName,
				Description = createdTask.Description,
				IsActive = createdTask.IsActive,
				CreatedByName = creator?.FullName ?? "Unknown",
				CreatedAt = createdTask.CreatedAt,
				CurrentStatus = "Available",
				CanTrigger = true
			};
		}

		public async Task<bool> TriggerTaskAsync(TriggerTaskRequest request)
		{
			if (request == null || request.TaskId < 1 || request.TriggeredBy < 1)
				throw new ArgumentNullException(nameof(request));

			// Check if task exists
			var task = await _taskRepository.GetByIdAsync(request.TaskId);
			if (task == null || !task.IsActive)
				return false;

			// Check if there's already an active assignment
			var activeAssignment = await _taskAssignmentRepository.GetActiveAssignmentForTaskAsync(request.TaskId);
			if (activeAssignment != null && activeAssignment.Status != "Completed")
				return false;

			// Calculate next user turn
			var nextUser = await _turnCalculator.CalculateNextUserForTaskAsync(request.TaskId);
			if (nextUser == null)
				return false;

			// Create new assignment
			var assignment = new TaskAssignment
			{
				TaskId = request.TaskId,
				UserId = nextUser.Id,
				Status = "InProgress",
				TriggeredBy = request.TriggeredBy,
				TriggeredAt = DateTime.UtcNow
			};

			await _taskAssignmentRepository.AddAsync(assignment);

			// Add to history
			var history = new TaskHistory
			{
				TaskId = request.TaskId,
				UserId = nextUser.Id,
				Action = "Triggered",
				Timestamp = DateTime.UtcNow
			};

			await _taskHistoryRepository.AddAsync(history);

			//// Send WhatsApp notification
			//await _whatsAppService.SendTaskNotificationAsync(nextUser.PhoneNumber, task.TaskName, nextUser.FullName);

			// Send SignalR notification to ALL connected clients
			await _hubContext.Clients.All.SendAsync("ReceiveTaskTriggered", new
			{
				taskId = task.Id,
				taskName = task.TaskName,
				assignedTo = nextUser.FullName,
				assignedUserId = nextUser.Id,
				timestamp = DateTime.Now
			});

			return true;
		}

		public async Task<bool> CompleteTaskAsync(int taskId, int userId)
		{
			if (taskId < 1 || userId < 1)
				return false;

			var activeAssignment = await _taskAssignmentRepository.GetActiveAssignmentForTaskAsync(taskId);

			if (activeAssignment == null || activeAssignment.UserId != userId)
				return false;

			activeAssignment.Status = "PendingApproval";
			activeAssignment.CompletedAt = DateTime.Now;

			await _taskAssignmentRepository.UpdateAsync(activeAssignment);

			// Add to history
			var history = new TaskHistory
			{
				TaskId = taskId,
				UserId = userId,
				Action = "PendingApproval",
				Timestamp = DateTime.Now
			};

			// Notify triggerer for approval
			if (activeAssignment.TriggeredBy.HasValue)
			{
				var triggerer = await _userRepository.GetByIdAsync(activeAssignment.TriggeredBy.Value);
				var task = await _taskRepository.GetByIdAsync(taskId);
				var completer = await _userRepository.GetByIdAsync(userId);

				await _hubContext.Clients.All.SendAsync("ReceiveApprovalRequest", new
				{
					taskId = taskId,
					taskName = task.TaskName,
					completedBy = completer.FullName,
					triggererId = triggerer.Id,
					triggererName = triggerer.FullName,
					timestamp = DateTime.UtcNow
				});
			}

			await _taskHistoryRepository.AddAsync(history);

			return true;
		}

		public async Task<bool> ApproveTaskAsync(int taskId, int approverId)
		{
			var activeAssignment = await _taskAssignmentRepository.GetActiveAssignmentForTaskAsync(taskId);

			if (activeAssignment == null || activeAssignment.Status != "PendingApproval")
				return false;

			//// Only triggerer can approve
			//if (activeAssignment.TriggeredBy != approverId)
			//	return false;

			activeAssignment.Status = "Completed";
			activeAssignment.IsApproved = true;
			activeAssignment.ApprovedBy = approverId;
			activeAssignment.ApprovedAt = DateTime.Now;

			await _taskAssignmentRepository.UpdateAsync(activeAssignment);

			// Add to history
			var history = new TaskHistory
			{
				TaskId = taskId,
				UserId = activeAssignment.UserId,
				Action = "Approved",
				Timestamp = DateTime.Now
			};
			await _taskHistoryRepository.AddAsync(history);

			// Notify all
			var task = await _taskRepository.GetByIdAsync(taskId);
			var completer = await _userRepository.GetByIdAsync(activeAssignment.UserId);

			await _hubContext.Clients.All.SendAsync("ReceiveTaskCompleted", new
			{
				taskId = taskId,
				taskName = task.TaskName,
				completedBy = completer.FullName,
				timestamp = DateTime.Now
			});

			return true;
		}

		public async Task<bool> RejectTaskAsync(int taskId, int approverId)
		{
			var activeAssignment = await _taskAssignmentRepository.GetActiveAssignmentForTaskAsync(taskId);

			if (activeAssignment == null || activeAssignment.Status != "PendingApproval")
				return false;

			// Only triggerer can reject
			if (activeAssignment.TriggeredBy != approverId)
				return false;

			// Reset to InProgress
			activeAssignment.Status = "InProgress";
			activeAssignment.CompletedAt = DateTime.Now;

			await _taskAssignmentRepository.UpdateAsync(activeAssignment);

			// Notify user
			var task = await _taskRepository.GetByIdAsync(taskId);
			var user = await _userRepository.GetByIdAsync(activeAssignment.UserId);

			await _hubContext.Clients.All.SendAsync("ReceiveTaskRejected", new
			{
				taskId = taskId,
				taskName = task.TaskName,
				userId = user.Id,
				userName = user.FullName,
				timestamp = DateTime.UtcNow
			});

			return true;
		}


		public async Task<bool> DeleteTaskAsync(int taskId)
		{
			if (taskId < 1) throw new ArgumentException("Invalid task ID.");

			var task = await _taskRepository.GetByIdAsync(taskId);
			if (task == null)
				return false;

			task.IsActive = false;
			await _taskRepository.UpdateAsync(task);
			return true;
		}
	}
}
