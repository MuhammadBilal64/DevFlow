using DevFlow.Application.Tasks.GetTaskById;
using FluentValidation.TestHelper;
using Xunit;

namespace DevFlow.UnitTests.Application.Tasks.GetTaskById
{
    public class GetTaskByIdValidatorTests
    {
        [Theory]
        [InlineData(1, 1)]
        [InlineData(10, 10)]
        public void Should_Not_Have_Validation_Error_When_Query_Is_Valid(
            int taskId,
            int projectId)
        {
            // Arrange
            var validator = new GetTaskByIdValidator();

            var query = new GetTaskByIdQuery
            {
                TaskId = taskId,
                ProjectId = projectId
            };


            // Act
            var result = validator.TestValidate(query);


            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.TaskId);
            result.ShouldNotHaveValidationErrorFor(x => x.ProjectId);
        }



        [Theory]
        [InlineData(0, 1)]
        [InlineData(-1, 1)]
        public void Should_Have_Validation_Error_When_TaskId_Is_Invalid(
            int taskId,
            int projectId)
        {
            // Arrange
            var validator = new GetTaskByIdValidator();

            var query = new GetTaskByIdQuery
            {
                TaskId = taskId,
                ProjectId = projectId
            };


            // Act
            var result = validator.TestValidate(query);


            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TaskId);
        }



        [Theory]
        [InlineData(1, 0)]
        [InlineData(1, -1)]
        public void Should_Have_Validation_Error_When_ProjectId_Is_Invalid(
            int taskId,
            int projectId)
        {
            // Arrange
            var validator = new GetTaskByIdValidator();

            var query = new GetTaskByIdQuery
            {
                TaskId = taskId,
                ProjectId = projectId
            };


            // Act
            var result = validator.TestValidate(query);


            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProjectId);
        }
    }
}