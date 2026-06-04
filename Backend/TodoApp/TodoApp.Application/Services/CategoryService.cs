using AutoMapper;
using TodoApp.Application.DTOs.Category;
using TodoApp.Application.Interfaces.Services;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces.Repositories;

namespace TodoApp.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetAvailableAsync(int userId)
        {
            var categories = await _categoryRepository.GetAvailableAsync(userId);
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto> CreateAsync(int userId, CreateCategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                Color = dto.Color,
                IsSystem = false,
                UserId = userId
            };
            var created = await _categoryRepository.CreateAsync(category);
            return _mapper.Map<CategoryDto>(created);
        }

        public async Task<CategoryDto> UpdateAsync(int id, int userId, UpdateCategoryDto dto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null || category.IsSystem || category.UserId != userId)
                throw new Exception("Category not found or cannot be edited");
            category.Name = dto.Name;
            category.Color = dto.Color;
            var updated = await _categoryRepository.UpdateAsync(category);
            return _mapper.Map<CategoryDto>(updated);
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null || category.IsSystem || category.UserId != userId)
                throw new Exception("Category not found or cannot be deleted");
            await _categoryRepository.DeleteAsync(id);
        }
    }
}
