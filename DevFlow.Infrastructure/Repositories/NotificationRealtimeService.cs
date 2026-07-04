using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Models;
using DevFlow.Domain.Entities;
using DevFlow.Infrastructure.hubs;
using Microsoft.AspNetCore.SignalR;

namespace DevFlow.Infrastructure.Repositories
{
    public class NotificationRealtimeService : INotificationRealtimeService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        public NotificationRealtimeService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task SendNotificationAsync(NotificationRealtimeModel notificationRealtimeModel)
        {
            await _hubContext.Clients.User(notificationRealtimeModel.UserId.ToString()).SendAsync("ReceiveNotification", notificationRealtimeModel);

        }
    }
}
