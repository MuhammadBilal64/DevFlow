using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Common.Models;
using DevFlow.Domain.Entities;

namespace DevFlow.Application.Abstractions
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task<PaginatedData<Notification>> GetByUserIdAsync(int userId,
    string? searchTerm,
    bool? isRead,
    string? sortBy,
    bool descending,
    int pageNumber,
    int pageSize);

        Task<Notification?> GetByIdAsync(int notificationId);

        Task<List<Notification>> GetUnreadByUserIdAsync(int userId);

        Task<int> GetUnreadCountAsync(int userId);
    }
}
