using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Common.Models;
using MediatR;

namespace DevFlow.Application.Projects.GetProjectsByWorkspace
{
    public class GetProjectsByWorkspaceQuery:PaginationRequest,IRequest<PagedResult<GetProjectsByWorkspaceResult>>
    {
        public int WorkspaceId { get; set; }
    }
}
