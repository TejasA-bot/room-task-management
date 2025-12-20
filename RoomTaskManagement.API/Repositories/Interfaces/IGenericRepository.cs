using System.Linq.Expressions;

namespace RoomTaskManagement.API.Repositories.Interfaces
{
	/// <summary>Generic repository interface for common data operations.</summary>
	public interface IGenericRepository<T> where T : class
	{
		public Task<T?> GetByIdAsync(int id);
		public Task<IEnumerable<T>> GetAllAsync();
		public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
		public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
		public Task<T> AddAsync(T entity);
		public Task UpdateAsync(T entity);
		public Task DeleteAsync(T entity);
		public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
	}
}
