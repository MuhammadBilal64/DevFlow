using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Models;
using DevFlow.Application.DomainEvents.TaskCompleted;
using DevFlow.Domain.Enum;
using DevFlow.Domain.Events;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.DomainEvents.TaskCompleted
{
    public class TaskCompletedEventHandlerTests
    {
        [Fact]
        public async Task Should_Notify_User_And_Execute_Workflow_When_Task_Is_Completed()
        {
            // Arrange
            var workflowEngineMock = new Mock<IWorkflowEngine>();
            var notificationServiceMock = new Mock<INotificationService>();

            var handler = new TaskCompletedEventHandler(
                workflowEngineMock.Object,
                notificationServiceMock.Object);


            var domainEvent = new TaskCompletedEvent(
                userId: 5,
                taskId: 10,
                taskTitle: "Implement JWT");


            // Act
            await handler.Handle(
                domainEvent,
                CancellationToken.None);


            // Assert

            notificationServiceMock.Verify(
                x => x.NotifyAsync(
                    5,
                    "Task 'Implement JWT' has been completed.",
                    NotificationType.TaskCompleted,
                    10),
                Times.Once);


            workflowEngineMock.Verify(
                x => x.ExecuteAsync(
                    WorkflowTrigger.TaskCompleted,
                    It.IsAny<WorkflowExecutionContext>()),
                Times.Once);
        }
    }
}