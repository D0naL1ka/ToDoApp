using FluentValidation;
using TodoApp.Application.DTOs.Category;

namespace TodoApp.Application.Validators.Category
{
    public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required")
                .MaximumLength(50).WithMessage("Category name max 50 characters");

            RuleFor(x => x.Color)
                .NotEmpty().WithMessage("Color is required")
                .Matches("^#[0-9A-Fa-f]{6}$").WithMessage("Color must be valid hex (e.g. #FF0000)");
        }
    }
}
