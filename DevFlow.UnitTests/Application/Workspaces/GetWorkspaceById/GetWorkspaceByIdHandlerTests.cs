using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Workspaces.GetWorkspaceById;
using DevFlow.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Workspaces.GetWorkspaceById
{
    public class GetWorkspaceByIdHandlerTests
    {
        [Fact]
        public async Task Should_Return_Workspace_When_User_Is_A_Member()
        {
            // Arrange

            var workspaceRepositoryMock = new Mock<IWorkspaceRepository>();

            var workspaceMemberRepositoryMock = new Mock<IWorkspaceMemberRepository>();

            var currentUserServiceMock = new Mock<ICurrentUserService>();
            currentUserServiceMock
    .Setup(x => x.UserId)
    .Returns(1);
            var member = new WorkspaceMember
            {
                UserId = 1,
                WorkspaceId = 10
            };

            var workspace = new Workspace
            {
                Id = 10,
                Name = "Development",
                CreatedAt = DateTime.UtcNow
            };
            workspaceMemberRepositoryMock
    .Setup(x => x.GetMemberAsync(1, 10))
    .ReturnsAsync(member);

            workspaceRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(workspace);

            var handler = new GetWorkspaceByIdHandler(
    currentUserServiceMock.Object,
    workspaceMemberRepositoryMock.Object,
    workspaceRepositoryMock.Object);
            var query = new GetWorkspaceByIdQuery
            {
                WorkspaceId = 10
            };
            // Act
            var result = await handler.Handle(query, CancellationToken.None);
            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(10);
            result.Name.Should().Be("Development");
            result.CreatedAt.Should().Be(workspace.CreatedAt);
            // Verify
            workspaceMemberRepositoryMock.Verify(
                x => x.GetMemberAsync(1, 10),
                Times.Once);

            workspaceRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);

        }
    }
}
