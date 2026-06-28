using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using DevFlow.Domain.Entities;

namespace DevFlow.Application.Abstractions
{
    public interface ITaskRepository
    {
        Task AddAsync(TaskItem task);
        Task<TaskItem?> GetByIdAsync(int TaskId);
        Task<List<TaskItem>> GetTasksByProjectAsync(int Id);
        Task UpdateAsync(TaskItem task);
        Task DeleteAsync(TaskItem task);
        Task<TaskItem?> GetAuthorizedForAdminAsync(int taskId,int currentUserId);
    }
}
