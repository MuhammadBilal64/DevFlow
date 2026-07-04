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
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationRealtimeService _notificationRealtimeService;
        public TaskCompletedEventHandler(IUnitOfWork unitOfWork,INotificationRepository notificationRepository, INotificationRealtimeService notificationRealtimeService)
        {
            _unitOfWork = unitOfWork;
            _notificationRepository = notificationRepository;
            _notificationRealtimeService = notificationRealtimeService;
        }
        public async Task Handle(TaskCompletedEvent notification, CancellationToken cancellationToken)
        {
            var notification_ = new Notification(notification.RecipientUserId, $"Task '{notification.TaskTitle}' has been completed.",
        NotificationType.TaskCompleted,
        notification.TaskId);
            await _notificationRepository.AddAsync(notification_);
            await _unitOfWork.SaveChangesAsync();
            var realtime = new NotificationRealtimeModel
            {
                UserId = notification_.UserId,
                Message = notification_.Message,
                Type = notification_.Type,
                ReferenceId = notification_.ReferenceId,
                CreatedAt = notification_.CreatedAt
            };
            await _notificationRealtimeService.SendNotificationAsync(realtime);

        }
    }
}
