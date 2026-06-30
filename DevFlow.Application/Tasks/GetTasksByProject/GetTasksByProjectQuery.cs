using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Common.Models;
using DevFlow.Domain.Enum;
using MediatR;
using TaskStatus = DevFlow.Domain.Enum.TaskStatus;

namespace DevFlow.Application.Tasks.GetTasksByProject
{
    public class GetTasksByProjectQuery:PaginationRequest,IRequest<PagedResult<GetTasksByProjectResult>>
    {
        public int ProjectId { get; set; }
        
        public TaskStatus? Status { get; set; }

        public TaskPriority? Priority { get; set; }
    }
}
