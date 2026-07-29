using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Tasks.UpdateTask;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Tasks.UpdateTask
{
    public class UpdateTaskHandlerTests
    {
        [Fact]
        public async Task Should_Update_Task_Successfully()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var taskRepositoryMock = new Mock<ITaskRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new UpdateTaskHandler(
                currentUserServiceMock.Object,
                unitOfWorkMock.Object,
                taskRepositoryMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var command = new UpdateTaskCommand
            {
                TaskId = 10,
                Title = "Updated Task",
                Description = "Updated Description",
                Priority = TaskPriority.High,
                DueDate = DateTime.UtcNow.AddDays(5)
            };

            var task = new TaskItem(
                "Old Task",
                "Old Description",
                5,
                1,
                DateTime.UtcNow.AddDays(2),
                TaskPriority.Low);

            taskRepositoryMock
                .Setup(x => x.GetByIdForAdminAsync(10, 1))
                .ReturnsAsync(task);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();

            result.Id.Should().Be(task.Id);
            result.Title.Should().Be(command.Title);
            result.Description.Should().Be(command.Description);

            // Verifies
            taskRepositoryMock.Verify(
                x => x.GetByIdForAdminAsync(10, 1),
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

            var handler = new UpdateTaskHandler(
                currentUserServiceMock.Object,
                unitOfWorkMock.Object,
                taskRepositoryMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var command = new UpdateTaskCommand
            {
                TaskId = 10,
                Title = "Updated Task",
                Description = "Updated Description",
                Priority = TaskPriority.High,
                DueDate = DateTime.UtcNow.AddDays(5)
            };

            taskRepositoryMock
                .Setup(x => x.GetByIdForAdminAsync(10, 1))
                .ReturnsAsync((TaskItem?)null);

            // Act
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();

            // Verifies
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
    }
}