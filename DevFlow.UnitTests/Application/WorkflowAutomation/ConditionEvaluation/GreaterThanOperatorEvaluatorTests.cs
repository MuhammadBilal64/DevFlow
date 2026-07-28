using DevFlow.Application.Workflows.ConditionEvaluation;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.WorkflowAutomation.ConditionEvaluation
{
    public class GreaterThanOperatorEvaluatorTests
    {
        [Fact]
        public void Should_Return_GreaterThan_Operator()
        {
            // Arrange
            var converterMock = new Mock<IWorkflowValueConverter>();

            var evaluator = new GreaterThanOperatorEvaluator(
                converterMock.Object);

            // Assert
            evaluator.Operator.Should().Be(WorkflowOperator.GreaterThan);
        }
        [Fact]
        public void Should_Return_True_When_Actual_Value_Is_Greater_Than_Expected_Value()
        {
            // Arrange
            var converterMock = new Mock<IWorkflowValueConverter>();

            var evaluator = new GreaterThanOperatorEvaluator(
                converterMock.Object);

            converterMock
                .Setup(x => x.ConvertToActualType(10, "5"))
                .Returns(5);

            // Act
            var result = evaluator.Evaluate(10, "5");

            // Assert
            result.Should().BeTrue();

            converterMock.Verify(
                x => x.ConvertToActualType(10, "5"),
                Times.Once);
        }
        [Fact]
        public void Should_Return_False_When_Actual_Value_Is_Not_Greater_Than_Expected_Value()
        {
            // Arrange
            var converterMock = new Mock<IWorkflowValueConverter>();

            var evaluator = new GreaterThanOperatorEvaluator(
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

            var evaluator = new GreaterThanOperatorEvaluator(
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
