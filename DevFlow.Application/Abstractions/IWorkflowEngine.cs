using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Common.Models;
using DevFlow.Domain.Enum;

namespace DevFlow.Application.Abstractions
{
    public interface IWorkflowEngine
    {
        Task ExecuteAsync(WorkflowTrigger trigger, WorkflowExecutionContext workflowExecutionContext);
    }
}
