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
        public TaskCompletedEventHandler(INotificationService notificationService   )
        {
            _notificationService = notificationService;
        }
        public async Task Handle(TaskCompletedEvent notification, CancellationToken cancellationToken)
        {
            await _notificationService.NotifyAsync(
      notification.RecipientUserId,
      $"Task '{notification.TaskTitle}' has been completed.",
      NotificationType.TaskCompleted,
      notification.TaskId);

        }
    }
}
