using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using DevFlow.Domain.Events;
using MediatR;

namespace DevFlow.Application.DomainEvents.TaskAssigned
{
    public class TaskAssignedEventHandler : INotificationHandler<TaskAssignedEvent>
    {
        private readonly INotificationService _notificationService;

        public TaskAssignedEventHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Handle(TaskAssignedEvent notification, CancellationToken cancellationToken)
        {

        await   _notificationService.NotifyAsync(
         notification.UserId,
         $"You have been assigned task '{notification.TaskTitle}'.",
         NotificationType.TaskAssigned,
         notification.TaskId);
            

        }
    }
}
