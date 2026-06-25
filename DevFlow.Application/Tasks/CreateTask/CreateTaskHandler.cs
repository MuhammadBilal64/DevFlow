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
        private readonly IWorkspaceMemberRepository _workspaceMemberRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ICurrentUserService _currentUserService;
        public CreateTaskHandler(ICurrentUserService currentUserService,IProjectRepository projectRepository,IUnitOfWork unitOfWork, IWorkspaceMemberRepository workspaceMemberRepository, ITaskRepository taskRepository)
        {
            _unitOfWork = unitOfWork;
            _workspaceMemberRepository = workspaceMemberRepository;
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _currentUserService= currentUserService;
        }

        public async Task<CreateTaskResult> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            var project =await  _projectRepository.GetByIdAsync(request.ProjectId);
            if (project == null)
            {
                throw new NotFoundException("Project Doesnot Exist");
            }
            var userId =   _currentUserService.UserId;
            var membership = await _workspaceMemberRepository.GetMemberAsync(userId, project.WorkspaceId);
            if (membership == null)
            {
                throw new ForbiddenException("You are not allowed to Add");
            }
            if (request.AssignedToUserId != null)
            {
                var member = await _workspaceMemberRepository.GetMemberAsync((int)request.AssignedToUserId, project.WorkspaceId);
                if (member == null)
                {
                    throw new ForbiddenException(
                        "User is not a member of the workspace");
                }
               



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
