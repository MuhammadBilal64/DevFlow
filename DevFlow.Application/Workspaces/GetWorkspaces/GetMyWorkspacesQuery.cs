using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Common.Models;
using MediatR;

namespace DevFlow.Application.Workspaces.GetWorkspaces
{
    public class GetMyWorkspacesQuery:PaginationRequest,IRequest<PagedResult<GetMyWorkspacesResult>>
    {

    }
}
