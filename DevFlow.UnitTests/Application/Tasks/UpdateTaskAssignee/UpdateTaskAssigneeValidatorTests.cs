using DevFlow.Application.Tasks.UpdateTaskAssignee;
using FluentValidation.TestHelper;
using Xunit;

namespace DevFlow.UnitTests.Application.Tasks.UpdateTaskAssignee
{
    public class UpdateTaskAssigneeValidatorTests
    {
        private readonly UpdateTaskAssigneeValidator _validator = new();



        [Fact]
        public void Should_Not_Have_Error_When_Command_Is_Valid()
        {
            // Arrange
            var command = new UpdateTaskAssigneeCommand
            {
                TaskId = 1,
                ProjectId = 10,
                NewAssigneeId = 5
            };


            // Act
            var result = _validator.TestValidate(command);


            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }



        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_TaskId_Is_Invalid(int taskId)
        {
            // Arrange
            var command = new UpdateTaskAssigneeCommand
            {
                TaskId = taskId,
                ProjectId = 10,
                NewAssigneeId = 5
            };


            // Act
            var result = _validator.TestValidate(command);


            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TaskId);
        }



        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_NewAssigneeId_Is_Invalid(int assigneeId)
        {
            // Arrange
            var command = new UpdateTaskAssigneeCommand
            {
                TaskId = 1,
                ProjectId = 10,
                NewAssigneeId = assigneeId
            };


            // Act
            var result = _validator.TestValidate(command);


            // Assert
            result.ShouldHaveValidationErrorFor(x => x.NewAssigneeId);
        }



        [Fact]
        public void Should_Have_Error_When_ProjectId_Is_Invalid()
        {
            // Arrange
            var command = new UpdateTaskAssigneeCommand
            {
                TaskId = 1,
                ProjectId = 0,
                NewAssigneeId = 5
            };


            // Act
            var result = _validator.TestValidate(command);


            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProjectId);
        }



        [Fact]
        public void Should_Not_Have_Error_When_NewAssigneeId_Is_Null()
        {
            // Arrange
            var command = new UpdateTaskAssigneeCommand
            {
                TaskId = 1,
                ProjectId = 10,
                NewAssigneeId = null
            };


            // Act
            var result = _validator.TestValidate(command);


            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.NewAssigneeId);
        }
    }
}