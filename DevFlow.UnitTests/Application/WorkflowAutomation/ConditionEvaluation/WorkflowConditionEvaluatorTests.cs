using DevFlow.Application.Common.Models;
using DevFlow.Application.Workflows.ConditionEvaluation;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.WorkflowAutomation.ConditionEvaluation
{
    public class WorkflowConditionEvaluatorTests
    {
        [Fact]
        public void Should_Return_Result_From_Correct_Operator_Evaluator()
        {
            // Arrange
            var operatorEvaluatorMock =
                new Mock<IConditionOperatorEvaluator>();

            operatorEvaluatorMock
       .Setup(x => x.Operator)
       .Returns(WorkflowOperator.Equals);

            operatorEvaluatorMock
        .Setup(x => x.Evaluate(
            It.IsAny<object?>(),
            It.IsAny<string>()))
        .Returns(true);

            // Create a list that will hold all operator evaluators
            var evaluators =
                new List<IConditionOperatorEvaluator>();

            // Add our fake evaluator into the list
            evaluators.Add(operatorEvaluatorMock.Object);

            // Pass the list into the constructor
            var workflowConditionEvaluator =
                new WorkflowConditionEvaluator(evaluators);



            // Create the workflow condition
            var condition =
                new WorkflowCondition(
                    "Priority",
                    WorkflowOperator.Equals,
                    "High");

            // Create values for the execution context
            var values =
                new Dictionary<string, object?>();


            values.Add("Priority", "High");
            // Create the execution context
            var context =
                new WorkflowExecutionContext(values);

            // Act
            var result =
                workflowConditionEvaluator.Evaluate(
                    condition,
                    context);
            // Assert
            result.Should().BeTrue();

            operatorEvaluatorMock.Verify(
                x => x.Evaluate(
                    "High",
                    "High"),
                Times.Once);



        }
        [Fact]

        public void Should_Throw_When_No_Evaluator_Is_Registered()
        {

            // Arrange

            // Create a fake operator evaluator
            var operatorEvaluatorMock =
                new Mock<IConditionOperatorEvaluator>();

            // Teach it that it supports only the Equals operator
            operatorEvaluatorMock
                .Setup(x => x.Operator)
                .Returns(WorkflowOperator.Equals);
            // Create a list of evaluators
            var evaluators =
                new List<IConditionOperatorEvaluator>();

            // Add the fake evaluator
            evaluators.Add(operatorEvaluatorMock.Object);

            // Create the WorkflowConditionEvaluator
            var workflowConditionEvaluator =
                new WorkflowConditionEvaluator(evaluators);

            // Create a condition that requires GreaterThan
            var condition =
                new WorkflowCondition(
                    "Priority",
                    WorkflowOperator.GreaterThan,
                    "10");
            // Create runtime values
            var values =
                new Dictionary<string, object?>();

            values.Add("Priority", 20);

            // Create execution context
            var context =
                new WorkflowExecutionContext(values);

            // Act
            Action act = () =>
                workflowConditionEvaluator.Evaluate(
                    condition,
                    context);

            // Assert
            act.Should()
                .Throw<InvalidOperationException>()
                .WithMessage(
                    "No evaluator registered for operator 'GreaterThan'.");

            // Ensure no evaluator was ever called
            operatorEvaluatorMock.Verify(
                x => x.Evaluate(
                    It.IsAny<object?>(),
                    It.IsAny<string>()),
                Times.Never);
        }
        [Fact]
        public void Should_Pass_Correct_Values_To_Operator_Evaluator()
        {
            // Arrange

            var operatorEvaluatorMock =
                new Mock<IConditionOperatorEvaluator>();

            operatorEvaluatorMock
                .Setup(x => x.Operator)
                .Returns(WorkflowOperator.Equals);

            operatorEvaluatorMock
                .Setup(x => x.Evaluate(
                    It.IsAny<object?>(),
                    It.IsAny<string>()))
                .Returns(true);

            var evaluators =
                new List<IConditionOperatorEvaluator>();

            evaluators.Add(operatorEvaluatorMock.Object);

            var workflowConditionEvaluator =
                new WorkflowConditionEvaluator(evaluators);

            var condition =
                new WorkflowCondition(
                    "Priority",
                    WorkflowOperator.Equals,
                    "High");

            var values =
                new Dictionary<string, object?>();

            values.Add("Priority", "Critical");

            var context =
                new WorkflowExecutionContext(values);

            // Act

            var result =
                workflowConditionEvaluator.Evaluate(
                    condition,
                    context);

            // Assert

            result.Should().BeTrue();

            operatorEvaluatorMock.Verify(
                x => x.Evaluate(
                    "Critical",
                    "High"),
                Times.Once);
        }



    }
}
