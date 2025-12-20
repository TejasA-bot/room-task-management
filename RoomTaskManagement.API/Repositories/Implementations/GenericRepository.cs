using Microsoft.EntityFrameworkCore;
using RoomTaskManagement.API.Data;
using RoomTaskManagement.API.Repositories.Interfaces;
using System.Linq.Expressions;

namespace RoomTaskManagement.API.Repositories.Implementations
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		protected readonly ApplicationDbContext _context;
		protected readonly DbSet<T> _dbSet;

		public GenericRepository(ApplicationDbContext context)
		{
			_context = context;
			_dbSet = _context.Set<T>();
		}

		public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

		public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

		public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) => await _dbSet.Where(predicate).ToListAsync();

		public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate) => await _dbSet.FirstOrDefaultAsync(predicate);

		public async Task<T> AddAsync(T entity)
		{
			await _dbSet.AddAsync(entity);
			await _context.SaveChangesAsync();
			return entity;
		}

		public async Task UpdateAsync(T entity)
		{
			_dbSet.Update(entity);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(T entity)
		{
			_dbSet.Remove(entity);
			await _context.SaveChangesAsync();
		}

		public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate) => await _dbSet.AnyAsync(predicate);
	}
}
