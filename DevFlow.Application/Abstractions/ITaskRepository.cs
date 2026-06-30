using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using DevFlow.Application.Common.Models;
using DevFlow.Domain.Entities;

namespace DevFlow.Application.Abstractions
{
    public interface ITaskRepository
    {
        Task AddAsync(TaskItem task);
        Task<TaskItem?> GetByIdAsync(int TaskId);
        Task<PaginatedData<TaskItem>> GetTasksByProjectAsync(
     int projectId,
     string? searchTerm, string? sortBy,
    bool descending,
     int pageNumber,
     int pageSize);
        Task UpdateAsync(TaskItem task);
        Task DeleteAsync(TaskItem task);
        Task<TaskItem?> GetByIdForAdminAsync(int taskId,int currentUserId);
        Task<TaskItem?> GetByIdForStatusUpdateAsync(int taskId, int currentUserId);
    }
}
