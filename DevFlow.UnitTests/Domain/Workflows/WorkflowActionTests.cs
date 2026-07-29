using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Xunit;

namespace DevFlow.UnitTests.Domain.Workflows
{
    public class WorkflowActionTests
    {
        [Fact]
        public void Should_Create_WorkflowAction_With_Valid_Data()
        {
            // Arrange
            var actionType = WorkflowActionType.NotifyUser;
            var parameters = "Notify Assignee";
            var order = 1;

            // Act
            var action = new WorkflowAction(
                actionType,
                parameters,
                order);

            // Assert
            action.ActionType.Should().Be(actionType);
            action.Parameters.Should().Be(parameters);
            action.Order.Should().Be(order);
        }
        [Fact]
        public void Should_Throw_When_Parameters_Are_Empty()
        {
            // Arrange
            var parameters = "";

            // Act
            Action act = () => new WorkflowAction(
                WorkflowActionType.NotifyUser,
                parameters,
                1);

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("Parameters cannot be empty*")
                .And.ParamName.Should().Be("Parameters_");
        }
        [Fact]
        public void Should_Throw_When_Order_Is_Less_Than_Or_Equal_To_Zero()
        {
            // Arrange
            var order = 0;

            // Act
            Action act = () => new WorkflowAction(
                WorkflowActionType.NotifyUser,
                "Notify Assignee",
                order);

            // Assert
            act.Should()
                .Throw<ArgumentOutOfRangeException>()
                .WithMessage("*Order must be greater than zero.*")
                .And.ParamName.Should().Be("order");
        }

    }
}
