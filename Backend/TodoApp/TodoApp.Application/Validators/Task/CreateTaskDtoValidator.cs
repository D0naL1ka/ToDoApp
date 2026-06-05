using FluentValidation;
using TodoApp.Application.DTOs.Task;

namespace TodoApp.Application.Validators.Task
{
    public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
    {
        public CreateTaskDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title max 200 characters");
        }
    }
}