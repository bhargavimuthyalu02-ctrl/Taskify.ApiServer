using FluentValidation;
using Taskify.Api.Controllers;

namespace Taskify.Api.Validators
{
    public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
    {
        public CreateTaskDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Description).MaximumLength(2000);
            RuleFor(x => x.OwnerEmail).NotEmpty().EmailAddress().MaximumLength(200);
            RuleFor(x => x.DueDate).GreaterThanOrEqualTo(DateTime.UtcNow.Date);
        }
    }
}
