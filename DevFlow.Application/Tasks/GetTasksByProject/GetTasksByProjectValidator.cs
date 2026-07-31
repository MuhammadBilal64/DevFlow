using DevFlow.Application.Common.Validators;
using FluentValidation;

namespace DevFlow.Application.Tasks.GetTasksByProject
{
    public class GetTasksByProjectValidator
     : AbstractValidator<GetTasksByProjectQuery>
    {
        public GetTasksByProjectValidator()
        {
            RuleFor(x => x.ProjectId)
      .GreaterThan(0);
            Include(new PaginationRequestValidator());
        }
    }
}
