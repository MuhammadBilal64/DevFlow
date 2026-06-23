using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace DevFlow.Application.Projects.UpdateProject
{
    public class UpdateProjectCommand:IRequest<UpdateProjectResult>
    {
        public int ProjectId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
