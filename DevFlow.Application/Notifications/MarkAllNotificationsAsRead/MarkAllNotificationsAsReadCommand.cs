using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace DevFlow.Application.Notifications.MarkAllNotificationsAsRead
{
    public class MarkAllNotificationsAsReadCommand : IRequest<MarkAllNotificationsAsReadResult>
    {
    }
}
