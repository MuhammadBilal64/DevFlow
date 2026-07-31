using MediatR;

namespace DevFlow.Application.ProjectMembers.GetProjectMembers
{
    public class GetProjectMembersQuery : IRequest<List<GetProjectMembersResult>>
    {
        public int ProjectId { get; set; }
    }

}
