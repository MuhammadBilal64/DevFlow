using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Models;
using DevFlow.Domain.Enum;
using DevFlow.Domain.Events;
using MediatR;

namespace DevFlow.Application.DomainEvents.TaskCompleted
{
    public class TaskCompletedEventHandler
        : INotificationHandler<TaskCompletedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IWorkflowEngine _workflowEngine;

        public TaskCompletedEventHandler(
            IWorkflowEngine workflowEngine,
            INotificationService notificationService)
        {
            _workflowEngine = workflowEngine;
            _notificationService = notificationService;
        }

        public async Task Handle(
            TaskCompletedEvent notification,
            CancellationToken cancellationToken)
        {
            await _notificationService.NotifyAsync(
                notification.RecipientUserId,
                $"Task '{notification.TaskTitle}' has been completed.",
                NotificationType.TaskCompleted,
                notification.TaskId);

            var values = new Dictionary<string, object?>
            {
                { "ProjectId", notification.ProjectId },
                { "TaskId", notification.TaskId },
                { "TaskTitle", notification.TaskTitle },
                { "AssigneeId", notification.RecipientUserId }
            };

            var context = new WorkflowExecutionContext(values);

            await _workflowEngine.ExecuteAsync(
                WorkflowTrigger.TaskCompleted,
                context);
        }
    }
}