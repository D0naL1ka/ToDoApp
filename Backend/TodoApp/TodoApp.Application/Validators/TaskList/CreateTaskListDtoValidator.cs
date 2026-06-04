using FluentValidation;
using TodoApp.Application.DTOs.TaskList;

namespace TodoApp.Application.Validators.TaskList
{
    public class CreateTaskListDtoValidator : AbstractValidator<CreateTaskListDto>
    {
        public CreateTaskListDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("List name is required")
                .MaximumLength(100).WithMessage("List name max 100 characters");
        }
    }
}
