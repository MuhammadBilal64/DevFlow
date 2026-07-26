using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Projects.GetProjectById;
using DevFlow.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Projects.GetProjectById
{
    public class GetProjectByIdHandlerTests
    {
        [Fact]
        public async Task Should_Return_Project_By_Id()
        {
            // Arrange
            var projectRepositoryMock = new Mock<IProjectRepository>();
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var workspaceMemberRepositoryMock = new Mock<IWorkspaceMemberRepository>();

            var handler = new GetProjectByIdHandler(
                projectRepositoryMock.Object,
                currentUserServiceMock.Object,
                workspaceMemberRepositoryMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var query = new GetProjectByIdQuery
            {
                Id = 10
            };

            var project = new Project(
      "AI Project",
      "Project Description",
      5,
      1);

            var member = new WorkspaceMember
            {
                UserId = 1,
                WorkspaceId = 5
            };

            projectRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(project);

            workspaceMemberRepositoryMock
                .Setup(x => x.GetMemberAsync(1, 5))
                .ReturnsAsync(member);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();

            result.ProjectId.Should().Be(0);
            result.ProjectName.Should().Be("AI Project");
            result.Description.Should().Be("Project Description");
            result.CreatedAt.Should().Be(project.CreatedAt);

            // Verifies
            projectRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);

            workspaceMemberRepositoryMock.Verify(
                x => x.GetMemberAsync(1, 5),
                Times.Once);
        }
        [Fact]
        public async Task Should_Throw_NotFoundException_When_Project_Does_Not_Exist()
        {
            // Arrange
            var projectRepositoryMock = new Mock<IProjectRepository>();
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var workspaceMemberRepositoryMock = new Mock<IWorkspaceMemberRepository>();

            var handler = new GetProjectByIdHandler(
                projectRepositoryMock.Object,
                currentUserServiceMock.Object,
                workspaceMemberRepositoryMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var query = new GetProjectByIdQuery
            {
                Id = 10
            };

            projectRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync((Project?)null);

            // Act
            Func<Task> act = () => handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();

            // Verifies
            projectRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);

            workspaceMemberRepositoryMock.Verify(
                x => x.GetMemberAsync(It.IsAny<int>(), It.IsAny<int>()),
                Times.Never);
        }
        [Fact]
        public async Task Should_Throw_UnauthorizedException_When_User_Is_Not_A_Workspace_Member()
        {
            // Arrange
            var projectRepositoryMock = new Mock<IProjectRepository>();
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var workspaceMemberRepositoryMock = new Mock<IWorkspaceMemberRepository>();

            var handler = new GetProjectByIdHandler(
                projectRepositoryMock.Object,
                currentUserServiceMock.Object,
                workspaceMemberRepositoryMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var query = new GetProjectByIdQuery
            {
                Id = 10
            };

            var project = new Project(
                "AI Project",
                "Project Description",
                5,
                1);

            projectRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(project);

            workspaceMemberRepositoryMock
                .Setup(x => x.GetMemberAsync(1, 5))
                .ReturnsAsync((WorkspaceMember?)null);

            // Act
            Func<Task> act = () => handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedException>();

            // Verifies
            projectRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);

            workspaceMemberRepositoryMock.Verify(
                x => x.GetMemberAsync(1, 5),
                Times.Once);
        }
    }
}