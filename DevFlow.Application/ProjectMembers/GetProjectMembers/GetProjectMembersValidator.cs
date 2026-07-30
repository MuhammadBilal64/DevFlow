using FluentValidation;

namespace DevFlow.Application.ProjectMembers.GetProjectMembers
{
    public class GetProjectMembersValidator : AbstractValidator<GetProjectMembersQuery>
    {
        public GetProjectMembersValidator()
        {
            RuleFor(x => x.ProjectId)
    .GreaterThan(0);
        }

    }
}
