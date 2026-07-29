using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Projects.CreateProject;
using DevFlow.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Projects.CreateProject
{
    public class CreateProjectHandlerTests
    {
        [Fact]
        public async Task Should_Create_Project_Successfully()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var projectRepositoryMock = new Mock<IProjectRepository>();
            var workspaceRepositoryMock = new Mock<IWorkspaceRepository>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new CreateProjectHandler(
                workspaceAuthorizationServiceMock.Object,
                workspaceRepositoryMock.Object,
                unitOfWorkMock.Object,
                currentUserServiceMock.Object,
                projectRepositoryMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var command = new CreateProjectCommand
            {
                ProjectName = "AI Project",
                Description = "Project Description",
                WorkspaceId = 10
            };

            var workspace = new Workspace
            {
                Id = 10,
                Name = "Development",
                CreatedBy = 1
            };

            workspaceRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(workspace);

            projectRepositoryMock
                .Setup(x => x.ExistsInWorkspaceAsync(10, "AI Project"))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();

            result.ProjectId.Should().Be(0);
            result.ProjectName.Should().Be("AI Project");

            // Verifies
            workspaceRepositoryMock.Verify(x => x.GetByIdAsync(10), Times.Once);
            workspaceAuthorizationServiceMock.Verify(x => x.EnsureAdminOrOwnerAsync(10), Times.Once);
            projectRepositoryMock.Verify(x => x.ExistsInWorkspaceAsync(10, "AI Project"), Times.Once);
            projectRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Project>()), Times.Once);
            unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Should_Throw_NotFoundException_When_Workspace_Does_Not_Exist()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var projectRepositoryMock = new Mock<IProjectRepository>();
            var workspaceRepositoryMock = new Mock<IWorkspaceRepository>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new CreateProjectHandler(
                workspaceAuthorizationServiceMock.Object,
                workspaceRepositoryMock.Object,
                unitOfWorkMock.Object,
                currentUserServiceMock.Object,
                projectRepositoryMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var command = new CreateProjectCommand
            {
                ProjectName = "AI Project",
                Description = "Project Description",
                WorkspaceId = 10
            };

            workspaceRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync((Workspace?)null);

            // Act
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();

            // Verifies
            workspaceRepositoryMock.Verify(x => x.GetByIdAsync(10), Times.Once);

            workspaceAuthorizationServiceMock.Verify(
                x => x.EnsureAdminOrOwnerAsync(It.IsAny<int>()),
                Times.Never);

            projectRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Project>()),
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
            var workspaceRepositoryMock = new Mock<IWorkspaceRepository>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new CreateProjectHandler(
                workspaceAuthorizationServiceMock.Object,
                workspaceRepositoryMock.Object,
                unitOfWorkMock.Object,
                currentUserServiceMock.Object,
                projectRepositoryMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var command = new CreateProjectCommand
            {
                ProjectName = "AI Project",
                Description = "Project Description",
                WorkspaceId = 10
            };

            var workspace = new Workspace
            {
                Id = 10,
                Name = "Development",
                CreatedBy = 1
            };

            workspaceRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(workspace);

            workspaceAuthorizationServiceMock
                .Setup(x => x.EnsureAdminOrOwnerAsync(10))
                .ThrowsAsync(new UnauthorizedException("Not Authorized"));

            // Act
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedException>();

            // Verifies
            workspaceRepositoryMock.Verify(x => x.GetByIdAsync(10), Times.Once);
            workspaceAuthorizationServiceMock.Verify(x => x.EnsureAdminOrOwnerAsync(10), Times.Once);

            projectRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Project>()),
                Times.Never);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task Should_Throw_ConflictException_When_Project_Already_Exists()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var projectRepositoryMock = new Mock<IProjectRepository>();
            var workspaceRepositoryMock = new Mock<IWorkspaceRepository>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new CreateProjectHandler(
                workspaceAuthorizationServiceMock.Object,
                workspaceRepositoryMock.Object,
                unitOfWorkMock.Object,
                currentUserServiceMock.Object,
                projectRepositoryMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var command = new CreateProjectCommand
            {
                ProjectName = "AI Project",
                Description = "Project Description",
                WorkspaceId = 10
            };

            var workspace = new Workspace
            {
                Id = 10,
                Name = "Development",
                CreatedBy = 1
            };

            workspaceRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(workspace);

            projectRepositoryMock
                .Setup(x => x.ExistsInWorkspaceAsync(10, "AI Project"))
                .ReturnsAsync(true);

            // Act
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ConflictException>();

            // Verifies
            workspaceRepositoryMock.Verify(x => x.GetByIdAsync(10), Times.Once);
            workspaceAuthorizationServiceMock.Verify(x => x.EnsureAdminOrOwnerAsync(10), Times.Once);
            projectRepositoryMock.Verify(x => x.ExistsInWorkspaceAsync(10, "AI Project"), Times.Once);

            projectRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Project>()),
                Times.Never);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }
    }
}