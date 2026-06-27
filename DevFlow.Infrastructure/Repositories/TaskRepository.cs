using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Domain.Entities;
using DevFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DevFlow.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly DevFlowDbContext _context;
        public TaskRepository(DevFlowDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TaskItem task)
        {
            await _context.Tasks.AddAsync(task);
        }

        public  Task DeleteAsync(TaskItem task)
        {
            _context.Tasks.Remove(task);
            return Task.CompletedTask;
        }

        public async Task<TaskItem?> GetByIdAsync(int TaskId)
        {
            return await _context.Tasks.Include(p => p.Project).FirstOrDefaultAsync(i=>i.Id==TaskId);
                }

        public async Task<List<TaskItem>> GetTasksByProjectAsync(int Id)
        {
            return await _context.Tasks.Where(t=>t.ProjectId==Id).ToListAsync();
        }

        public async Task UpdateAsync(TaskItem task)
        {
            _context.Tasks.Update(task);
              
        }
    }
}
