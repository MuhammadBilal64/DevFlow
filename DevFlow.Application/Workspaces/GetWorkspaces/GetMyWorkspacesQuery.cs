using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace DevFlow.Application.Workspaces.GetWorkspaces
{
    public class GetMyWorkspacesQuery:IRequest<List<GetMyWorkspacesResult>>
    {

    }
}
