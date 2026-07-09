using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Models;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using DevFlow.Domain.Events;
using MediatR;

namespace DevFlow.Application.DomainEvents.TaskCompleted
{
    public class TaskCompletedEventHandler : INotificationHandler<TaskCompletedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IWorkflowEngine _workflowEngine;
        public TaskCompletedEventHandler(IWorkflowEngine workflowEngine,INotificationService notificationService   )
        {
            _notificationService = notificationService;
            _workflowEngine = workflowEngine;
        }
        public async Task Handle(TaskCompletedEvent notification, CancellationToken cancellationToken)
        {
            await _notificationService.NotifyAsync(
      notification.RecipientUserId,
      $"Task '{notification.TaskTitle}' has been completed.",
      NotificationType.TaskCompleted,
      notification.TaskId);
            var values = new Dictionary<string, object?>
            {
                { "TaskId", notification.TaskId },
        { "TaskTitle", notification.TaskTitle },
        { "AssigneeId", notification.RecipientUserId },
                
            };
            var context = new WorkflowExecutionContext(values);
            await _workflowEngine.ExecuteAsync(
        WorkflowTrigger.TaskCompleted,
        context);

        }
    }
}
