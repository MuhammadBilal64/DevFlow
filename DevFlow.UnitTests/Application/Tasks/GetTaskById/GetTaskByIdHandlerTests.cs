using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Tasks.GetTaskById;
using DevFlow.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Tasks.GetTaskById
{
    public class GetTaskByIdHandlerTests
    {
        [Fact]
        public async Task Should_Return_Task_By_Id()
        {

            // Arrange
            var taskRepositoryMock = new Mock<ITaskRepository>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();

            var handler = new GetTaskByIdHandler(
                taskRepositoryMock.Object,
                workspaceAuthorizationServiceMock.Object);
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
                DevFlow.Domain.Enum.TaskPriority.High);

            typeof(TaskItem)
                .GetProperty(nameof(TaskItem.Project))!
                .SetValue(task, project);

            taskRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(task);

            var query = new GetTaskByIdQuery
            {
                TaskId = 10
            };
            //Act
            var result = await handler.Handle(query, CancellationToken.None);
            //Assert
            result.Should().NotBeNull();
            result.TaskId.Should().Be(task.Id);
            result.Title.Should().Be(task.Title);
            result.Description.Should().Be(task.Description);

            // Verifies
            taskRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);

            workspaceAuthorizationServiceMock.Verify(
                x => x.EnsureWorkspaceMemberAsync(project.WorkspaceId),
                Times.Once);



        }

        [Fact]
        public async Task Should_Throw_NotFoundException_When_Task_Does_Not_Exist()
        {
            // Arrange
            var taskRepositoryMock = new Mock<ITaskRepository>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();

            var handler = new GetTaskByIdHandler(
                taskRepositoryMock.Object,
                workspaceAuthorizationServiceMock.Object);

            var query = new GetTaskByIdQuery
            {
                TaskId = 10
            };

            taskRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync((TaskItem?)null);

            // Act
            Func<Task> act = () => handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();

            // Verifies
            taskRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);

            workspaceAuthorizationServiceMock.Verify(
                x => x.EnsureWorkspaceMemberAsync(It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public async Task Should_Throw_UnauthorizedException_When_User_Is_Not_A_Workspace_Member()
        {
            // Arrange
            var taskRepositoryMock = new Mock<ITaskRepository>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();

            var handler = new GetTaskByIdHandler(
                taskRepositoryMock.Object,
                workspaceAuthorizationServiceMock.Object);

            var query = new GetTaskByIdQuery
            {
                TaskId = 10
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
                DevFlow.Domain.Enum.TaskPriority.High);

            typeof(TaskItem).GetProperty(nameof(TaskItem.Project))!.SetValue(task, project);


            taskRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(task);

            workspaceAuthorizationServiceMock
                .Setup(x => x.EnsureWorkspaceMemberAsync(project.WorkspaceId))
                .ThrowsAsync(new UnauthorizedException("Not a workspace member"));

            // Act
            Func<Task> act = () => handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedException>();

            // Verifies
            taskRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);

            workspaceAuthorizationServiceMock.Verify(
                x => x.EnsureWorkspaceMemberAsync(project.WorkspaceId),
                Times.Once);
        }
    }
}
