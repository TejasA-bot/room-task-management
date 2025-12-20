using RoomTaskManagement.API.Models.Entities;

namespace RoomTaskManagement.API.Repositories.Interfaces
{
	public interface ITaskAssignmentRepository : IGenericRepository<TaskAssignment>
	{
		public Task<TaskAssignment?> GetActiveAssignmentForTaskAsync(int taskId);
		public Task<IEnumerable<TaskAssignment>> GetUserTaskHistoryAsync(int userId, int taskId);
		public Task<TaskAssignment?> GetLastCompletedAssignmentAsync(int taskId, int userId);
	}
}
