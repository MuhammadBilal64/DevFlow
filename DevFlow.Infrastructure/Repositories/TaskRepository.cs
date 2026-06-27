using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Domain.Entities;
using DevFlow.Infrastructure.Persistence;

namespace DevFlow.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly DevFlowDbContext _context;
        public TaskRepository(DevFlowDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(TaskItem task)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(TaskItem task)
        {
            throw new NotImplementedException();
        }

        public Task<TaskItem?> GetByIdAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<List<TaskItem>> GetTasksByProjectAsync(int Id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TaskItem task)
        {
            throw new NotImplementedException();
        }
    }
}
