using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Common.Models;
using DevFlow.Domain.Entities;

namespace DevFlow.Application.Workflows.ActionExecution
{
    public interface IActionDispatcher
    {
        Task ExecuteAsync(WorkflowAction action, WorkflowExecutionContext workflowExecutionContext);
    }
}
