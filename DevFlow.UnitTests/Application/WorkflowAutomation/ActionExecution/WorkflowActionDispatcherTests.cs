using DevFlow.Application.Common.Models;
using DevFlow.Application.Workflows.ActionExecution;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.WorkflowAutomation.ActionExecution
{
    public class WorkflowActionDispatcherTests
    {
        [Fact]
        public async Task Should_Execute_Correct_Action_Executor()
        {
            // Arrange

            var actionExecutorMock =
                new Mock<IActionExecutor>();

            actionExecutorMock
                .Setup(x => x.Type)
                .Returns(WorkflowActionType.NotifyUser);

            actionExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<WorkflowAction>(),
                    It.IsAny<WorkflowExecutionContext>()))
                .Returns(Task.CompletedTask);

            var executors =
                new List<IActionExecutor>();

            executors.Add(actionExecutorMock.Object);

            var dispatcher =
                new WorkflowActionDispatcher(executors);

            var action =
                new WorkflowAction(
                    WorkflowActionType.NotifyUser,
                    "dummy",
                    1);

            var values =
                new Dictionary<string, object?>();

            var context =
                new WorkflowExecutionContext(values);

            // Act

            await dispatcher.ExecuteAsync(
                action,
                context);

            // Assert

            actionExecutorMock.Verify(
                x => x.ExecuteAsync(
                    action,
                    context),
                Times.Once);
        }

        [Fact]
        public async Task Should_Throw_When_No_Action_Executor_Is_Registered()
        {
            // Arrange

            var actionExecutorMock =
                new Mock<IActionExecutor>();

            actionExecutorMock
                .Setup(x => x.Type)
                .Returns(WorkflowActionType.SendEmail);

            var executors =
                new List<IActionExecutor>();

            executors.Add(actionExecutorMock.Object);

            var dispatcher =
                new WorkflowActionDispatcher(executors);

            var action =
                new WorkflowAction(
                    /* Use an action type different from NotifyUser */
                    WorkflowActionType.NotifyUser,
                    "dummy",
                    1);

            var context =
                new WorkflowExecutionContext(
                    new Dictionary<string, object?>());

            // Act

            Func<Task> act = () =>
                dispatcher.ExecuteAsync(
                    action,
                    context);

            // Assert

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage(
                    $"No executor registered for action '{action.ActionType}'.");

            actionExecutorMock.Verify(
                x => x.ExecuteAsync(
                    It.IsAny<WorkflowAction>(),
                    It.IsAny<WorkflowExecutionContext>()),
                Times.Never);
        }

    }
}
