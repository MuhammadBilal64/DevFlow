using DevFlow.Application.Workflows.GetWorkflowById;
using FluentValidation.TestHelper;
using Xunit;

namespace DevFlow.UnitTests.Application.Workflows.GetWorkflowById
{
    public class GetWorkflowByIdValidatorTests
    {
        private readonly GetWorkflowByIdValidator _validator = new();


        [Fact]
        public void Should_Not_Have_Error_When_Query_Is_Valid()
        {
            // Arrange
            var query = new GetWorkflowByIdQuery
            {
                ProjectId = 1,
                WorkflowId = 10
            };


            // Act
            var result = _validator.TestValidate(query);


            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }



        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_ProjectId_Is_Invalid(int projectId)
        {
            // Arrange
            var query = new GetWorkflowByIdQuery
            {
                ProjectId = projectId,
                WorkflowId = 10
            };


            // Act
            var result = _validator.TestValidate(query);


            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProjectId);
        }



        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_WorkflowId_Is_Invalid(int workflowId)
        {
            // Arrange
            var query = new GetWorkflowByIdQuery
            {
                ProjectId = 1,
                WorkflowId = workflowId
            };


            // Act
            var result = _validator.TestValidate(query);


            // Assert
            result.ShouldHaveValidationErrorFor(x => x.WorkflowId);
        }
    }
}