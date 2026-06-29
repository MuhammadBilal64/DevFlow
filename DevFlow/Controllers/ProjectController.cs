using System.Security.Cryptography.Pkcs;
using DevFlow.Api.Contracts.Responses;
using DevFlow.Application.Common.Models;
using DevFlow.Application.Projects.CreateProject;
using DevFlow.Application.Projects.GetProjectById;
using DevFlow.Application.Projects.GetProjectsByWorkspace;
using DevFlow.Application.Projects.UpdateProject;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

            var result=await _mediator.Send(command);
            return Ok(ApiResponse<CreateProjectResult>.Ok(result,"Project Created Successfully"));

        }
        [HttpGet("{Id}")]
        [Authorize]
        public async Task<IActionResult>GetProjectById([FromRoute]int Id)
        {
            var result=await _mediator.Send(new GetProjectByIdQuery { Id=Id});
            return Ok(ApiResponse<GetProjectByIdResult>.Ok(result, "Retrieved Successfully"));
        }
        [HttpGet("workspace/{workspaceId}")]
        [Authorize]
        public async Task<IActionResult>GetProjectsByWorkspace([FromRoute] int WorkspaceId,[FromQuery]GetProjectsByWorkspaceQuery query){

            query.WorkspaceId = WorkspaceId;
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<PagedResult<GetProjectsByWorkspaceResult>>.Ok(result, "Retrieved Successfully"));

        }
        [HttpPut("{ProjectId}")]
        [Authorize]
        public async Task<IActionResult>UpdateProject([FromRoute]int ProjectId,UpdateProjectCommand command)
        {
            command.ProjectId = ProjectId;
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<UpdateProjectResult>.Ok(result,"Updated Successfully"));
        }

    }
}
