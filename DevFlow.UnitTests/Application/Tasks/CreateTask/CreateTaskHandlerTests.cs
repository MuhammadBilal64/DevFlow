using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Tasks.CreateTask;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Tasks.CreateTask
{
    public class CreateTaskHandlerTests
    {
        [Fact]
        public async Task Should_Create_Task_Successfully()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var taskRepositoryMock = new Mock<ITaskRepository>();
            var projectRepositoryMock = new Mock<IProjectRepository>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new CreateTaskHandler(
                workspaceAuthorizationServiceMock.Object,
                currentUserServiceMock.Object,
                projectRepositoryMock.Object,
                unitOfWorkMock.Object,
                taskRepositoryMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var command = new CreateTaskCommand
            {
                ProjectId = 10,
                Title = "Implement JWT",
                Description = "Complete authentication module",
                Priority = TaskPriority.High,
                DueDate = DateTime.UtcNow.AddDays(5)
            };

            var project = new Project(
                "DevFlow",
                "Project Description",
                5,
                1);

            typeof(Project)
    .GetProperty(nameof(Project.Id))!
    .SetValue(project, 10);


            projectRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(project);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();

            result.Title.Should().Be("Implement JWT");
            result.Description.Should().Be("Complete authentication module");

            // Verifies
            projectRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);

            workspaceAuthorizationServiceMock.Verify(
                x => x.EnsureWorkspaceMemberAsync(5, 1),
                Times.Once);

            taskRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<TaskItem>()),
                Times.Once);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task Should_Throw_NotFoundException_When_Project_Does_Not_Exist()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var taskRepositoryMock = new Mock<ITaskRepository>();
            var projectRepositoryMock = new Mock<IProjectRepository>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new CreateTaskHandler(
                workspaceAuthorizationServiceMock.Object,
                currentUserServiceMock.Object,
                projectRepositoryMock.Object,
                unitOfWorkMock.Object,
                taskRepositoryMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var command = new CreateTaskCommand
            {
                ProjectId = 10,
                Title = "Implement JWT",
                Description = "Complete authentication module",
                Priority = TaskPriority.High,
                DueDate = DateTime.UtcNow.AddDays(5)
            };

            projectRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync((Project?)null);

            // Act
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();

            // Verifies
            projectRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);

            workspaceAuthorizationServiceMock.Verify(
                x => x.EnsureWorkspaceMemberAsync(It.IsAny<int>(), It.IsAny<int>()),
                Times.Never);

            taskRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<TaskItem>()),
                Times.Never);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task Should_Throw_UnauthorizedException_When_User_Is_Not_A_Workspace_Member()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var taskRepositoryMock = new Mock<ITaskRepository>();
            var projectRepositoryMock = new Mock<IProjectRepository>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new CreateTaskHandler(
                workspaceAuthorizationServiceMock.Object,
                currentUserServiceMock.Object,
                projectRepositoryMock.Object,
                unitOfWorkMock.Object,
                taskRepositoryMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var command = new CreateTaskCommand
            {
                ProjectId = 10,
                Title = "Implement JWT",
                Description = "Complete authentication module",
                Priority = TaskPriority.High,
                DueDate = DateTime.UtcNow.AddDays(5)
            };

            var project = new Project(
                "DevFlow",
                "Project Description",
                5,
                1);

            projectRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(project);

            workspaceAuthorizationServiceMock
                .Setup(x => x.EnsureWorkspaceMemberAsync(5, 1))
                .ThrowsAsync(new UnauthorizedException("Not a workspace member"));

            // Act
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedException>();

            // Verifies
            projectRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);

            workspaceAuthorizationServiceMock.Verify(
                x => x.EnsureWorkspaceMemberAsync(5, 1),
                Times.Once);

            taskRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<TaskItem>()),
                Times.Never);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }
    }
}