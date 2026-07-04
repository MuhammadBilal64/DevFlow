using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Common.Models;
using MediatR;

namespace DevFlow.Application.Notifications.GetNotifications
{
    public class GetNotificationsQuery:PaginationRequest,IRequest<PagedResult<GetNotificationsResult>>
    {
        public bool ?IsRead {  get; set; }
    }
}
