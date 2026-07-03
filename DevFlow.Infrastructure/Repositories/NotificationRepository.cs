using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Models;
using DevFlow.Domain.Entities;
using DevFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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

        public Task<Notification?> GetByIdAsync(int notificationId)
        {
            throw new NotImplementedException();
        }

        public async Task<PaginatedData<Notification>> GetByUserIdAsync(int userId, string? searchTerm, bool? isRead, string? sortBy, bool descending, int pageNumber, int pageSize)
        {
            var query = _context.Notifications.AsNoTracking().Where(u => u.UserId == userId);
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query=query.Where(p=>p.Message.Contains(searchTerm));
            }
            if (isRead.HasValue)
            {
                query = query.Where(n => n.IsRead == isRead.Value);
            }
            var sortingFields = new Dictionary<string, Expression<Func<Notification, object>>>
            {
                {"createdat",p=>p.CreatedAt },{"isread",p=>p.IsRead}
            };
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                if (sortingFields.TryGetValue(sortBy.ToLower(), out var expression))
                {
                    query = descending
                        ? query.OrderByDescending(expression)
                        : query.OrderBy(expression);
                }
                else
                {
                    query = query.OrderByDescending(p => p.CreatedAt);
                }
            }
            else
            {
                query = query.OrderByDescending(p => p.CreatedAt);
            }
            var totalCount = await query.CountAsync();
            var items= await query.Skip((pageNumber-1)*pageSize).Take(pageSize).ToListAsync();
            var result = new PaginatedData<Notification>
            {
                Items=items,
                TotalCount=totalCount
            };
            return result;
        }

        public Task<List<Notification>> GetUnreadByUserIdAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetUnreadCountAsync(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
