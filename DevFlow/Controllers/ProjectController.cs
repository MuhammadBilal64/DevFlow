using DevFlow.Api.Contracts.Responses;
using DevFlow.Application.Common.Models;
using DevFlow.Application.ProjectMembers.AddProjectMember;
using DevFlow.Application.ProjectMembers.GetProjectMembers;
using DevFlow.Application.ProjectMembers.RemoveProjectMember;
using DevFlow.Application.Projects.CreateProject;
using DevFlow.Application.Projects.GetMyProjectsByWorkspace;
using DevFlow.Application.Projects.GetProjectById;
using DevFlow.Application.Projects.UpdateProject;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevFlow.Api.Controllers
{
    [Route("api/projects")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProjectController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateProject(CreateProjectCommand command)
        {

            var result = await _mediator.Send(command);
            return Ok(ApiResponse<CreateProjectResult>.Ok(result, "Project Created Successfully"));

        }
        [HttpGet("{Id}")]
        [Authorize]
        public async Task<IActionResult> GetProjectById([FromRoute] int Id)
        {
            var result = await _mediator.Send(new GetProjectByIdQuery { Id = Id });
            return Ok(ApiResponse<GetProjectByIdResult>.Ok(result, "Retrieved Successfully"));
        }
        [HttpGet("workspace/{workspaceId}")]
        [Authorize]
        public async Task<IActionResult> GetMyProjectsByWorkspace([FromRoute] int WorkspaceId, [FromQuery] GetMyProjectsByWorkspaceQuery query)
        {

            query.WorkspaceId = WorkspaceId;
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<PagedResult<GetMyProjectsByWorkspaceResult>>.Ok(result, "Retrieved Successfully"));

        }
        [HttpPut("{ProjectId}")]
        [Authorize]
        public async Task<IActionResult> UpdateProject([FromRoute] int ProjectId, UpdateProjectCommand command)
        {
            command.ProjectId = ProjectId;
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<UpdateProjectResult>.Ok(result, "Updated Successfully"));
        }
        [HttpPost("{projectId}/members")]
        [Authorize]
        public async Task<IActionResult> AddProjectMember(
    [FromRoute] int projectId,
    AddProjectMemberCommand command)
        {
            command.ProjectId = projectId;

            var result = await _mediator.Send(command);

            return Ok(ApiResponse<AddProjectMemberResult>.Ok(
                result,
                "Project member added successfully"));
        }

        [HttpGet("{projectId}/members")]
        [Authorize]
        public async Task<IActionResult> GetProjectMembers(
            [FromRoute] int projectId)
        {
            var result = await _mediator.Send(new GetProjectMembersQuery
            {
                ProjectId = projectId
            });

            return Ok(ApiResponse<List<GetProjectMembersResult>>.Ok(
                result,
                "Retrieved successfully"));
        }

        [HttpDelete("{projectId}/members/{userId}")]
        [Authorize]
        public async Task<IActionResult> RemoveProjectMember(
            [FromRoute] int projectId,
            [FromRoute] int userId)
        {
            var command = new RemoveProjectMemberCommand
            {
                ProjectId = projectId,
                UserId = userId
            };

            var result = await _mediator.Send(command);

            return Ok(ApiResponse<RemoveProjectMemberResult>.Ok(
                result,
                "Project member removed successfully"));
        }

    }
}
