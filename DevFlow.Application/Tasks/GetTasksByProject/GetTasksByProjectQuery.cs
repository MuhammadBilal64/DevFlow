using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace DevFlow.Application.Tasks.GetTasksByProject
{
    public class GetTasksByProjectQuery:IRequest<List<GetTasksByProjectResult>>
    {
        public int ProjectId { get; set; }
    }
}
