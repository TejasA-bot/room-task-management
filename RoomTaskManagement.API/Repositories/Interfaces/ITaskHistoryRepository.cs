using RoomTaskManagement.API.Models.Entities;

namespace RoomTaskManagement.API.Repositories.Interfaces
{
	public interface ITaskHistoryRepository : IGenericRepository<TaskHistory>
	{
		public Task<IEnumerable<TaskHistory>> GetTaskHistoryAsync(int taskId);
		public Task<IEnumerable<TaskHistory>> GetUserHistoryAsync(int userId);
	}
}
