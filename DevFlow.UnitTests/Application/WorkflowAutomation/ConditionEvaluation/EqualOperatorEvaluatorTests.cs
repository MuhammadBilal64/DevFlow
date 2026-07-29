using DevFlow.Application.Workflows.ConditionEvaluation;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.WorkflowAutomation.ConditionEvaluation
{
    public class EqualOperatorEvaluatorTests
    {
        [Fact]
        public void Should_Return_Equals_Operator()
        {
            // Arrange
            var converterMock = new Mock<IWorkflowValueConverter>();

            var evaluator = new EqualOperatorEvaluator(
                converterMock.Object);

            // Assert
            evaluator.Operator.Should().Be(WorkflowOperator.Equals);
        }
        [Fact]
        public void Should_Return_True_When_Values_Are_Equal()
        {
            // Arrange
            var converterMock = new Mock<IWorkflowValueConverter>();

            var evaluator = new EqualOperatorEvaluator(
                converterMock.Object);

            converterMock
                .Setup(x => x.ConvertToActualType(10, "10"))
                .Returns(10);

            // Act
            var result = evaluator.Evaluate(10, "10");

            // Assert
            result.Should().BeTrue();

            converterMock.Verify(
                x => x.ConvertToActualType(10, "10"),
                Times.Once);
        }
        [Fact]
        public void Should_Return_False_When_Values_Are_Not_Equal()
        {
            // Arrange
            var converterMock = new Mock<IWorkflowValueConverter>();

            var evaluator = new EqualOperatorEvaluator(
                converterMock.Object);

            converterMock
                .Setup(x => x.ConvertToActualType(10, "20"))
                .Returns(20);

            // Act
            var result = evaluator.Evaluate(10, "20");

            // Assert
            result.Should().BeFalse();

            converterMock.Verify(
                x => x.ConvertToActualType(10, "20"),
                Times.Once);
        }
        [Fact]
        public void Should_Throw_When_Actual_Value_Is_Null()
        {
            // Arrange
            var converterMock = new Mock<IWorkflowValueConverter>();

            var evaluator = new EqualOperatorEvaluator(
                converterMock.Object);

            // Act
            Action act = () => evaluator.Evaluate(
                null,
                "10");

            // Assert
            act.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("Actual value cannot be null.");

            converterMock.Verify(
                x => x.ConvertToActualType(
                    It.IsAny<object>(),
                    It.IsAny<string>()),
                Times.Never);
        }

    }
}
