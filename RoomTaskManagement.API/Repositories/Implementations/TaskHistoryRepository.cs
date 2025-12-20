using Microsoft.EntityFrameworkCore;
using RoomTaskManagement.API.Data;
using RoomTaskManagement.API.Models.Entities;
using RoomTaskManagement.API.Repositories.Interfaces;

namespace RoomTaskManagement.API.Repositories.Implementations
{
	public class TaskHistoryRepository : GenericRepository<TaskHistory>, ITaskHistoryRepository
	{
		public TaskHistoryRepository(ApplicationDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<TaskHistory>> GetTaskHistoryAsync(int taskId)
		{
			return await _dbSet
				.Where(th => th.TaskId == taskId)
				.Include(th => th.User)
				.OrderByDescending(th => th.Timestamp)
				.ToListAsync();
		}

		public async Task<IEnumerable<TaskHistory>> GetUserHistoryAsync(int userId)
		{
			return await _dbSet
				.Where(th => th.UserId == userId)
				.Include(th => th.Task)
				.OrderByDescending(th => th.Timestamp)
				.ToListAsync();
		}
	}
}
