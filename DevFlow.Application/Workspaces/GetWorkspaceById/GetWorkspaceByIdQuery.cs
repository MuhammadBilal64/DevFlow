using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace DevFlow.Application.Workspaces.GetWorkspaceById
{
    public class GetWorkspaceByIdQuery:IRequest<GetWorkspaceByIdResult>
    {
        public int WorkspaceId {  get; set; }
    }
}
