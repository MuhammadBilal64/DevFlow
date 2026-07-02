using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Domain.Entities;
using DevFlow.Infrastructure.Persistence;

namespace DevFlow.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly DevFlowDbContext _context;

        public NotificationRepository(DevFlowDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
        }
    }
}
