using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Tasks.DeleteTask;
using DevFlow.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Tasks.DeleteTask
{
    public class DeleteTaskHandlerTests
    {
        [Fact]
        public async Task Should_Delete_Task_Successfully()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var taskRepositoryMock = new Mock<ITaskRepository>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new DeleteTaskHandler(
                currentUserServiceMock.Object,
                unitOfWorkMock.Object,
                taskRepositoryMock.Object,
                workspaceAuthorizationServiceMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var command = new DeleteTaskCommand
            {
                TaskId = 10
            };

            var task = new TaskItem(
                "Implement JWT",
                "Task Description",
                5,
                1,
                DateTime.UtcNow.AddDays(2),
                DevFlow.Domain.Enum.TaskPriority.High);

            taskRepositoryMock
                .Setup(x => x.GetByIdForAdminAsync(10, 1))
                .ReturnsAsync(task);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.TaskId.Should().Be(task.Id);
            result.Title.Should().Be(task.Title);

            // Verifies
            taskRepositoryMock.Verify(
                x => x.GetByIdForAdminAsync(10, 1),
                Times.Once);

            taskRepositoryMock.Verify(
                x => x.DeleteAsync(task),
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

            var handler = new DeleteTaskHandler(
                currentUserServiceMock.Object,
                unitOfWorkMock.Object,
                taskRepositoryMock.Object,
                workspaceAuthorizationServiceMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var command = new DeleteTaskCommand
            {
                TaskId = 10
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
                x => x.DeleteAsync(It.IsAny<TaskItem>()),
                Times.Never);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

    }
}
