using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace DevFlow.Application.Workflows.GetWorkflowById
{
    public class GetWorkflowByIdQuery:IRequest<GetWorkflowByIdResult>
    {
        public int WorkflowId {  get; set; }
    }
}
