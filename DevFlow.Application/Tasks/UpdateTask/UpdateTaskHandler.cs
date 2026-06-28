using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using MediatR;

namespace DevFlow.Application.Tasks.UpdateTask
{
    public class UpdateTaskHandler : IRequestHandler<UpdateTaskCommand, UpdateTaskResult>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public UpdateTaskHandler(ICurrentUserService currentUserService,IUnitOfWork unitOfWork,ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
            _currentUserService= currentUserService;
        }
        public async Task<UpdateTaskResult> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var task = await _taskRepository.GetByIdForAdminAsync(request.TaskId, userId);
            if (task == null)
            {
                throw new NotFoundException("Task does not Exist");
            }
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
