using DevFlow.Api.Contracts.Responses;
using DevFlow.Application.Tasks.CreateTask;
using DevFlow.Application.Tasks.GetTaskById;
using DevFlow.Application.Tasks.GetTasksByProject;
using DevFlow.Application.Tasks.UpdateTask;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DevFlow.Api.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TaskController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult>CreateTask(CreateTaskCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<CreateTaskResult>.Ok(result, "Task Created Successfully"));

        }
        [HttpGet("{taskId}")]
        [Authorize]
        public async Task<IActionResult> GetTaskById(int taskId)
        {
            var result = await _mediator.Send(new GetTaskByIdQuery
            {
                TaskId = taskId
            });
            return Ok(ApiResponse<GetTaskByIdResult>.Ok(result, "Retrieved Successfully"));

        }
        [HttpGet("project/{projectId}")]
        [Authorize]
        public async Task<IActionResult> GetTasksByProject(int projectId)
        {
            var result = await _mediator.Send(new GetTasksByProjectQuery
            {
                ProjectId = projectId,

            });
            return Ok(ApiResponse<List<GetTasksByProjectResult>>.Ok(result, "Retrieved Successfully"));
        }
        [HttpPut("{taskId}")]
        [Authorize]
        public async Task<IActionResult> UpdateTask([FromRoute]int taskId,UpdateTaskCommand command)
        {
            command.TaskId = taskId;
            var result =await _mediator.Send(command);
            return Ok(ApiResponse<UpdateTaskResult>.Ok(result, "Updated Successfully"));
        }
        [HttpPatch("{taskId}/status")]
        public async Task<IActionResult>UpdateTaskStatus([FromRoute]int taskId)
    }
}
