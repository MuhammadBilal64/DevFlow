using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Xunit;

namespace DevFlow.UnitTests.Domain.Workflows
{
    public class WorkflowConditionTests
    {
        [Fact]
        public void Should_Create_WorkflowCondition_With_Valid_Data()
        {
            // Arrange
            var field = "Priority";
            var workflowOperator = WorkflowOperator.Equals;
            var value = "High";

            // Act
            var condition = new WorkflowCondition(
                field,
                workflowOperator,
                value);

            // Assert
            condition.Field.Should().Be(field);
            condition.Operator.Should().Be(workflowOperator);
            condition.Value.Should().Be(value);
        }
        [Fact]
        public void Should_Throw_When_Field_Is_Empty()
        {
            // Arrange
            var field = "";

            // Act
            Action act = () => new WorkflowCondition(
                field,
                WorkflowOperator.Equals,
                "High");

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("Field cannot be empty.*")
                .And.ParamName.Should().Be("field");
        }
        [Fact]
        public void Should_Throw_When_Value_Is_Empty()
        {
            // Arrange
            var value = "";

            // Act
            Action act = () => new WorkflowCondition(
                "Priority",
                WorkflowOperator.Equals,
                value);

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("Value cannot be empty.*")
                .And.ParamName.Should().Be("value");
        }

    }
}
