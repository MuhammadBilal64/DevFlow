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
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationRepository _notificationRepository;

        public TaskAssignedEventHandler(IUnitOfWork unitOfWork, INotificationRepository notificationRepository)
        {
            _unitOfWork = unitOfWork;
            _notificationRepository = notificationRepository;
        }

        public async Task Handle(TaskAssignedEvent notification, CancellationToken cancellationToken)
        {

            var notificationEntity = new Notification(
         notification.UserId,
         $"You have been assigned task '{notification.TaskTitle}'.",
         NotificationType.TaskAssigned,
         notification.TaskId);
            await _notificationRepository.AddAsync(notificationEntity);
            await _unitOfWork.SaveChangesAsync();

        }
    }
}
