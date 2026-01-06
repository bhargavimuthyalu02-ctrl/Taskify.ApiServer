using FluentValidation;
using Taskify.Api.Controllers;

namespace Taskify.Api.Validators
{
    public class UpdateTaskDtoValidator : AbstractValidator<UpdateTaskDto>
    {
        public UpdateTaskDtoValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            Include(new CreateTaskDtoValidator());
        }
    }
}
