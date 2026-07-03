using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using MediatR;

namespace DevFlow.Application.Notifications.GetUnreadNotificationCount
{
    public class GetUnreadNotificationCountHandler
    : IRequestHandler<GetUnreadNotificationCountQuery,
        GetUnreadNotificationCountResult>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationRepository _notificationRepository;

        public GetUnreadNotificationCountHandler(
            ICurrentUserService currentUserService,
            INotificationRepository notificationRepository)
        {
            _currentUserService = currentUserService;
            _notificationRepository = notificationRepository;
        }

        public async Task<GetUnreadNotificationCountResult> Handle(
            GetUnreadNotificationCountQuery request,
            CancellationToken cancellationToken)
        {
            var count = await _notificationRepository
                .GetUnreadCountAsync(_currentUserService.UserId);

            return new GetUnreadNotificationCountResult
            {
                UnreadCount = count
            };
        }
    }
}
