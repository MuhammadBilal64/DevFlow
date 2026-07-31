using DevFlow.Api.Contracts.Responses;
using DevFlow.Application.Common.Models;
using DevFlow.Application.Tasks.CreateTask;
using DevFlow.Application.Tasks.DeleteTask;
using DevFlow.Application.Tasks.GetTaskById;
using DevFlow.Application.Tasks.GetTasksByProject;
using DevFlow.Application.Tasks.UpdateTask;
using DevFlow.Application.Tasks.UpdateTaskAssignee;
using DevFlow.Application.Tasks.UpdateTaskStatus;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevFlow.Api.Controllers
{
    [Route("api/projects/{projectId}/tasks")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TaskController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(
            [FromRoute] int projectId,
            CreateTaskCommand command)
        {
            command.ProjectId = projectId;

            var result = await _mediator.Send(command);

            return Ok(ApiResponse<CreateTaskResult>.Ok(
                result,
                "Task Created Successfully"));
        }

        [HttpGet]
        public async Task<IActionResult> GetTasksByProject(
            [FromRoute] int projectId,
            [FromQuery] GetTasksByProjectQuery query)
        {
            query.ProjectId = projectId;

            var result = await _mediator.Send(query);

            return Ok(ApiResponse<PagedResult<GetTasksByProjectResult>>.Ok(
                result,
                "Retrieved Successfully"));
        }

        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTaskById(
            [FromRoute] int projectId,
            [FromRoute] int taskId)
        {
            var result = await _mediator.Send(new GetTaskByIdQuery
            {
                ProjectId = projectId,
                TaskId = taskId
            });

            return Ok(ApiResponse<GetTaskByIdResult>.Ok(
                result,
                "Retrieved Successfully"));
        }

        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateTask(
            [FromRoute] int projectId,
            [FromRoute] int taskId,
            UpdateTaskCommand command)
        {
            command.ProjectId = projectId;
            command.TaskId = taskId;

            var result = await _mediator.Send(command);

            return Ok(ApiResponse<UpdateTaskResult>.Ok(
                result,
                "Task Updated Successfully"));
        }

        [HttpPatch("{taskId}/status")]
        public async Task<IActionResult> UpdateTaskStatus(
            [FromRoute] int projectId,
            [FromRoute] int taskId,
            UpdateTaskStatusCommand command)
        {
            command.ProjectId = projectId;
            command.TaskId = taskId;

            var result = await _mediator.Send(command);

            return Ok(ApiResponse<UpdateTaskStatusResult>.Ok(
                result,
                "Status Updated Successfully"));
        }

        [HttpPatch("{taskId}/assignee")]
        public async Task<IActionResult> UpdateTaskAssignee(
            [FromRoute] int projectId,
            [FromRoute] int taskId,
            UpdateTaskAssigneeCommand command)
        {
            command.ProjectId = projectId;
            command.TaskId = taskId;

            var result = await _mediator.Send(command);

            return Ok(ApiResponse<UpdateTaskAssigneeResult>.Ok(
                result,
                "Assignee Updated Successfully"));
        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(
            [FromRoute] int projectId,
            [FromRoute] int taskId)
        {
            var command = new DeleteTaskCommand
            {
                ProjectId = projectId,
                TaskId = taskId
            };

            var result = await _mediator.Send(command);

            return Ok(ApiResponse<DeleteTaskResult>.Ok(
                result,
                "Task Deleted Successfully"));
        }
    }
}