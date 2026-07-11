using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Domain.Entities;
using MediatR;

namespace DevFlow.Application.Workflows.CreateWorkflow
{
    public class CreateWorkflowHandler : IRequestHandler<CreateWorkflowCommand, CreateWorkflowResult>
    {
        private readonly IWorkflowRepository _workflowRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateWorkflowHandler(
            IWorkflowRepository workflowRepository,
            IUnitOfWork unitOfWork)
        {
            _workflowRepository = workflowRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateWorkflowResult> Handle(CreateWorkflowCommand request, CancellationToken cancellationToken)
        {
            var workflow = new Workflow(
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
            foreach(var action_ in request.Actions)
            {
                var action = new WorkflowAction(action_.ActionType,action_.Parameters,action_.Order);
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
