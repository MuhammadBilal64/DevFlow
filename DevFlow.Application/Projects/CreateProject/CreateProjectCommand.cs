using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace DevFlow.Application.Projects.CreateProject
{
    public class CreateProjectCommand:IRequest<CreateProjectResult>
    {
        public string ProjectName { get; set; } = null!;
        public int WorkspaceId {  get; set; }
        public string Description { get; set; } = null!;

    }
}
