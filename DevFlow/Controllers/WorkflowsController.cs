using DevFlow.Api.Contracts.Responses;
using DevFlow.Application.Common.Models;
using DevFlow.Application.Workflows.CreateWorkflow;
using DevFlow.Application.Workflows.DisableWorkflow;
using DevFlow.Application.Workflows.EnableWorkflow;
using DevFlow.Application.Workflows.GetAllWorkflows;
using DevFlow.Application.Workflows.GetWorkflowById;
using DevFlow.Application.Workflows.UpdateWorkflow;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevFlow.Api.Controllers
{
    [Route("api/workflows")]
    [ApiController]
    [Authorize]
    public class WorkflowController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WorkflowController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorkflow(CreateWorkflowCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(ApiResponse<CreateWorkflowResult>.Ok(
                result,
                "Workflow created successfully"));
        }

        [HttpGet("{workflowId}")]
        public async Task<IActionResult> GetWorkflowById([FromRoute] int workflowId)
        {
            var result = await _mediator.Send(new GetWorkflowByIdQuery
            {
                WorkflowId = workflowId
            });

            return Ok(ApiResponse<GetWorkflowByIdResult>.Ok(
                result,
                "Retrieved successfully"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWorkflows([FromQuery] GetAllWorkflowsQuery query)
        {
            var result = await _mediator.Send(query);

            return Ok(ApiResponse<PagedResult<GetAllWorkflowsResult>>.Ok(
                result,
                "Retrieved successfully"));
        }

        [HttpPut("{workflowId}")]
        public async Task<IActionResult> UpdateWorkflow(
            [FromRoute] int workflowId,
            UpdateWorkflowCommand command)
        {
            command.WorkflowId = workflowId;

            await _mediator.Send(command);

            return Ok(ApiResponse<object>.Ok(
                null,
                "Workflow updated successfully"));
        }

        [HttpPatch("{workflowId}/enable")]
        public async Task<IActionResult> EnableWorkflow([FromRoute] int workflowId)
        {
            await _mediator.Send(new EnableWorkflowCommand
            {
                WorkflowId = workflowId
            });

            return Ok(ApiResponse<object>.Ok(
                null,
                "Workflow enabled successfully"));
        }

        [HttpPatch("{workflowId}/disable")]
        public async Task<IActionResult> DisableWorkflow([FromRoute] int workflowId)
        {
            await _mediator.Send(new DisableWorkflowCommand
            {
                WorkflowId = workflowId
            });

            return Ok(ApiResponse<object>.Ok(
                null,
                "Workflow disabled successfully"));
        }
    }
}