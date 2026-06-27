using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using MediatR;

namespace DevFlow.Application.Tasks.UpdateTask
{
    public class UpdateTaskHandler : IRequestHandler<UpdateTaskCommand, UpdateTaskResult>
    {
        private readonly IWorkspaceAuthorizationService _authorizationService;
        private readonly ITaskRepository _taskRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateTaskHandler(IUnitOfWork unitOfWork,ITaskRepository taskRepository,IWorkspaceAuthorizationService workspaceAuthorizationService)
        {
            _taskRepository = taskRepository;
           _authorizationService = workspaceAuthorizationService;
            _unitOfWork = unitOfWork;
        }
        public async Task<UpdateTaskResult> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(request.TaskId);
            if (task == null)
            {
                throw new NotFoundException("Task does not Exist");
            }
            await _authorizationService.EnsureAdminOrOwnerAsync(task.Project.WorkspaceId);
            task.Title = request.Title;
            task.Description= request.Description;
            task.DueDate = request.DueDate;
            task.Priority= request.Priority;

            await _taskRepository.UpdateAsync(task);
            await _unitOfWork.SaveChangesAsync();
            var result = new UpdateTaskResult
            {
                Id=task.Id,
                Title=task.Title,
                Description=task.Description,
            };
            return result;
        }
    }
}
