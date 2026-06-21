using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using DevFlow.Domain.Enum;
using DevFlow.Domain.Entities;

namespace DevFlow.Application.Workspaces.AddWorkspaceMember
{
    public class AddWorkspaceMemberHandler : IRequestHandler<AddWorkspaceMemberCommand, AddWorkspaceMemberResult>
    {
        public readonly IWorkspaceRepository _workspaceRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IWorkspaceMemberRepository _workspaceMemberRepository;
        private readonly IUserRepository _userRepository;


        public AddWorkspaceMemberHandler(IUserRepository userRepository,IWorkspaceMemberRepository workspaceMemberRepository, IWorkspaceRepository workspaceRepository, ICurrentUserService currentUserService)
        {

            _workspaceRepository = workspaceRepository;
            _currentUserService = currentUserService;
            _workspaceMemberRepository = workspaceMemberRepository;
            _userRepository=userRepository;
        }
        public async Task<AddWorkspaceMemberResult> Handle(AddWorkspaceMemberCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var membership = await _workspaceMemberRepository.GetMemberAsync(userId, request.WorkspaceId);
            if (membership == null)
            {
                throw new UnauthorizedException("Not a workspace member");
            }

            if (membership.Role !=WorkspaceRole.Admin && membership.Role !=WorkspaceRole.Owner)
            {
                throw new UnauthorizedException("Not allowed to add");
            }
            var user = await _userRepository.GetByUserIdAsync(request.UserId);

            if (user == null)
            {
                throw new NotFoundException("User does not exist");
            }
           
           
            var workspace = await _workspaceRepository.GetByIdAsync(request.WorkspaceId);
            if (workspace == null)
            {
                throw new NotFoundException("No such Workspace Exist");
            }

            var member = new WorkspaceMember
            {
                UserId = request.UserId,
                WorkspaceId = request.WorkspaceId,
                Role = request.Role,
                JoinedAt = DateTime.UtcNow,

            };
            await _workspaceMemberRepository.AddAsync(member);
            var result = new AddWorkspaceMemberResult
            {
                UserId = member.UserId,
                WorkspaceId = member.WorkspaceId,
                Role = member.Role,

            };


            return result;
           
             
        }
    }
}
