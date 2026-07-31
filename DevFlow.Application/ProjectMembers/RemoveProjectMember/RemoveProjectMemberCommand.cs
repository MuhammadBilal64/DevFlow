using MediatR;

namespace DevFlow.Application.ProjectMembers.RemoveProjectMember
{
    public class RemoveProjectMemberCommand
         : IRequest<RemoveProjectMemberResult>
    {
        public int ProjectId { get; set; }

        public int UserId { get; set; }
    }
}
