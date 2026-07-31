using DevFlow.Application.Abstractions;
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
            var projectRepositoryMock = new Mock<IProjectRepository>();
            var projectAuthorizationServiceMock = new Mock<IProjectAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new UpdateProjectHandler(
                projectAuthorizationServiceMock.Object,
                unitOfWorkMock.Object,
                projectRepositoryMock.Object);

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
            typeof(Project)
    .GetProperty(nameof(Project.Id))!
    .SetValue(project, 10);

            projectRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(project);

            projectAuthorizationServiceMock
                .Setup(x => x.EnsureCanManageProjectAsync(10))
                .Returns(Task.CompletedTask);

            projectRepositoryMock
                .Setup(x => x.ExistsInWorkspaceAsync(5, "Updated Project"))
                .ReturnsAsync(false);

            var result = await handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.ProjectName.Should().Be("Updated Project");
            result.Description.Should().Be("Updated Description");

            projectRepositoryMock.Verify(x => x.GetByIdAsync(10), Times.Once);
            projectAuthorizationServiceMock.Verify(x => x.EnsureCanManageProjectAsync(10), Times.Once);
            projectRepositoryMock.Verify(x => x.ExistsInWorkspaceAsync(5, "Updated Project"), Times.Once);
            projectRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Project>()), Times.Once);
            unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }


        [Fact]
        public async Task Should_Throw_NotFoundException_When_Project_Does_Not_Exist()
        {
            var projectRepositoryMock = new Mock<IProjectRepository>();
            var projectAuthorizationServiceMock = new Mock<IProjectAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new UpdateProjectHandler(
                projectAuthorizationServiceMock.Object,
                unitOfWorkMock.Object,
                projectRepositoryMock.Object);

            var command = new UpdateProjectCommand
            {
                ProjectId = 10,
                Name = "Updated Project",
                Description = "Updated Description"
            };

            projectRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync((Project?)null);

            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();

            projectRepositoryMock.Verify(x => x.GetByIdAsync(10), Times.Once);
            projectAuthorizationServiceMock.Verify(
                x => x.EnsureCanManageProjectAsync(It.IsAny<int>()),
                Times.Never);
        }


        [Fact]
        public async Task Should_Throw_UnauthorizedException_When_User_Cannot_Manage_Project()
        {
            var projectRepositoryMock = new Mock<IProjectRepository>();
            var projectAuthorizationServiceMock = new Mock<IProjectAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new UpdateProjectHandler(
                projectAuthorizationServiceMock.Object,
                unitOfWorkMock.Object,
                projectRepositoryMock.Object);

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
            typeof(Project)
    .GetProperty(nameof(Project.Id))!
    .SetValue(project, 10);

            projectRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(project);

            projectAuthorizationServiceMock
                .Setup(x => x.EnsureCanManageProjectAsync(10))
                .ThrowsAsync(new UnauthorizedException("Not Authorized"));

            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<UnauthorizedException>();

            projectRepositoryMock.Verify(x => x.GetByIdAsync(10), Times.Once);
            projectAuthorizationServiceMock.Verify(
                x => x.EnsureCanManageProjectAsync(10),
                Times.Once);

            projectRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<Project>()),
                Times.Never);
        }


        [Fact]
        public async Task Should_Throw_ConflictException_When_Project_Name_Already_Exists()
        {
            var projectRepositoryMock = new Mock<IProjectRepository>();
            var projectAuthorizationServiceMock = new Mock<IProjectAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new UpdateProjectHandler(
                projectAuthorizationServiceMock.Object,
                unitOfWorkMock.Object,
                projectRepositoryMock.Object);

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
            typeof(Project)
    .GetProperty(nameof(Project.Id))!
    .SetValue(project, 10);

            projectRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(project);

            projectAuthorizationServiceMock
                .Setup(x => x.EnsureCanManageProjectAsync(10))
                .Returns(Task.CompletedTask);

            projectRepositoryMock
                .Setup(x => x.ExistsInWorkspaceAsync(5, "Updated Project"))
                .ReturnsAsync(true);

            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<ConflictException>();

            projectRepositoryMock.Verify(x => x.GetByIdAsync(10), Times.Once);
            projectAuthorizationServiceMock.Verify(
                x => x.EnsureCanManageProjectAsync(10),
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