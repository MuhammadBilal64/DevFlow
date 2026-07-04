using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using MediatR;

namespace DevFlow.Application.Notifications.MarkAllNotificationsAsRead
{
    public class MarkAllNotificationsAsReadHandler
    : IRequestHandler<MarkAllNotificationsAsReadCommand, MarkAllNotificationsAsReadResult>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MarkAllNotificationsAsReadHandler(
            ICurrentUserService currentUserService,
            INotificationRepository notificationRepository,
            IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<MarkAllNotificationsAsReadResult> Handle(
            MarkAllNotificationsAsReadCommand request,
            CancellationToken cancellationToken)
        {
            var notifications = await _notificationRepository
                .GetUnreadByUserIdAsync(_currentUserService.UserId);

            foreach (var notification in notifications)
            {
                notification.MarkAsRead();
            }

            await _unitOfWork.SaveChangesAsync();

            return new MarkAllNotificationsAsReadResult
            {
                Message = "All notifications marked as read."
            };
        }
    }
}
