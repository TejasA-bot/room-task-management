using Microsoft.EntityFrameworkCore;
using RoomTaskManagement.API.Data;
using RoomTaskManagement.API.Models.Entities;
using RoomTaskManagement.API.Repositories.Interfaces;

namespace RoomTaskManagement.API.Repositories.Implementations
{
	public class TaskRepository : GenericRepository<TaskEntity>, ITaskRepository
	{
		public TaskRepository(ApplicationDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<TaskEntity>> GetActiveTasksAsync() => await _dbSet.Where(t => t.IsActive).Include(t => t.Creator).ToListAsync();

		public async Task<TaskEntity?> GetTaskWithAssignmentsAsync(int taskId) => await _dbSet.Include(t => t.TaskAssignments).ThenInclude(ta => ta.User).FirstOrDefaultAsync(t => t.Id == taskId);
	}
}
