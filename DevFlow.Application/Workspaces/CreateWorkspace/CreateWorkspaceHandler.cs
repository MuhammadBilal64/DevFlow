using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using MediatR;

namespace DevFlow.Application.Workspaces.CreateWorkspace
{
    public class CreateWorkspaceHandler : IRequestHandler<CreateWorkspaceCommand, CreateWorkspaceResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IWorkspaceMemberRepository _workspaceMemberRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateWorkspaceHandler(IUnitOfWork unitOfWork,IUserRepository userRepository,IWorkspaceRepository workSpaceRepository,ICurrentUserService currentUserService,IWorkspaceMemberRepository workspaceMemberRepository)
        {
            _userRepository = userRepository;
            _workspaceRepository= workSpaceRepository;
            _currentUserService = currentUserService;
            _workspaceMemberRepository = workspaceMemberRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<CreateWorkspaceResult> Handle(CreateWorkspaceCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var workspace = new Workspace
            {
                Name = request.Name,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
            };
            await _workspaceRepository.AddAsync(workspace);

            var member = new WorkspaceMember
            {
                WorkspaceId=workspace.Id,
                UserId=workspace.CreatedBy,
                Role=WorkspaceRole.Owner,
                JoinedAt=DateTime.UtcNow,
            };
            await _workspaceMemberRepository.AddAsync(member);
            await _unitOfWork.SaveChangesAsync();
            var result = new CreateWorkspaceResult
            {
                Name= workspace.Name,
                Id= workspace.Id,
            };
            return result;

        }
    }
}
