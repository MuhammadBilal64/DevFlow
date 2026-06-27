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
            if (request.AssignedToUserId != null)
            {


                await _workspaceAuthorizationService
                       .EnsureWorkspaceMemberAsync(
                           project.WorkspaceId,
                           request.AssignedToUserId.Value);

            }

            var task= new TaskItem
            {
                Title = request.Title,
                Description = request.Description,
                ProjectId = project.Id,

                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,

                AssignedToUserId = request.AssignedToUserId,

                AssignedAt = request.AssignedToUserId != null
    ? DateTime.UtcNow
    : null,
                DueDate = request.DueDate,

                Priority = request.Priority,

                Status = Domain.Enum.TaskStatus.Todo,
       
            };
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
