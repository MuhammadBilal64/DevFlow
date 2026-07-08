using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Models;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;

namespace DevFlow.Infrastructure.Repositories
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationRealtimeService _notificationRealtimeService;
        public NotificationService(IUnitOfWork unitofWork,INotificationRepository notificationRepository,INotificationRealtimeService notificationRealtimeService)
        {
            _notificationRepository = notificationRepository;
            _notificationRealtimeService = notificationRealtimeService;
            _unitOfWork= unitofWork;

        }
        public async Task NotifyAsync(int userId, string message, NotificationType type, int? referenceId = null)
        {
            var notification = new Notification(
       userId,
       message,
       type,
       referenceId);

            await _notificationRepository.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
            var realtime = new NotificationRealtimeModel
            {
                UserId = notification.UserId,
                Message = notification.Message,
                Type = notification.Type,
                ReferenceId = notification.ReferenceId,
                CreatedAt = notification.CreatedAt
            };
            await _notificationRealtimeService.SendNotificationAsync(realtime);

        }
    }
}
