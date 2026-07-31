using DevFlow.Application.Common.Validators;
using FluentValidation;

namespace DevFlow.Application.Projects.GetMyProjectsByWorkspace
{
    public class GetMyProjectsByWorkspaceValidator
        : AbstractValidator<GetMyProjectsByWorkspaceQuery>
    {
        public GetMyProjectsByWorkspaceValidator()
        {
            Include(new PaginationRequestValidator());

            RuleFor(x => x.WorkspaceId)
                .GreaterThan(0);
        }
    }
}