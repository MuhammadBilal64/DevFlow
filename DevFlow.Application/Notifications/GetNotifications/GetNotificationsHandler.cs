using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Common.Models;
using DevFlow.Domain.Entities;
using MediatR;

namespace DevFlow.Application.Notifications.GetNotifications
{
    public class GetNotificationsHandler : IRequestHandler<GetNotificationsQuery, PagedResult<GetNotificationsResult>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationRepository _notificationRepository;
        public GetNotificationsHandler(ICurrentUserService currentUserService, INotificationRepository notificationRepository)
        {
            _currentUserService = currentUserService;
            _notificationRepository = notificationRepository;
        }

        public async Task<PagedResult<GetNotificationsResult>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {

             var paginatedNotifications = await _notificationRepository.GetByUserIdAsync(_currentUserService.UserId, request.SearchTerm,
                request.IsRead, request.SortBy, request.Descending, request.PageNumber, request.PageSize
                );
            var totalPages=(int)Math.Ceiling((double) paginatedNotifications.TotalCount/request.PageSize);
            var notifications =  paginatedNotifications.Items.Select(u => new GetNotificationsResult
            {
                Id=u.Id,
                Message=u.Message,
                Type=u.Type,
                IsRead=u.IsRead,
                CreatedAt=u.CreatedAt,
                ReferenceId =u.ReferenceId,

            }).ToList();
            return new PagedResult<GetNotificationsResult>
            {
                Items=notifications,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = paginatedNotifications.TotalCount,
                TotalPages = totalPages


            };
  

        }
    }
}
