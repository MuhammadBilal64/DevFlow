using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace DevFlow.Application.Tasks.UpdateTaskAssignee
{
    public class UpdateTaskAssigneeHandler : IRequestHandler<UpdateTaskAssigneeCommand, UpdateTaskAssigneeResult>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IWorkspaceAuthorizationService _workspaceAuthorizationService;
public UpdateTaskAssigneeHandler(IUnitOfWork unitOfWork, ITaskRepository taskRepository, ICurrentUserService currentUserService, IWorkspaceAuthorizationService workspaceAuthorizationService)
        {
            _unitOfWork = unitOfWork;
            _taskRepository = taskRepository;
            _currentUserService = currentUserService;
            _workspaceAuthorizationService = workspaceAuthorizationService;
        }

        public async Task<UpdateTaskAssigneeResult> Handle(UpdateTaskAssigneeCommand request, CancellationToken cancellationToken)
        {
            var userId =  _currentUserService.UserId;
            var task = await _taskRepository.GetByIdForAdminAsync(request.TaskId,userId);
            if (task == null)
            {
                throw new NotFoundException("Task Doesnot Exist");
            }

            


            if (request.NewAssigneeId != null)
            {
                await _workspaceAuthorizationService.EnsureWorkspaceMemberAsync(
                    task.Project.WorkspaceId,
                    request.NewAssigneeId.Value);
                task.Assign(request.NewAssigneeId.Value);

            }
            else
            {
                task.Unassign();
            }

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
