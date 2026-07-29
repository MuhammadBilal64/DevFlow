using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Workspaces.AddWorkspaceMember;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Workspaces.AddWorkspaceMember
{
    public class AddWorkspaceMemberHandlerTests
    {
        [Fact]
        public async Task Should_Add_Workspace_Member_Successfully()
        {
            // Arrange
            var workspaceRepositoryMock = new Mock<IWorkspaceRepository>();
            var workspaceMemberRepositoryMock = new Mock<IWorkspaceMemberRepository>();
            var userRepositoryMock = new Mock<IUserRepository>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new AddWorkspaceMemberHandler(
                unitOfWorkMock.Object,
                workspaceAuthorizationServiceMock.Object,
                userRepositoryMock.Object,
                workspaceMemberRepositoryMock.Object,
                workspaceRepositoryMock.Object);

            var command = new AddWorkspaceMemberCommand
            {
                WorkspaceId = 10,
                UserId = 2,
                Role = WorkspaceRole.Member
            };
            var workspace = new Workspace
            {
                Id = 10,
                Name = "Development"
            };
            var user = new User
            {
                Id = 2
            };
            workspaceRepositoryMock.Setup(x => x.GetByIdAsync(10)).ReturnsAsync(workspace);
            userRepositoryMock
    .Setup(x => x.GetByUserIdAsync(2))
    .ReturnsAsync(user);

            workspaceMemberRepositoryMock
    .Setup(x => x.GetMemberAsync(2, 10))
    .ReturnsAsync((WorkspaceMember?)null);
            // Act
            var result = await handler.Handle(command, CancellationToken.None);
            //assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(2);
            result.WorkspaceId.Should().Be(10);
            result.Role.Should().Be(WorkspaceRole.Member);
            workspaceMemberRepositoryMock.Verify(
    x => x.AddAsync(It.Is<WorkspaceMember>(m =>
        m.UserId == 2 &&
        m.WorkspaceId == 10 &&
        m.Role == WorkspaceRole.Member)),
    Times.Once);
            workspaceRepositoryMock.Verify(
    x => x.GetByIdAsync(10),
    Times.Once);
            unitOfWorkMock.Verify(
    x => x.SaveChangesAsync(),
    Times.Once);
            workspaceAuthorizationServiceMock.Verify(
    x => x.EnsureAdminOrOwnerAsync(10),
    Times.Once);







        }
        [Fact]
        public async Task Should_Throw_NotFoundException_When_Workspace_Does_Not_Exist()
        {
            // Arrange
            var workspaceRepositoryMock = new Mock<IWorkspaceRepository>();
            var workspaceMemberRepositoryMock = new Mock<IWorkspaceMemberRepository>();
            var userRepositoryMock = new Mock<IUserRepository>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new AddWorkspaceMemberHandler(
                unitOfWorkMock.Object,
                workspaceAuthorizationServiceMock.Object,
                userRepositoryMock.Object,
                workspaceMemberRepositoryMock.Object,
                workspaceRepositoryMock.Object);

            var command = new AddWorkspaceMemberCommand
            {
                WorkspaceId = 10,
                UserId = 2,
                Role = WorkspaceRole.Member
            };

            workspaceRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync((Workspace?)null);

            // Act
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();

            workspaceRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);

            workspaceAuthorizationServiceMock.Verify(
                x => x.EnsureAdminOrOwnerAsync(It.IsAny<int>()),
                Times.Never);
        }
        [Fact]
        public async Task Should_Throw_NotFoundException_When_User_Does_Not_Exist()
        {
            // Arrange
            var workspaceRepositoryMock = new Mock<IWorkspaceRepository>();
            var workspaceMemberRepositoryMock = new Mock<IWorkspaceMemberRepository>();
            var userRepositoryMock = new Mock<IUserRepository>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new AddWorkspaceMemberHandler(
                unitOfWorkMock.Object,
                workspaceAuthorizationServiceMock.Object,
                userRepositoryMock.Object,
                workspaceMemberRepositoryMock.Object,
                workspaceRepositoryMock.Object);

            var command = new AddWorkspaceMemberCommand
            {
                WorkspaceId = 10,
                UserId = 2,
                Role = WorkspaceRole.Member
            };

            var workspace = new Workspace
            {
                Id = 10,
                Name = "Development"
            };

            workspaceRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(workspace);

            userRepositoryMock
                .Setup(x => x.GetByUserIdAsync(2))
                .ReturnsAsync((User?)null);

            // Act
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();

            workspaceRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);

            workspaceAuthorizationServiceMock.Verify(
                x => x.EnsureAdminOrOwnerAsync(10),
                Times.Once);

            userRepositoryMock.Verify(
                x => x.GetByUserIdAsync(2),
                Times.Once);

            workspaceMemberRepositoryMock.Verify(
                x => x.GetMemberAsync(It.IsAny<int>(), It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public async Task Should_Throw_ConflictException_When_User_Is_Already_A_Workspace_Member()
        {

            // Arrange
            var workspaceRepositoryMock = new Mock<IWorkspaceRepository>();
            var workspaceMemberRepositoryMock = new Mock<IWorkspaceMemberRepository>();
            var userRepositoryMock = new Mock<IUserRepository>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new AddWorkspaceMemberHandler(
                unitOfWorkMock.Object,
                workspaceAuthorizationServiceMock.Object,
                userRepositoryMock.Object,
                workspaceMemberRepositoryMock.Object,
                workspaceRepositoryMock.Object);
            var commmand = new AddWorkspaceMemberCommand
            {
                WorkspaceId = 10,
                UserId = 2,
                Role = WorkspaceRole.Member
            };

            var workspace = new Workspace
            {
                Id = 10,
                Name = "Development"
            };
            var user = new User
            {
                Id = 2
            };

            var existingMember = new WorkspaceMember
            {
                UserId = 2,
                WorkspaceId = 10,
                Role = WorkspaceRole.Member
            };
            workspaceRepositoryMock.Setup(x => x.GetByIdAsync(10)).ReturnsAsync(workspace);

            userRepositoryMock
                .Setup(x => x.GetByUserIdAsync(2))
                .ReturnsAsync(user
                );
            workspaceMemberRepositoryMock
        .Setup(x => x.GetMemberAsync(2, 10))
        .ReturnsAsync(existingMember);
            Func<Task> act = () => handler.Handle(commmand, CancellationToken.None);

            await act.Should().ThrowAsync<ConflictException>();
            workspaceRepositoryMock.Verify(
    x => x.GetByIdAsync(10),
    Times.Once);

            workspaceAuthorizationServiceMock.Verify(
                x => x.EnsureAdminOrOwnerAsync(10),
                Times.Once);

            userRepositoryMock.Verify(
                x => x.GetByUserIdAsync(2),
                Times.Once);

            workspaceMemberRepositoryMock.Verify(
                x => x.GetMemberAsync(2, 10),
                Times.Once);

            workspaceMemberRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<WorkspaceMember>()),
                Times.Never);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);



        }


    }
}
