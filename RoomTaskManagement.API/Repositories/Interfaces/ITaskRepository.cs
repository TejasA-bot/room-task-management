using RoomTaskManagement.API.Models.Entities;

namespace RoomTaskManagement.API.Repositories.Interfaces
{
	public interface ITaskRepository : IGenericRepository<TaskEntity>
	{
		public Task<IEnumerable<TaskEntity>> GetActiveTasksAsync();

		public Task<TaskEntity?> GetTaskWithAssignmentsAsync(int taskId);
	}
}
