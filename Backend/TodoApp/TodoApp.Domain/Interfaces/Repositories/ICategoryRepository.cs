using TodoApp.Domain.Entities;

namespace TodoApp.Domain.Interfaces.Repositories
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<IEnumerable<Category>> GetAvailableAsync(int userId); // системні + особисті
        Task<IEnumerable<Category>> GetSystemAsync();              // тільки системні
        Task<IEnumerable<Category>> GetByUserIdAsync(int userId);  // тільки особисті
    }
}
