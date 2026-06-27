using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace DevFlow.Application.Tasks.UpdateTaskAssignee
{
    public class UpdateTaskAssigneeHandler : IRequestHandler<UpdateTaskAssigneeCommand, UpdateTaskAssigneeResult>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IWorkspaceAuthorizationService _workspaceAuthorizationService;
        private readonly IUnitOfWork _unitOfWork;
public UpdateTaskAssigneeHandler(IUnitOfWork unitOfWork, ITaskRepository taskRepository, IWorkspaceAuthorizationService workspaceAuthorizationService)
        {
            _workspaceAuthorizationService = workspaceAuthorizationService;
            _unitOfWork = unitOfWork;
            _taskRepository = taskRepository;

        }

        public async Task<UpdateTaskAssigneeResult> Handle(UpdateTaskAssigneeCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(request.TaskId);
            if (task == null)
            {
                throw new NotFoundException("Task Doesnot Exist");
            }

            if (task.AssignedToUserId == request.NewAssigneeId)
            {
                return new UpdateTaskAssigneeResult
                {
                    Id = task.Id,
                    Title = task.Title
                };
            }
            await _workspaceAuthorizationService.EnsureAdminOrOwnerAsync(task.Project.WorkspaceId);
         
          
                await _workspaceAuthorizationService.EnsureWorkspaceMemberAsync(task.Project.WorkspaceId, request.NewAssigneeId);

            task.AssignedToUserId = request.NewAssigneeId;
            task.AssignedAt = DateTime.UtcNow;
            await _taskRepository.UpdateAsync(task);
            await _unitOfWork.SaveChangesAsync();
            var result = new UpdateTaskAssigneeResult
            {
                Id=task.Id,
                Title=task.Title,
            };
            return result;
        }
    }
}
