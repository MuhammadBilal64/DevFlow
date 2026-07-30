using DevFlow.Application.Common.Models;
using MediatR;

namespace DevFlow.Application.Projects.GetMyProjectsByWorkspace
{
    public class GetMyProjectsByWorkspaceQuery
        : PaginationRequest, IRequest<PagedResult<GetMyProjectsByWorkspaceResult>>
    {
        public int WorkspaceId { get; set; }
    }
}