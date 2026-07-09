using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Common.Models;
using DevFlow.Domain.Enum;
using MediatR;

namespace DevFlow.Application.Workflows.GetAllWorkflows
{
    public class GetAllWorkflowsQuery:PaginationRequest,IRequest<PagedResult<GetAllWorkflowsResult>>
    {
        public WorkflowTrigger? Trigger { get; set; }

        public bool? IsEnabled { get; set; }
    }
}
