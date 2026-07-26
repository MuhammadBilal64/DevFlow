using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Workspaces.RemoveWorkspaceMember;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Workspaces.RemoveWorkspaceMember
{
    public class RemoveWorkspaceMemberHandlerTests
    {
        [Fact]
    public async Task Should_Remove_Workspace_Member_Successfully(){
            // Arrange
            var workspaceMemberRepositoryMock = new Mock<IWorkspaceMemberRepository>();
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new RemoveWorkspaceMemberHandler(
                workspaceAuthorizationServiceMock.Object,
                unitOfWorkMock.Object,
                workspaceMemberRepositoryMock.Object,
                currentUserServiceMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var command = new RemoveWorkspaceMemberCommand
            {
                UserId = 2,
                WorkspaceId = 10
            };
            var member = new WorkspaceMember
            {
                UserId = 2,
                WorkspaceId = 10,
                Role = WorkspaceRole.Member
            };

            workspaceMemberRepositoryMock.Setup(x=>x.GetMemberAsync(2,10)).ReturnsAsync(member);
            //act
            var result =await  handler.Handle(command, CancellationToken.None);
            //assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            //verifies
            workspaceAuthorizationServiceMock.Verify(x => x.EnsureAdminOrOwnerAsync(10), Times.Once);
            workspaceMemberRepositoryMock.Verify(x => x.GetMemberAsync(2, 10), Times.Once);
            workspaceMemberRepositoryMock.Verify(x => x.RemoveAsync(2, 10), Times.Once);
            unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);




        }

        [Fact]
        public async Task Should_Throw_UnauthorizedException_When_User_Is_Not_Admin_Or_Owner()
        {
            // Arrange
            var workspaceMemberRepositoryMock = new Mock<IWorkspaceMemberRepository>();
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new RemoveWorkspaceMemberHandler(
                workspaceAuthorizationServiceMock.Object,
                unitOfWorkMock.Object,
                workspaceMemberRepositoryMock.Object,
                currentUserServiceMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);
            var command = new RemoveWorkspaceMemberCommand
            {
                UserId = 2,
                WorkspaceId = 10
            };
            workspaceAuthorizationServiceMock
                .Setup(x => x.EnsureAdminOrOwnerAsync(10))
                .ThrowsAsync(new UnauthorizedException("Not Authorized"));
            //act
            Func<Task>act=()=> handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<UnauthorizedException>();
            //verifies
            workspaceAuthorizationServiceMock.Verify(x => x.EnsureAdminOrOwnerAsync(10), Times.Once);
            workspaceMemberRepositoryMock.Verify(x => x.RemoveAsync(2, 10), Times.Never);
            unitOfWorkMock.Verify(x=>x.SaveChangesAsync(), Times.Never);





        }
        [Fact]
        public async Task Should_Throw_NotFoundException_When_Target_Member_Does_Not_Exist()
        {
            // Arrange
            var workspaceMemberRepositoryMock = new Mock<IWorkspaceMemberRepository>();
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new RemoveWorkspaceMemberHandler(
                workspaceAuthorizationServiceMock.Object,
                unitOfWorkMock.Object,
                workspaceMemberRepositoryMock.Object,
                currentUserServiceMock.Object);
            currentUserServiceMock
              .Setup(x => x.UserId)
              .Returns(1);
            var command = new RemoveWorkspaceMemberCommand
            {
                UserId = 2,
                WorkspaceId = 10
            };
            workspaceMemberRepositoryMock
     .Setup(x => x.GetMemberAsync(2, 10))
     .ReturnsAsync((WorkspaceMember?)null);

            //act
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>();
            //verifies
            workspaceAuthorizationServiceMock.Verify(x => x.EnsureAdminOrOwnerAsync(10), Times.Once);
            workspaceMemberRepositoryMock.Verify(x => x.GetMemberAsync(2, 10), Times.Once);
            workspaceMemberRepositoryMock.Verify(
    x => x.RemoveAsync(It.IsAny<int>(), It.IsAny<int>()),
    Times.Never);

            unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);



        }
        [Fact]
        public async Task Should_Throw_ConflictException_When_Target_Member_Is_Owner()
        {
            // Arrange
            var workspaceMemberRepositoryMock = new Mock<IWorkspaceMemberRepository>();
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new RemoveWorkspaceMemberHandler(
                workspaceAuthorizationServiceMock.Object,
                unitOfWorkMock.Object,
                workspaceMemberRepositoryMock.Object,
                currentUserServiceMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var command = new RemoveWorkspaceMemberCommand
            {
                UserId = 2,
                WorkspaceId = 10
            };

            var member = new WorkspaceMember
            {
                UserId = 2,
                WorkspaceId = 10,
                Role = WorkspaceRole.Owner
            };

            workspaceMemberRepositoryMock
                .Setup(x => x.GetMemberAsync(2, 10))
                .ReturnsAsync(member);

            // Act
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ConflictException>();

            // Verifies
            workspaceAuthorizationServiceMock.Verify(
                x => x.EnsureAdminOrOwnerAsync(10),
                Times.Once);

            workspaceMemberRepositoryMock.Verify(
                x => x.GetMemberAsync(2, 10),
                Times.Once);

            workspaceMemberRepositoryMock.Verify(
                x => x.RemoveAsync(It.IsAny<int>(), It.IsAny<int>()),
                Times.Never);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }
        [Fact]
        public async Task Should_Throw_ConflictException_When_User_Tries_To_Remove_Himself()
        {
            // Arrange
            var workspaceMemberRepositoryMock = new Mock<IWorkspaceMemberRepository>();
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new RemoveWorkspaceMemberHandler(
                workspaceAuthorizationServiceMock.Object,
                unitOfWorkMock.Object,
                workspaceMemberRepositoryMock.Object,
                currentUserServiceMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(2);

            var command = new RemoveWorkspaceMemberCommand
            {
                UserId = 2,
                WorkspaceId = 10
            };

            var member = new WorkspaceMember
            {
                UserId = 2,
                WorkspaceId = 10,
                Role = WorkspaceRole.Member
            };

            workspaceMemberRepositoryMock
                .Setup(x => x.GetMemberAsync(2, 10))
                .ReturnsAsync(member);

            // Act
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ConflictException>();

            // Verifies
            workspaceAuthorizationServiceMock.Verify(
                x => x.EnsureAdminOrOwnerAsync(10),
                Times.Once);

            workspaceMemberRepositoryMock.Verify(
                x => x.GetMemberAsync(2, 10),
                Times.Once);

            workspaceMemberRepositoryMock.Verify(
                x => x.RemoveAsync(It.IsAny<int>(), It.IsAny<int>()),
                Times.Never);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

    }
}
