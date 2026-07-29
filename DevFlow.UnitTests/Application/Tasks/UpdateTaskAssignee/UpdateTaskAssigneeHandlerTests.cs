using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Tasks.UpdateTaskAssignee;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Tasks.UpdateTaskAssignee
{
    public class UpdateTaskAssigneeHandlerTests
    {
        [Fact]
        public async Task Should_Assign_Task_Successfully()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var taskRepositoryMock = new Mock<ITaskRepository>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new UpdateTaskAssigneeHandler(
                unitOfWorkMock.Object,
                taskRepositoryMock.Object,
                currentUserServiceMock.Object,
                workspaceAuthorizationServiceMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var command = new UpdateTaskAssigneeCommand
            {
                TaskId = 10,
                NewAssigneeId = 5
            };

            var project = new Project(
                "DevFlow",
                "Project Description",
                5,
                1);
            typeof(Project)
    .GetProperty(nameof(Project.Id))!
    .SetValue(project, 10);

            var task = new TaskItem(
                "Implement JWT",
                "Task Description",
                project.Id,
                1,
                DateTime.UtcNow.AddDays(2),
                TaskPriority.High);

            typeof(TaskItem)
                .GetProperty(nameof(TaskItem.Project))!
                .SetValue(task, project);

            taskRepositoryMock
                .Setup(x => x.GetByIdForAdminAsync(10, 1))
                .ReturnsAsync(task);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(task.Id);
            result.Title.Should().Be(task.Title);

            taskRepositoryMock.Verify(
                x => x.GetByIdForAdminAsync(10, 1),
                Times.Once);

            workspaceAuthorizationServiceMock.Verify(
                x => x.EnsureWorkspaceMemberAsync(project.WorkspaceId, 5),
                Times.Once);

            taskRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<TaskItem>()),
                Times.Once);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task Should_Unassign_Task_Successfully()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var taskRepositoryMock = new Mock<ITaskRepository>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new UpdateTaskAssigneeHandler(
                unitOfWorkMock.Object,
                taskRepositoryMock.Object,
                currentUserServiceMock.Object,
                workspaceAuthorizationServiceMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var command = new UpdateTaskAssigneeCommand
            {
                TaskId = 10,
                NewAssigneeId = null
            };

            var project = new Project(
                "DevFlow",
                "Project Description",
                5,
                1);
            typeof(Project)
    .GetProperty(nameof(Project.Id))!
    .SetValue(project, 10);
            var task = new TaskItem(
                "Implement JWT",
                "Task Description",
                project.Id,
                1,
                DateTime.UtcNow.AddDays(2),
                TaskPriority.High);

            typeof(TaskItem)
                .GetProperty(nameof(TaskItem.Project))!
                .SetValue(task, project);

            task.Assign(5);

            taskRepositoryMock
                .Setup(x => x.GetByIdForAdminAsync(10, 1))
                .ReturnsAsync(task);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();

            workspaceAuthorizationServiceMock.Verify(
                x => x.EnsureWorkspaceMemberAsync(It.IsAny<int>(), It.IsAny<int>()),
                Times.Never);

            taskRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<TaskItem>()),
                Times.Once);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task Should_Throw_NotFoundException_When_Task_Does_Not_Exist()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var taskRepositoryMock = new Mock<ITaskRepository>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new UpdateTaskAssigneeHandler(
                unitOfWorkMock.Object,
                taskRepositoryMock.Object,
                currentUserServiceMock.Object,
                workspaceAuthorizationServiceMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var command = new UpdateTaskAssigneeCommand
            {
                TaskId = 10,
                NewAssigneeId = 5
            };

            taskRepositoryMock
                .Setup(x => x.GetByIdForAdminAsync(10, 1))
                .ReturnsAsync((TaskItem?)null);

            // Act
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();

            taskRepositoryMock.Verify(
                x => x.GetByIdForAdminAsync(10, 1),
                Times.Once);

            taskRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<TaskItem>()),
                Times.Never);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task Should_Throw_UnauthorizedException_When_New_Assignee_Is_Not_Workspace_Member()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var taskRepositoryMock = new Mock<ITaskRepository>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new UpdateTaskAssigneeHandler(
                unitOfWorkMock.Object,
                taskRepositoryMock.Object,
                currentUserServiceMock.Object,
                workspaceAuthorizationServiceMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var command = new UpdateTaskAssigneeCommand
            {
                TaskId = 10,
                NewAssigneeId = 5
            };

            var project = new Project(
                "DevFlow",
                "Project Description",
                5,
                1);
            typeof(Project)
    .GetProperty(nameof(Project.Id))!
    .SetValue(project, 10);

            var task = new TaskItem(
                "Implement JWT",
                "Task Description",
                project.Id,
                1,
                DateTime.UtcNow.AddDays(2),
                TaskPriority.High);

            typeof(TaskItem)
                .GetProperty(nameof(TaskItem.Project))!
                .SetValue(task, project);

            taskRepositoryMock
                .Setup(x => x.GetByIdForAdminAsync(10, 1))
                .ReturnsAsync(task);

            workspaceAuthorizationServiceMock
                .Setup(x => x.EnsureWorkspaceMemberAsync(project.WorkspaceId, 5))
                .ThrowsAsync(new UnauthorizedException("Not a workspace member"));

            // Act
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedException>();

            taskRepositoryMock.Verify(
                x => x.GetByIdForAdminAsync(10, 1),
                Times.Once);

            workspaceAuthorizationServiceMock.Verify(
                x => x.EnsureWorkspaceMemberAsync(project.WorkspaceId, 5),
                Times.Once);

            taskRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<TaskItem>()),
                Times.Never);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }
    }
}