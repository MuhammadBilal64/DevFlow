using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using MediatR;

namespace DevFlow.Application.Tasks.DeleteTask
{
    public class DeleteTaskHandler : IRequestHandler<DeleteTaskCommand, DeleteTaskResult>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectAuthorizationService _projectAuthorizationService;
        public DeleteTaskHandler(
        IUnitOfWork unitOfWork,
        ITaskRepository taskRepository,
        IProjectAuthorizationService projectAuthorizationService)
        {
            _unitOfWork = unitOfWork;
            _taskRepository = taskRepository;
            _projectAuthorizationService = projectAuthorizationService;
        }
        public async Task<DeleteTaskResult> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(request.TaskId);
            if (task == null)
            {
                throw new NotFoundException("Task Doesnot Exist");
            }
            await _projectAuthorizationService
    .EnsureProjectMemberAsync(task.ProjectId);

            await _taskRepository.DeleteAsync(task);
            await _unitOfWork.SaveChangesAsync();
            var result = new DeleteTaskResult
            {
                TaskId = task.Id,
                Title = task.Title,
            };
            return result;
        }
    }
}
