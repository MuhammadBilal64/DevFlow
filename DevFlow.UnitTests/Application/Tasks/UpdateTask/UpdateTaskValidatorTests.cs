using DevFlow.Application.Tasks.UpdateTask;
using DevFlow.Domain.Enum;
using FluentValidation.TestHelper;
using Xunit;

namespace DevFlow.UnitTests.Application.Tasks.UpdateTask
{
    public class UpdateTaskValidatorTests
    {
        private readonly UpdateTaskValidator _validator = new();



        [Fact]
        public void Should_Not_Have_Error_When_Command_Is_Valid()
        {
            // Arrange
            var command = new UpdateTaskCommand
            {
                TaskId = 1,
                ProjectId = 10,
                Title = "Updated Task",
                Description = "Updated Description",
                Priority = TaskPriority.High
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
            var command = new UpdateTaskCommand
            {
                TaskId = taskId,
                ProjectId = 10,
                Title = "Valid Title",
                Priority = TaskPriority.High
            };


            // Act
            var result = _validator.TestValidate(command);


            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TaskId);
        }



        [Fact]
        public void Should_Have_Error_When_ProjectId_Is_Invalid()
        {
            // Arrange
            var command = new UpdateTaskCommand
            {
                TaskId = 1,
                ProjectId = 0,
                Title = "Valid Title",
                Priority = TaskPriority.High
            };


            // Act
            var result = _validator.TestValidate(command);


            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProjectId);
        }



        [Fact]
        public void Should_Have_Error_When_Title_Is_Empty()
        {
            // Arrange
            var command = new UpdateTaskCommand
            {
                TaskId = 1,
                ProjectId = 10,
                Title = string.Empty,
                Priority = TaskPriority.High
            };


            // Act
            var result = _validator.TestValidate(command);


            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }



        [Fact]
        public void Should_Have_Error_When_Title_Exceeds_50_Characters()
        {
            // Arrange
            var command = new UpdateTaskCommand
            {
                TaskId = 1,
                ProjectId = 10,
                Title = new string('A', 51),
                Priority = TaskPriority.High
            };


            // Act
            var result = _validator.TestValidate(command);


            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }



        [Fact]
        public void Should_Have_Error_When_Description_Exceeds_200_Characters()
        {
            // Arrange
            var command = new UpdateTaskCommand
            {
                TaskId = 1,
                ProjectId = 10,
                Title = "Valid Title",
                Description = new string('A', 201),
                Priority = TaskPriority.High
            };


            // Act
            var result = _validator.TestValidate(command);


            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }



        [Theory]
        [InlineData(-1)]
        [InlineData(100)]
        public void Should_Have_Error_When_Priority_Is_Invalid(int priority)
        {
            // Arrange
            var command = new UpdateTaskCommand
            {
                TaskId = 1,
                ProjectId = 10,
                Title = "Valid Title",
                Priority = (TaskPriority)priority
            };


            // Act
            var result = _validator.TestValidate(command);


            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Priority);
        }
    }
}