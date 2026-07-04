using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace DevFlow.Application.Notifications.GetUnreadNotificationCount
{
    public class GetUnreadNotificationCountQuery
    : IRequest<GetUnreadNotificationCountResult>
    {
    }
}
