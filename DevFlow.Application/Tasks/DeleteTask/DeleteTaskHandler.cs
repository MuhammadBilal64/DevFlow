using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using MediatR;

namespace DevFlow.Application.Tasks.DeleteTask
{
    public class DeleteTaskHandler : IRequestHandler<DeleteTaskCommand, DeleteTaskResult>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IWorkspaceAuthorizationService _workspaceAuthorizationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public DeleteTaskHandler(ICurrentUserService currentUserService,IUnitOfWork unitOfWork, ITaskRepository taskRepository, IWorkspaceAuthorizationService workspaceAuthorizationService)
        {
            _workspaceAuthorizationService = workspaceAuthorizationService;
            _unitOfWork = unitOfWork;
            _taskRepository = taskRepository;
            _currentUserService=currentUserService;
        }
        public async Task<DeleteTaskResult> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            var userId=_currentUserService.UserId;
            var task = await _taskRepository.GetByIdForAdminAsync(request.TaskId,userId);
            if (task == null)
            {
                throw new NotFoundException("Task Doesnot Exist");
            }
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
