using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace DevFlow.Application.Workflows.EnableWorkflow
{
    public class EnableWorkflowCommand:IRequest
    {
        public int WorkflowId {  get; set; }
    }
}
