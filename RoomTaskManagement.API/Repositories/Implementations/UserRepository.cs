using Microsoft.EntityFrameworkCore;
using RoomTaskManagement.API.Data;
using RoomTaskManagement.API.Models.Entities;
using RoomTaskManagement.API.Repositories.Interfaces;

namespace RoomTaskManagement.API.Repositories.Implementations
{
	public class UserRepository : GenericRepository<User>, IUserRepository
	{
		public UserRepository(ApplicationDbContext context) : base(context)
		{
		}

		public async Task<User?> GetByUsernameAsync(string username) => await _dbSet.FirstOrDefaultAsync(u => u.Username == username);

		public async Task<IEnumerable<User>> GetActiveUsersAsync() => await _dbSet.Where(u => !"Deleted".Equals(u.Role, StringComparison.InvariantCultureIgnoreCase)).ToListAsync();

		public async Task<IEnumerable<User>> GetAvailableUsersAsync() => await _dbSet.Where(u => !u.IsOutOfStation).ToListAsync();
	}
}
