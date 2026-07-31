using DevFlow.Application.Workflows.GetAllWorkflows;
using DevFlow.Domain.Enum;
using FluentValidation.TestHelper;
using Xunit;

namespace DevFlow.UnitTests.Application.Workflows.GetAllWorkflows
{
    public class GetAllWorkflowValidatorTests
    {
        private readonly GetWorkflowsByProjectValidator _validator = new();


        [Fact]
        public void Should_Not_Have_Error_When_Query_Is_Valid()
        {
            // Arrange
            var query = new GetWorkflowsByProjectQuery
            {
                ProjectId = 1,
                PageNumber = 1,
                PageSize = 10,
                Trigger = WorkflowTrigger.TaskAssigned
            };


            // Act
            var result = _validator.TestValidate(query);


            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }



        [Theory]
        [InlineData((WorkflowTrigger)100)]
        [InlineData((WorkflowTrigger)(-1))]
        public void Should_Have_Error_When_Trigger_Is_Invalid(
            WorkflowTrigger trigger)
        {
            // Arrange
            var query = new GetWorkflowsByProjectQuery
            {
                ProjectId = 1,
                PageNumber = 1,
                PageSize = 10,
                Trigger = trigger
            };


            // Act
            var result = _validator.TestValidate(query);


            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Trigger);
        }



        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_ProjectId_Is_Invalid(
            int projectId)
        {
            // Arrange
            var query = new GetWorkflowsByProjectQuery
            {
                ProjectId = projectId,
                PageNumber = 1,
                PageSize = 10
            };


            // Act
            var result = _validator.TestValidate(query);


            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProjectId);
        }
    }
}