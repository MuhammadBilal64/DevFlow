using DevFlow.Application.Tasks.UpdateTaskStatus;
using FluentValidation.TestHelper;
using Xunit;
using TaskStatusEnum = DevFlow.Domain.Enum.TaskStatus;

namespace DevFlow.UnitTests.Application.Tasks.UpdateTaskStatus
{
    public class UpdateTaskStatusValidatorTests
    {
        private readonly UpdateTaskStatusValidator _validator = new();



        [Fact]
        public void Should_Not_Have_Error_When_Command_Is_Valid()
        {
            // Arrange
            var command = new UpdateTaskStatusCommand
            {
                TaskId = 1,
                ProjectId = 10,
                TaskStatus = TaskStatusEnum.InProgress
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
            var command = new UpdateTaskStatusCommand
            {
                TaskId = taskId,
                ProjectId = 10,
                TaskStatus = TaskStatusEnum.Todo
            };


            // Act
            var result = _validator.TestValidate(command);


            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TaskId);
        }



        [Theory]
        [InlineData((TaskStatusEnum)100)]
        [InlineData((TaskStatusEnum)(-1))]
        public void Should_Have_Error_When_TaskStatus_Is_Invalid(
            TaskStatusEnum status)
        {
            // Arrange
            var command = new UpdateTaskStatusCommand
            {
                TaskId = 1,
                ProjectId = 10,
                TaskStatus = status
            };


            // Act
            var result = _validator.TestValidate(command);


            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TaskStatus);
        }



        [Fact]
        public void Should_Have_Error_When_ProjectId_Is_Invalid()
        {
            // Arrange
            var command = new UpdateTaskStatusCommand
            {
                TaskId = 1,
                ProjectId = 0,
                TaskStatus = TaskStatusEnum.Todo
            };


            // Act
            var result = _validator.TestValidate(command);


            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProjectId);
        }
    }
}