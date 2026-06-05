using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces.Repositories;
using TodoApp.Infrastructure.Data;

namespace TodoApp.Infrastructure.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Category>> GetAvailableAsync(int userId)
            => await _context.Categories
                .Where(c => c.IsSystem || c.UserId == userId)
                .OrderBy(c => c.IsSystem ? 0 : 1)
                .ThenBy(c => c.Name)
                .ToListAsync();

        public async Task<IEnumerable<Category>> GetSystemAsync()
            => await _context.Categories
                .Where(c => c.IsSystem)
                .ToListAsync();

        public async Task<IEnumerable<Category>> GetByUserIdAsync(int userId)
            => await _context.Categories
                .Where(c => !c.IsSystem && c.UserId == userId)
                .ToListAsync();
    }
}
