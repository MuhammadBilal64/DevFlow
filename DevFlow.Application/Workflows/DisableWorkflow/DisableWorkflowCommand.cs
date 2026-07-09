using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace DevFlow.Application.Workflows.DisableWorkflow
{
    public class DisableWorkflowCommand : IRequest
    {
        public int WorkflowId { get; set; }
    }
}
