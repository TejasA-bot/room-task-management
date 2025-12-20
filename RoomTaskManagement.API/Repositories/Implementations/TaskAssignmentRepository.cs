using Microsoft.EntityFrameworkCore;
using RoomTaskManagement.API.Data;
using RoomTaskManagement.API.Models.Entities;
using RoomTaskManagement.API.Repositories.Interfaces;

namespace RoomTaskManagement.API.Repositories.Implementations
{
	public class TaskAssignmentRepository : GenericRepository<TaskAssignment>, ITaskAssignmentRepository
	{
		public TaskAssignmentRepository(ApplicationDbContext context) : base(context)
		{
		}

		public async Task<TaskAssignment?> GetActiveAssignmentForTaskAsync(int taskId)
		{
			return await _dbSet
				.Include(ta => ta.User)
				.Include(ta => ta.Triggerer)
				.FirstOrDefaultAsync(ta => ta.TaskId == taskId && ta.Status != "Completed");
		}

		public async Task<IEnumerable<TaskAssignment>> GetUserTaskHistoryAsync(int userId, int taskId)
		{
			return await _dbSet
				.Where(ta => ta.UserId == userId && ta.TaskId == taskId && ta.Status == "Completed")
				.OrderByDescending(ta => ta.CompletedAt)
				.ToListAsync();
		}

		public async Task<TaskAssignment?> GetLastCompletedAssignmentAsync(int taskId, int userId)
		{
			return await _dbSet
				.Where(ta => ta.TaskId == taskId && ta.UserId == userId && ta.Status == "Completed")
				.OrderByDescending(ta => ta.CompletedAt)
				.FirstOrDefaultAsync();
		}
	}
}
