using AutoMapper;
using TodoApp.Application.DTOs.Category;
using TodoApp.Application.DTOs.Task;
using TodoApp.Application.DTOs.TaskList;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Mappings
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            // Task
            CreateMap<TaskItem, TaskDto>()
                .ForMember(dest => dest.TaskListName,
                    opt => opt.MapFrom(src => src.TaskList != null ? src.TaskList.Name : null))
                .ForMember(dest => dest.Categories,
                    opt => opt.MapFrom(src => src.TaskCategories
                        .Select(tc => tc.Category)));

            // TaskList
            CreateMap<TaskList, TaskListDto>()
                .ForMember(dest => dest.TaskCount,
                    opt => opt.MapFrom(src => src.Tasks.Count));

            // Category
            CreateMap<Category, CategoryDto>();
        }
    }
}
