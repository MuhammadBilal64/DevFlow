using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using MediatR;

namespace DevFlow.Application.Tasks.DeleteTask
{
    public class DeleteTaskHandler : IRequestHandler<DeleteTaskCommand, DeleteTaskResult>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IWorkspaceAuthorizationService _workspaceAuthorizationService;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteTaskHandler(IUnitOfWork unitOfWork, ITaskRepository taskRepository, IWorkspaceAuthorizationService workspaceAuthorizationService)
        {
            _workspaceAuthorizationService = workspaceAuthorizationService;
            _unitOfWork = unitOfWork;
            _taskRepository = taskRepository;
        }
        public async Task<DeleteTaskResult> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(request.TaskId);
            if (task == null)
            {
                throw new NotFoundException("Task Doesnot Exist");
            }
            await _workspaceAuthorizationService.EnsureAdminOrOwnerAsync(task.Project.WorkspaceId);
            await _taskRepository.DeleteAsync(task);
            await _unitOfWork.SaveChangesAsync();
            var result = new DeleteTaskResult
            {
                TaskId=task.Id,
                Title=task.Title,
            };
            return result;
        }
    }
}
