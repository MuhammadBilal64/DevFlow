using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace DevFlow.Application.Projects.GetProjectsByWorkspace
{
    public class GetProjectsByWorkspaceQuery:IRequest<List<GetProjectsByWorkspaceResult>>
    {
        public int WorkspaceId { get; set; }
    }
}
