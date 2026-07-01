using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using DevFlow.Domain.Entities;
using MediatR;

namespace DevFlow.Application.Tasks.CreateTask
{
    public class CreateTaskHandler : IRequestHandler<CreateTaskCommand, CreateTaskResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IWorkspaceAuthorizationService _workspaceAuthorizationService;
        public CreateTaskHandler(IWorkspaceAuthorizationService workspaceAuthorizationService,ICurrentUserService currentUserService,IProjectRepository projectRepository,IUnitOfWork unitOfWork, ITaskRepository taskRepository)
        {
            _unitOfWork = unitOfWork;
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _currentUserService= currentUserService;
            _workspaceAuthorizationService= workspaceAuthorizationService;
        }

        public async Task<CreateTaskResult> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            var project =await  _projectRepository.GetByIdAsync(request.ProjectId);
            if (project == null)
            {
                throw new NotFoundException("Project Doesnot Exist");
            }
            var userId =   _currentUserService.UserId;
            await _workspaceAuthorizationService.EnsureWorkspaceMemberAsync(project.WorkspaceId, userId);
            var task = new TaskItem(
     request.Title,
     request.Description,
     project.Id,
     userId,
     request.DueDate,
     request.Priority);
            await _taskRepository.AddAsync(task);
            await _unitOfWork.SaveChangesAsync();
            var result = new CreateTaskResult
            {
                TaskId=task.Id,
                Title= task.Title,
                Description= task.Description,
            };
            return result;
        }
    }
}
