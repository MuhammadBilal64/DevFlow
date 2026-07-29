using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Models;
using DevFlow.Application.Workflows.ActionExecution;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.WorkflowAutomation.ActionExecution
{
    public class NotifyUserActionExecutorTests
    {
        // Should execute notification action successfully for assignee
        [Fact]
        public async Task Should_Send_Notification_To_Assignee_When_Action_Is_Valid()
        {
            // Arrange

            var notificationServiceMock =
                new Mock<INotificationService>();

            var executor =
                   new NotifyUserActionExecutor(
                       notificationServiceMock.Object);

            var action =
      new WorkflowAction(
          WorkflowActionType.NotifyUser,
          """
            {
                "recipient": "Assignee",
                "message": "Task completed"
            }
            """,
          1);

            var context =
            new WorkflowExecutionContext(
                new Dictionary<string, object?>
                {
                    ["AssigneeId"] = 10
                });


            // Act

            await executor.ExecuteAsync(
                action,
                context);
            // Assert

            notificationServiceMock.Verify(
                x => x.NotifyAsync(
                    10,
                    "Task completed",
                    NotificationType.Workflow,
                    null),
                Times.Once);

        }



        // Should execute notification action successfully for reporter
        [Fact]
        public async Task Should_Send_Notification_To_Reporter_When_Action_Is_Valid()
        {
            // Arrange

            var notificationServiceMock =
                new Mock<INotificationService>();

            var executor =
                new NotifyUserActionExecutor(
                    notificationServiceMock.Object);


            var action =
                new WorkflowAction(
                    WorkflowActionType.NotifyUser,
                    """
            {
                "recipient": "Reporter",
                "message": "Your task was updated"
            }
            """,
                    1);


            var context =
                new WorkflowExecutionContext(
                    new Dictionary<string, object?>
                    {
                        ["ReporterId"] = 20
                    });


            // Act

            await executor.ExecuteAsync(
                action,
                context);


            // Assert

            notificationServiceMock.Verify(
                x => x.NotifyAsync(
                    20,
                    "Your task was updated",
                    NotificationType.Workflow,
                    null),
                Times.Once);
        }




        // Should throw when action parameters contain invalid JSON
        [Fact]
        public async Task Should_Throw_When_Action_Parameters_Contain_Invalid_Json()
        {
            // Arrange

            var notificationServiceMock =
                new Mock<INotificationService>();

            var executor =
                new NotifyUserActionExecutor(
                    notificationServiceMock.Object);


            var action =
                new WorkflowAction(
                    WorkflowActionType.NotifyUser,
                    """
            {
                "recipient": "Assignee",
                "message":
            """,
                    1);


            var context =
                new WorkflowExecutionContext(
                    new Dictionary<string, object?>
                    {
                        ["AssigneeId"] = 10
                    });


            // Act

            Func<Task> act = () =>
                executor.ExecuteAsync(
                    action,
                    context);


            // Assert

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage(
                    "NotifyUser action parameters contain invalid JSON.");


            notificationServiceMock.Verify(
                x => x.NotifyAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<NotificationType>(),
                    It.IsAny<int?>()),
                Times.Never);
        }

        // Should throw when notification message is empty
        [Fact]
        public async Task Should_Throw_When_Message_Is_Empty()
        {
            // Arrange

            var notificationServiceMock =
                new Mock<INotificationService>();

            var executor =
                new NotifyUserActionExecutor(
                    notificationServiceMock.Object);


            var action =
                new WorkflowAction(
                    WorkflowActionType.NotifyUser,
                    """
            {
                "recipient": "Assignee",
                "message": ""
            }
            """,
                    1);


            var context =
                new WorkflowExecutionContext(
                    new Dictionary<string, object?>
                    {
                        ["AssigneeId"] = 10
                    });


            // Act

            Func<Task> act = () =>
                executor.ExecuteAsync(
                    action,
                    context);


            // Assert

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage(
                    "NotifyUser action requires a non-empty message.");


            notificationServiceMock.Verify(
                x => x.NotifyAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<NotificationType>(),
                    It.IsAny<int?>()),
                Times.Never);
        }


        // Should throw when recipient id is missing from workflow context
        [Fact]
        public async Task Should_Throw_When_Recipient_Id_Is_Missing_From_Context()
        {
            // Arrange

            var notificationServiceMock =
                new Mock<INotificationService>();

            var executor =
                new NotifyUserActionExecutor(
                    notificationServiceMock.Object);


            var action =
                new WorkflowAction(
                    WorkflowActionType.NotifyUser,
                    """
            {
                "recipient": "Assignee",
                "message": "Task completed"
            }
            """,
                    1);


            var context =
                new WorkflowExecutionContext(
                    new Dictionary<string, object?>());


            // Act

            Func<Task> act = () =>
                executor.ExecuteAsync(
                    action,
                    context);


            // Assert

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage(
                    "Workflow context does not contain a valid 'AssigneeId'.");


            notificationServiceMock.Verify(
                x => x.NotifyAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<NotificationType>(),
                    It.IsAny<int?>()),
                Times.Never);
        }




        // Should expose NotifyUser as executor type
        [Fact]
        public void Should_Return_NotifyUser_As_Action_Type()
        {
            // Arrange

            var notificationServiceMock =
                new Mock<INotificationService>();

            var executor =
                new NotifyUserActionExecutor(
                    notificationServiceMock.Object);


            // Act

            var result = executor.Type;


            // Assert

            result.Should()
                .Be(WorkflowActionType.NotifyUser);
        }
    }
}