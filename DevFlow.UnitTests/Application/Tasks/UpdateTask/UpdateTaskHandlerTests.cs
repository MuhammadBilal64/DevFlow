using DevFlow.Application.Abstractions;
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
            var taskRepositoryMock = new Mock<ITaskRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var projectAuthorizationServiceMock = new Mock<IProjectAuthorizationService>();

            var handler = new UpdateTaskHandler(
                unitOfWorkMock.Object,
                taskRepositoryMock.Object,
                projectAuthorizationServiceMock.Object);

            var command = new UpdateTaskCommand
            {
                TaskId = 10,
                ProjectId = 5,
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
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(task);

            projectAuthorizationServiceMock
                .Setup(x => x.EnsureProjectMemberAsync(5))
                .Returns(Task.CompletedTask);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();

            result.Id.Should().Be(task.Id);
            result.Title.Should().Be(command.Title);
            result.Description.Should().Be(command.Description);

            taskRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);

            projectAuthorizationServiceMock.Verify(
                x => x.EnsureProjectMemberAsync(5),
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
            var taskRepositoryMock = new Mock<ITaskRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var projectAuthorizationServiceMock = new Mock<IProjectAuthorizationService>();

            var handler = new UpdateTaskHandler(
                unitOfWorkMock.Object,
                taskRepositoryMock.Object,
                projectAuthorizationServiceMock.Object);

            var command = new UpdateTaskCommand
            {
                TaskId = 10,
                ProjectId = 5,
                Title = "Updated Task",
                Description = "Updated Description",
                Priority = TaskPriority.High,
                DueDate = DateTime.UtcNow.AddDays(5)
            };

            taskRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync((TaskItem?)null);

            // Act
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<NotFoundException>();

            taskRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);

            projectAuthorizationServiceMock.Verify(
                x => x.EnsureProjectMemberAsync(It.IsAny<int>()),
                Times.Never);

            taskRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<TaskItem>()),
                Times.Never);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }
    }
}