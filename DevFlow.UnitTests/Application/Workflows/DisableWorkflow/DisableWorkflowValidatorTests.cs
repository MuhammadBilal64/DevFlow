using DevFlow.Application.Workflows.DisableWorkflow;
using FluentValidation.TestHelper;
using Xunit;

namespace DevFlow.UnitTests.Application.Workflows.DisableWorkflow
{
    public class DisableWorkflowValidatorTests
    {
        private readonly DisableWorkflowValidator _validator = new();

        [Fact]
        public void Should_Not_Have_Error_When_Command_Is_Valid()
        {
            // Arrange
            var command = new DisableWorkflowCommand
            {
                ProjectId = 1,
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
            var command = new DisableWorkflowCommand
            {
                ProjectId = 1,
                WorkflowId = workflowId
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.WorkflowId);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_ProjectId_Is_Invalid(int projectId)
        {
            // Arrange
            var command = new DisableWorkflowCommand
            {
                ProjectId = projectId,
                WorkflowId = 1
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProjectId);
        }
    }
}