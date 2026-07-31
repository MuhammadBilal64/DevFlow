using DevFlow.Application.Workflows.EnableWorkflow;
using FluentValidation.TestHelper;
using Xunit;

namespace DevFlow.UnitTests.Application.Workflows.EnableWorkflow
{
    public class EnableWorkflowValidatorTests
    {
        private readonly EnableWorkflowValidator _validator = new();


        [Fact]
        public void Should_Not_Have_Error_When_Command_Is_Valid()
        {
            var command = new EnableWorkflowCommand
            {
                ProjectId = 1,
                WorkflowId = 1
            };


            var result = _validator.TestValidate(command);


            result.ShouldNotHaveAnyValidationErrors();
        }



        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_WorkflowId_Is_Invalid(int workflowId)
        {
            var command = new EnableWorkflowCommand
            {
                ProjectId = 1,
                WorkflowId = workflowId
            };


            var result = _validator.TestValidate(command);


            result.ShouldHaveValidationErrorFor(x => x.WorkflowId);
        }



        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_ProjectId_Is_Invalid(int projectId)
        {
            var command = new EnableWorkflowCommand
            {
                ProjectId = projectId,
                WorkflowId = 1
            };


            var result = _validator.TestValidate(command);


            result.ShouldHaveValidationErrorFor(x => x.ProjectId);
        }
    }
}