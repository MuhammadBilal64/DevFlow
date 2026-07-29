using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Tasks.UpdateTaskStatus;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Moq;
using Xunit;
using TaskStatusEnum = DevFlow.Domain.Enum.TaskStatus;
namespace DevFlow.UnitTests.Application.Tasks.UpdateTaskStatus
{
    public class UpdateTaskStatusHandlerTests
    {
        [Fact]
        public async Task Should_Update_Task_Status_Successfully()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var taskRepositoryMock = new Mock<ITaskRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();

            var handler = new UpdateTaskStatusHandler(
                currentUserServiceMock.Object,
                taskRepositoryMock.Object,
                unitOfWorkMock.Object,
                workspaceAuthorizationServiceMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var command = new UpdateTaskStatusCommand
            {
                TaskId = 10,
                TaskStatus = TaskStatusEnum.Completed
            };

            var task = new TaskItem(
                "Implement JWT",
                "Task Description",
                5,
                1,
                DateTime.UtcNow.AddDays(2),
                TaskPriority.High);

            taskRepositoryMock
                .Setup(x => x.GetByIdForStatusUpdateAsync(10, 1))
                .ReturnsAsync(task);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(task.Id);
            result.Status.Should().Be(TaskStatusEnum.Completed);

            taskRepositoryMock.Verify(
                x => x.GetByIdForStatusUpdateAsync(10, 1),
                Times.Once);

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
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();

            var handler = new UpdateTaskStatusHandler(
                currentUserServiceMock.Object,
                taskRepositoryMock.Object,
                unitOfWorkMock.Object,
                workspaceAuthorizationServiceMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var command = new UpdateTaskStatusCommand
            {
                TaskId = 10,
                TaskStatus = TaskStatusEnum.Completed
            };

            taskRepositoryMock
                .Setup(x => x.GetByIdForStatusUpdateAsync(10, 1))
                .ReturnsAsync((TaskItem?)null);

            // Act
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();

            taskRepositoryMock.Verify(
                x => x.GetByIdForStatusUpdateAsync(10, 1),
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