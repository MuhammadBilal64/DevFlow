using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using DevFlow.Domain.Events;
using MediatR;

namespace DevFlow.Application.DomainEvents.ProjectCreated
{
    public class ProjectCreatedEventHandler : INotificationHandler<ProjectCreatedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationRepository _notificationRepository;
        private readonly IWorkspaceMemberRepository _workspaceMemberRepository;
        public ProjectCreatedEventHandler(IUnitOfWork unitOfWork, INotificationRepository notificationRepository, IWorkspaceMemberRepository workspaceMemberRepository)
        {
            _unitOfWork = unitOfWork;
            _notificationRepository = notificationRepository;
            _workspaceMemberRepository = workspaceMemberRepository;
        }

        public  async Task Handle(ProjectCreatedEvent notification, CancellationToken cancellationToken)
        {
            
            var members = await _workspaceMemberRepository.GetAllByWorkspaceIdAsync(notification.WorkspaceId);
            
            foreach (var member in members)
            {
                if (member.UserId == notification.CreatedBy)
                    continue;

                var notificationEntity = new Notification(
     member.UserId,
     $"Project '{notification.ProjectName}' was created.",
     NotificationType.ProjectCreated);
                    await _notificationRepository.AddAsync(notificationEntity );
                

            }
            await _unitOfWork.SaveChangesAsync();

        }
    }
}
