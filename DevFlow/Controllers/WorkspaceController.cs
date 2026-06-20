using DevFlow.Api.Contracts.Responses;
using DevFlow.Application.Workspaces.AddWorkspaceMember;
using DevFlow.Application.Workspaces.CreateWorkspace;
using DevFlow.Application.Workspaces.GetWorkspaceById;
using DevFlow.Application.Workspaces.GetWorkspaceMembers;
using DevFlow.Application.Workspaces.GetWorkspaces;
using DevFlow.Application.Workspaces.RemoveWorkspaceMember;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DevFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkspaceController : ControllerBase
    {
        private readonly IMediator _mediator;
        public WorkspaceController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("CreateWorkspace")]
        public async Task<IActionResult> CreateWorkspace(CreateWorkspaceCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<CreateWorkspaceResult>.Ok(result, "Workspace Created Successfully"));

        }
        [HttpGet("my")]
        public async Task<IActionResult> GetMyWorkspaces()
        {
            var result = await _mediator.Send(new GetMyWorkspacesQuery());
            return Ok(ApiResponse<List<GetMyWorkspacesResult>>.Ok(result, "Retrieved all workspaces"));

        }

        [HttpGet("{workspaceId}")]
        public async Task<IActionResult> GetById([FromRoute] int workspaceId)
        {
            var result = await _mediator.Send(new GetWorkspaceByIdQuery { WorkspaceId = workspaceId });
            return Ok(ApiResponse<GetWorkspaceByIdResult>.Ok(result, "Retrieved Successfully"));
        }

        [HttpGet("{workspaceId}/members")]
        public async Task<IActionResult> GetWorkspaceMembers([FromRoute] int workspaceId)
        {

            var result=await _mediator.Send(new GetWorkspaceMembersQuery { WorkspaceId = workspaceId });
            return Ok(ApiResponse<List<GetWorkspaceMembersResult>>.Ok(result, "Retrieved Successfully"));
        }
        [HttpDelete("{workspaceId}/members/{userId}")]
        public async Task<IActionResult> RemoveWorkspaceMember([FromRoute]RemoveWorkspaceMemberCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<RemoveWorkspaceMemberResult>.Ok(result, "Removed Successfully"));
        }
        [HttpPost("AddWorkspaceMember")]
        public async Task<IActionResult>AddWorkspaceMember(AddWorkspaceMemberCommand Command)
        {
            var result = await _mediator.Send(Command);
            return Ok(ApiResponse<AddWorkspaceMemberResult>.Ok(result, "Member Added Successfully"));
        }
        
        

        }

    }

