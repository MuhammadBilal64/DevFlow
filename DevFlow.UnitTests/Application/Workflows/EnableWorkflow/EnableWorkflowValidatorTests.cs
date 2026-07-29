using DevFlow.Application.Workflows.EnableWorkflow;
using FluentValidation.TestHelper;
using Xunit;

namespace DevFlow.UnitTests.Application.Workflows.EnableWorkflow
{
    public class EnableWorkflowValidatorTests
    {
        private readonly EnableWorkflowValidator _validator = new();

        [Fact]
        public void Should_Not_Have_Error_When_WorkflowId_Is_Valid()
        {
            // Arrange
            var command = new EnableWorkflowCommand
            {
                WorkflowId = 1
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_WorkflowId_Is_Invalid(int workflowId)
        {
            // Arrange
            var command = new EnableWorkflowCommand
            {
                WorkflowId = workflowId
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.WorkflowId);
        }
    }
}