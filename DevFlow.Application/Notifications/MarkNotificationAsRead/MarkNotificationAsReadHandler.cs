using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using MediatR;

namespace DevFlow.Application.Notifications.MarkNotificationAsRead
{
    public class MarkNotificationAsReadHandler : IRequestHandler<MarkNotificationAsReadCommand, MarkNotificationAsReadResult>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MarkNotificationAsReadHandler(
            ICurrentUserService currentUserService,
            INotificationRepository notificationRepository,
            IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<MarkNotificationAsReadResult> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            var notification = await _notificationRepository.GetByIdAsync(request.NotificationId);

            if (notification == null)
                throw new NotFoundException("Notification not found.");

            if (notification.UserId != _currentUserService.UserId)
                throw new ForbiddenException("You are not allowed to access this notification.");

            notification.MarkAsRead();

            await _unitOfWork.SaveChangesAsync();

            return new MarkNotificationAsReadResult
            {
                Message = "Notification marked as read."
            };

        }
    }
}
