using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace DevFlow.Application.Notifications.MarkNotificationAsRead
{
    public class MarkNotificationAsReadCommand:IRequest<MarkNotificationAsReadResult>
    {
        public int NotificationId { get; set; }
    }
}
