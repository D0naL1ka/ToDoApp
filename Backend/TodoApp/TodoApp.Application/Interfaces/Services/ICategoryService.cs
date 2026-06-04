using TodoApp.Application.DTOs.Category;

namespace TodoApp.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAvailableAsync(int userId);
        Task<CategoryDto> CreateAsync(int userId, CreateCategoryDto dto);
        Task<CategoryDto> UpdateAsync(int id, int userId, UpdateCategoryDto dto);
        Task DeleteAsync(int id, int userId);
    }
}
