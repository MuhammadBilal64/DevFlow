using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Common.Models;
using MediatR;

namespace DevFlow.Application.Workspaces.GetWorkspaceMembers
{
    public class GetWorkspaceMembersQuery:PaginationRequest,IRequest<PagedResult<GetWorkspaceMembersResult>>
    {
        public int WorkspaceId {  get; set; }
    }
}
