using RoomTaskManagement.API.Models.Entities;

namespace RoomTaskManagement.API.Repositories.Interfaces
{
	public interface IUserRepository : IGenericRepository<User>
	{
		public Task<User?> GetByUsernameAsync(string username);

		public Task<IEnumerable<User>> GetActiveUsersAsync();

		public Task<IEnumerable<User>> GetAvailableUsersAsync(); // Not out of station.
	}
}
