using DevFlow.Api.Contracts.Responses;
using DevFlow.Application.Common.Models;
using DevFlow.Application.Workspaces.AddWorkspaceMember;
using DevFlow.Application.Workspaces.CreateWorkspace;
using DevFlow.Application.Workspaces.GetWorkspaceById;
using DevFlow.Application.Workspaces.GetWorkspaceMembers;
using DevFlow.Application.Workspaces.GetWorkspaces;
using DevFlow.Application.Workspaces.RemoveWorkspaceMember;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DevFlow.Api.Controllers
{
    [Route("api/workspaces")]
    [ApiController]
    public class WorkspaceController : ControllerBase
    {
        private readonly IMediator _mediator;
        public WorkspaceController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateWorkspace(CreateWorkspaceCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<CreateWorkspaceResult>.Ok(result, "Workspace Created Successfully"));

        }
        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMyWorkspaces([FromQuery]GetMyWorkspacesQuery query)
        {
            
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<PagedResult<GetMyWorkspacesResult>>.Ok(result, "Retrieved all workspaces"));

        }

        [HttpGet("{workspaceId}")]
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] int workspaceId)
        {
            var result = await _mediator.Send(new GetWorkspaceByIdQuery { WorkspaceId = workspaceId });
            return Ok(ApiResponse<GetWorkspaceByIdResult>.Ok(result, "Retrieved Successfully"));
        }

        [HttpGet("{workspaceId}/members")]
        [Authorize]
        public async Task<IActionResult> GetWorkspaceMembers([FromRoute] int workspaceId,[FromQuery] GetWorkspaceMembersQuery query)
        {
            query.WorkspaceId = workspaceId;
            var result=await _mediator.Send(query);
            return Ok(ApiResponse<PagedResult<GetWorkspaceMembersResult>>.Ok(result, "Retrieved Successfully"));
        }
        [HttpDelete("{workspaceId}/members/{userId}")]
        [Authorize]
        public async Task<IActionResult> RemoveWorkspaceMember([FromRoute] int userId,[FromRoute]int workspaceId)
        {
            var command = new RemoveWorkspaceMemberCommand
            {
                UserId=userId,
                WorkspaceId=workspaceId
            };
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<RemoveWorkspaceMemberResult>.Ok(result, "Removed Successfully"));
        }
        [HttpPost("{workspaceId}/members")]
        [Authorize]
        public async Task<IActionResult>AddWorkspaceMember([FromRoute]int workspaceId,AddWorkspaceMemberCommand Command)
        {
            Command.WorkspaceId=workspaceId;
            var result = await _mediator.Send(Command);

            return Ok(ApiResponse<AddWorkspaceMemberResult>.Ok(result, "Member Added Successfully"));
        }
        
        

        }

    }

