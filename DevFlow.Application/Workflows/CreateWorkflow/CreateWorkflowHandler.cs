using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using DevFlow.Domain.Entities;
using MediatR;

namespace DevFlow.Application.Workflows.CreateWorkflow
{
    public class CreateWorkflowHandler : IRequestHandler<CreateWorkflowCommand, CreateWorkflowResult>
    {
        private readonly IWorkflowRepository _workflowRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectAuthorizationService _projectAuthorizationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public CreateWorkflowHandler(
            IWorkflowRepository workflowRepository, IProjectAuthorizationService projectAuthorizationService, IProjectRepository projectRepository,
            IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _workflowRepository = workflowRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _projectRepository = projectRepository;
            _projectAuthorizationService = projectAuthorizationService;
        }

        public async Task<CreateWorkflowResult> Handle(CreateWorkflowCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetByIdAsync(request.ProjectId);
            if (project == null)
                throw new NotFoundException("Project not found.");
            await _projectAuthorizationService
                        .EnsureCanManageProjectAsync(request.ProjectId);
            var exists = await _workflowRepository
                                .ExistsInProjectAsync(request.ProjectId, request.Name);

            if (exists)
            {
                throw new ConflictException(
                    "Workflow with this name already exists.");
            }
            var workflow = new Workflow(
       project,
       _currentUserService.UserId,
       request.Name,
       request.Description,
       request.Trigger);

            foreach (var condition_ in request.Conditions)
            {
                var condition = new WorkflowCondition(
                   condition_.Field,
                   condition_.Operator,
                   condition_.Value);
                workflow.AddCondition(condition);

            }
            foreach (var action_ in request.Actions)
            {
                var action = new WorkflowAction(action_.ActionType, action_.Parameters, action_.Order);
                workflow.AddAction(action);
            }
            await _workflowRepository.AddAsync(workflow);
            await _unitOfWork.SaveChangesAsync();
            return new CreateWorkflowResult
            {
                Id = workflow.Id,
                Name = workflow.Name,
                Trigger = workflow.Trigger
            };
        }
    }
}
