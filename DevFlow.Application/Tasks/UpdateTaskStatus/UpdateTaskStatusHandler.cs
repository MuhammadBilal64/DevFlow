using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using MediatR;

namespace DevFlow.Application.Tasks.UpdateTaskStatus
{
    public class UpdateTaskStatusHandler : IRequestHandler<UpdateTaskStatusCommand, UpdateTaskStatusResult>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWorkspaceAuthorizationService _workspaceAuthorizationService;
        public UpdateTaskStatusHandler(ICurrentUserService currentUserService,ITaskRepository taskRepository, IUnitOfWork unitOfWork, IWorkspaceAuthorizationService workspaceAuthorizationService)
        {
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _workspaceAuthorizationService = workspaceAuthorizationService;
        }

        public  async Task<UpdateTaskStatusResult> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdForStatusUpdateAsync(request.TaskId,_currentUserService.UserId);
            if (task == null)
            {
                throw new NotFoundException("Task Doesnot Exist");
            }
            if (task.Status != request.TaskStatus)
            {
                if (request.TaskStatus == DevFlow.Domain.Enum.TaskStatus.Completed)
                {
                    if (task.CompletedAt == null)
                    {
                        task.CompletedAt = DateTime.UtcNow;
                    }
                }
                else
                {
                    task.CompletedAt = null;
                }
            }
            task.Status = (Domain.Enum.TaskStatus)request.TaskStatus;
            await _taskRepository.UpdateAsync(task);
            await _unitOfWork.SaveChangesAsync();
            var result = new UpdateTaskStatusResult
            {
                Id=task.Id,
                Status=task.Status
            };
            return result;
        }
    }
}
