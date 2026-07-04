using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Common.Models;

namespace DevFlow.Application.Abstractions
{
    public interface INotificationRealtimeService
    {
        Task SendNotificationAsync(NotificationRealtimeModel notificationRealtimeModel);
    }
}
