using FluentValidation;
using TodoApp.Application.DTOs.Task;

namespace TodoApp.Application.Validators.Task
{
    public class UpdateTaskDtoValidator : AbstractValidator<UpdateTaskDto>
    {
        public UpdateTaskDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title max 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description max 1000 characters");

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future")
                .When(x => x.DueDate.HasValue);
        }
    }
}
