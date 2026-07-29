using DevFlow.Application.Workflows.ConditionEvaluation;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.WorkflowAutomation.ConditionEvaluation
{
    public class ContainsOperatorEvaluatorTests
    {
        [Fact]
        public void Should_Return_Contains_Operator()
        {
            // Arrange
            var converterMock = new Mock<IWorkflowValueConverter>();
            var evaluator = new ContainsOperatorEvaluator(
              converterMock.Object);

            // Assert
            evaluator.Operator.Should().Be(WorkflowOperator.Contains);

        }
        [Fact]
        public void Should_Return_True_When_Actual_Value_Contains_Expected_Value()
        {
            // Arrange
            var converterMock = new Mock<IWorkflowValueConverter>();

            var evaluator = new ContainsOperatorEvaluator(
                converterMock.Object);

            // Act
            var result = evaluator.Evaluate(
                "Hello World",
                "World");

            // Assert
            result.Should().BeTrue();
        }
        [Fact]
        public void Should_Return_True_When_Contains_Is_Case_Insensitive()
        {
            // Arrange
            var converterMock = new Mock<IWorkflowValueConverter>();

            var evaluator = new ContainsOperatorEvaluator(
                converterMock.Object);

            // Act
            var result = evaluator.Evaluate(
                "Hello World",
                "world");

            // Assert
            result.Should().BeTrue();
        }
        [Fact]
        public void Should_Return_False_When_Actual_Value_Does_Not_Contain_Expected_Value()
        {
            // Arrange
            var converterMock = new Mock<IWorkflowValueConverter>();

            var evaluator = new ContainsOperatorEvaluator(
                converterMock.Object);

            // Act
            var result = evaluator.Evaluate(
                "Hello World",
                "Test");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Should_Return_False_When_Actual_Value_Is_Null()
        {
            // Arrange
            var converterMock = new Mock<IWorkflowValueConverter>();

            var evaluator = new ContainsOperatorEvaluator(
                converterMock.Object);

            // Act
            var result = evaluator.Evaluate(
                null,
                "World");

            // Assert
            result.Should().BeFalse();
        }

    }
}
