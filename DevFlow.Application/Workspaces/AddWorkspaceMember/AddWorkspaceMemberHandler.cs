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
        private readonly IWorkspaceMemberRepository _workspaceMemberRepository;
        private readonly IUserRepository _userRepository;
        private readonly IWorkspaceAuthorizationService _workspaceAuthorizationService;
        private readonly IUnitOfWork _unitOfWork;


        public AddWorkspaceMemberHandler(IUnitOfWork unitOfWork,IWorkspaceAuthorizationService workspaceAuthorizationService,IUserRepository userRepository,IWorkspaceMemberRepository workspaceMemberRepository, IWorkspaceRepository workspaceRepository)
        {

            _workspaceRepository = workspaceRepository;
            _workspaceMemberRepository = workspaceMemberRepository;
            _userRepository=userRepository;
            _workspaceAuthorizationService = workspaceAuthorizationService;
            _unitOfWork = unitOfWork;
        }
        public async Task<AddWorkspaceMemberResult> Handle(AddWorkspaceMemberCommand request, CancellationToken cancellationToken)
        {
            
            var workspace = await _workspaceRepository.GetByIdAsync(request.WorkspaceId);
            if (workspace == null)
            {
                throw new NotFoundException("No such Workspace Exist");
            }
            await _workspaceAuthorizationService.EnsureAdminOrOwnerAsync(request.WorkspaceId);
            var user = await _userRepository.GetByUserIdAsync(request.UserId);

            if (user == null)
            {
                throw new NotFoundException("User does not exist");
            }
           
            var existingMember =
             await _workspaceMemberRepository.GetMemberAsync(
       request.UserId,
       request.WorkspaceId);

            if (existingMember != null)
            {
                throw new ConflictException(
                    "User is already a workspace member");
            }
            var member = new WorkspaceMember
            {
                UserId = request.UserId,
                WorkspaceId = request.WorkspaceId,
                Role = request.Role,
                JoinedAt = DateTime.UtcNow,

            };
            await _workspaceMemberRepository.AddAsync(member);
            await _unitOfWork.SaveChangesAsync();
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
