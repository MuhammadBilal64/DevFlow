using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Projects.UpdateProject;
using DevFlow.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Projects.UpdateProject
{
    public class UpdateProjectHandlerTests
    {
        [Fact]
        public async Task Should_Update_Project_Successfully()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var projectRepositoryMock = new Mock<IProjectRepository>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();
            var workspaceMemberRepositoryMock = new Mock<IWorkspaceMemberRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new UpdateProjectHandler(
                workspaceAuthorizationServiceMock.Object,
                unitOfWorkMock.Object,
                projectRepositoryMock.Object,
                workspaceMemberRepositoryMock.Object,
                currentUserServiceMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var command = new UpdateProjectCommand
            {
                ProjectId = 10,
                Name = "Updated Project",
                Description = "Updated Description"
            };

            var project = new Project(
                "Old Project",
                "Old Description",
                5,
                1);

            projectRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(project);

            projectRepositoryMock
                .Setup(x => x.ExistsInWorkspaceAsync(5, "Updated Project"))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();

            result.ProjectName.Should().Be("Updated Project");
            result.Description.Should().Be("Updated Description");

            projectRepositoryMock.Verify(x => x.GetByIdAsync(10), Times.Once);
            workspaceAuthorizationServiceMock.Verify(x => x.EnsureAdminOrOwnerAsync(5), Times.Once);
            projectRepositoryMock.Verify(x => x.ExistsInWorkspaceAsync(5, "Updated Project"), Times.Once);
            projectRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Project>()), Times.Once);
            unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Should_Throw_NotFoundException_When_Project_Does_Not_Exist()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var projectRepositoryMock = new Mock<IProjectRepository>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();
            var workspaceMemberRepositoryMock = new Mock<IWorkspaceMemberRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new UpdateProjectHandler(
                workspaceAuthorizationServiceMock.Object,
                unitOfWorkMock.Object,
                projectRepositoryMock.Object,
                workspaceMemberRepositoryMock.Object,
                currentUserServiceMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var command = new UpdateProjectCommand
            {
                ProjectId = 10,
                Name = "Updated Project",
                Description = "Updated Description"
            };

            projectRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync((Project?)null);

            // Act
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();

            // Verifies
            projectRepositoryMock.Verify(x => x.GetByIdAsync(10), Times.Once);

            workspaceAuthorizationServiceMock.Verify(
                x => x.EnsureAdminOrOwnerAsync(It.IsAny<int>()),
                Times.Never);

            projectRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<Project>()),
                Times.Never);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task Should_Throw_UnauthorizedException_When_User_Is_Not_Admin_Or_Owner()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var projectRepositoryMock = new Mock<IProjectRepository>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();
            var workspaceMemberRepositoryMock = new Mock<IWorkspaceMemberRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new UpdateProjectHandler(
                workspaceAuthorizationServiceMock.Object,
                unitOfWorkMock.Object,
                projectRepositoryMock.Object,
                workspaceMemberRepositoryMock.Object,
                currentUserServiceMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var command = new UpdateProjectCommand
            {
                ProjectId = 10,
                Name = "Updated Project",
                Description = "Updated Description"
            };

            var project = new Project(
                "Old Project",
                "Old Description",
                5,
                1);

            projectRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(project);

            workspaceAuthorizationServiceMock
                .Setup(x => x.EnsureAdminOrOwnerAsync(5))
                .ThrowsAsync(new UnauthorizedException("Not Authorized"));

            // Act
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedException>();

            // Verifies
            projectRepositoryMock.Verify(x => x.GetByIdAsync(10), Times.Once);

            workspaceAuthorizationServiceMock.Verify(
                x => x.EnsureAdminOrOwnerAsync(5),
                Times.Once);

            projectRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<Project>()),
                Times.Never);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }
        [Fact]
        public async Task Should_Throw_ConflictException_When_Project_Name_Already_Exists()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var projectRepositoryMock = new Mock<IProjectRepository>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();
            var workspaceMemberRepositoryMock = new Mock<IWorkspaceMemberRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new UpdateProjectHandler(
                workspaceAuthorizationServiceMock.Object,
                unitOfWorkMock.Object,
                projectRepositoryMock.Object,
                workspaceMemberRepositoryMock.Object,
                currentUserServiceMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var command = new UpdateProjectCommand
            {
                ProjectId = 10,
                Name = "Updated Project",
                Description = "Updated Description"
            };

            var project = new Project(
                "Old Project",
                "Old Description",
                5,
                1);

            projectRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(project);

            projectRepositoryMock
                .Setup(x => x.ExistsInWorkspaceAsync(5, "Updated Project"))
                .ReturnsAsync(true);

            // Act
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ConflictException>();

            // Verifies
            projectRepositoryMock.Verify(x => x.GetByIdAsync(10), Times.Once);

            workspaceAuthorizationServiceMock.Verify(
                x => x.EnsureAdminOrOwnerAsync(5),
                Times.Once);

            projectRepositoryMock.Verify(
                x => x.ExistsInWorkspaceAsync(5, "Updated Project"),
                Times.Once);

            projectRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<Project>()),
                Times.Never);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }
    }
}