using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Models;
using DevFlow.Application.DomainEvents.TaskAssigned;
using DevFlow.Domain.Enum;
using DevFlow.Domain.Events;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.DomainEvents.TaskAssigned
{
    public class TaskAssignedEventHandlerTests
    {
        [Fact]
        public async Task Should_Notify_User_And_Execute_Workflow_When_Task_Is_Assigned()
        {
            // Arrange
            var notificationServiceMock = new Mock<INotificationService>();
            var workflowEngineMock = new Mock<IWorkflowEngine>();

            var handler = new TaskAssignedEventHandler(
                notificationServiceMock.Object,
                workflowEngineMock.Object);


            var domainEvent = new TaskAssignedEvent(
                5,
                10,
                "Implement JWT");


            // Act
            await handler.Handle(
                domainEvent,
                CancellationToken.None);


            // Assert

            notificationServiceMock.Verify(
                x => x.NotifyAsync(
                    5,
                    "You have been assigned task 'Implement JWT'.",
                    NotificationType.TaskAssigned,
                    10),
                Times.Once);

            workflowEngineMock.Verify(
                x => x.ExecuteAsync(
                    WorkflowTrigger.TaskAssigned,
                    It.IsAny<WorkflowExecutionContext>()),
                Times.Once);
        }
    }
}