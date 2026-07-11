using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Common.Models;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;

namespace DevFlow.Application.Workflows.ActionExecution
{
    public interface IActionExecutor
    {
         WorkflowActionType Type { get; }
        Task ExecuteAsync(WorkflowAction action,WorkflowExecutionContext executionContext);
        
    }
}
