using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace DevFlow.Application.Workspaces.CreateWorkspace
{
    public class CreateWorkspaceCommand:IRequest<CreateWorkspaceResult>
    {
        public string Name { get; set; } = null!;
    }
}
