using RoomTaskManagement.API.Models.DTOs;
using RoomTaskManagement.API.Models.Requests;

namespace RoomTaskManagement.API.Services.Interfaces
{
	public interface ITaskService
	{
		public Task<IEnumerable<TaskDto>> GetAllTasksAsync();
		public Task<TaskDto?> GetTaskByIdAsync(int id);
		public Task<TaskDto> CreateTaskAsync(CreateTaskRequest request, int createdBy);
		public Task<bool> TriggerTaskAsync(TriggerTaskRequest request);
		public Task<bool> CompleteTaskAsync(int taskId, int userId);
		public Task<bool> DeleteTaskAsync(int taskId);
	}
}
