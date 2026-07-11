using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Models;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using DevFlow.Domain.Events;
using MediatR;

namespace DevFlow.Application.DomainEvents.TaskAssigned
{
    public class TaskAssignedEventHandler : INotificationHandler<TaskAssignedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IWorkflowEngine _workflowEngine;

        public TaskAssignedEventHandler(
            INotificationService notificationService,
            IWorkflowEngine workflowEngine)
        {
            _notificationService = notificationService;
            _workflowEngine = workflowEngine;
        }

        public async Task Handle(TaskAssignedEvent notification, CancellationToken cancellationToken)
        {

        await   _notificationService.NotifyAsync(
         notification.UserId,
         $"You have been assigned task '{notification.TaskTitle}'.",
         NotificationType.TaskAssigned,
         notification.TaskId);
            var values = new Dictionary<string, object?>
{
    { "TaskId", notification.TaskId },
    { "TaskTitle", notification.TaskTitle },
    { "AssigneeId", notification.UserId }
};
            var context = new WorkflowExecutionContext(values);
            await _workflowEngine.ExecuteAsync(
    WorkflowTrigger.TaskAssigned,
    context
   );

        }
    }
}
