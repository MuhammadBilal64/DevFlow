using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using MediatR;

namespace DevFlow.Application.Tasks.UpdateTask
{
    public class UpdateTaskHandler : IRequestHandler<UpdateTaskCommand, UpdateTaskResult>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectAuthorizationService _projectAuthorizationService;
        public UpdateTaskHandler(
      IUnitOfWork unitOfWork,
      ITaskRepository taskRepository,
      IProjectAuthorizationService projectAuthorizationService)
        {
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
            _projectAuthorizationService = projectAuthorizationService;
        }
        public async Task<UpdateTaskResult> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {

            var task = await _taskRepository.GetByIdAsync(request.TaskId);

            if (task == null)
                throw new NotFoundException("Task does not exist.");

            if (task.ProjectId != request.ProjectId)
                throw new NotFoundException("Task does not exist.");

            await _projectAuthorizationService
                .EnsureProjectMemberAsync(request.ProjectId);
            task.UpdateTitle(request.Title);
            task.UpdateDescription(request.Description);
            task.UpdatePriority(request.Priority);
            task.UpdateDueDate(request.DueDate);

            await _taskRepository.UpdateAsync(task);
            await _unitOfWork.SaveChangesAsync();
            var result = new UpdateTaskResult
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
            };
            return result;
        }
    }
}
