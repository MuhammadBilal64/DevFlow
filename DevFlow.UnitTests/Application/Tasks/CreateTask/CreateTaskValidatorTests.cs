using DevFlow.Application.Tasks.CreateTask;
using DevFlow.Domain.Enum;
using FluentValidation.TestHelper;
using Xunit;

namespace DevFlow.UnitTests.Application.Tasks.CreateTask
{
    public class CreateTaskValidatorTests
    {
        [Theory]
        [InlineData("Implement CQRS", "Implement MediatR handlers", 1)]
        [InlineData("Fix Bug", "Resolve login issue", 10)]
        public void Should_Not_Have_Validation_Error_When_Command_Is_Valid(
            string title,
            string description,
            int projectId)
        {
            // Arrange
            var validator = new CreateTaskValidator();

            var command = new CreateTaskCommand
            {
                Title = title,
                Description = description,
                ProjectId = projectId,
                Priority = TaskPriority.Medium,
                DueDate = DateTime.UtcNow.AddDays(2)
            };

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Title);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
            result.ShouldNotHaveValidationErrorFor(x => x.ProjectId);
            result.ShouldNotHaveValidationErrorFor(x => x.Priority);
            result.ShouldNotHaveValidationErrorFor(x => x.DueDate);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Have_Validation_Error_When_Title_Is_Empty(string title)
        {
            // Arrange
            var validator = new CreateTaskValidator();

            var command = new CreateTaskCommand
            {
                Title = title,
                Description = "Description",
                ProjectId = 1,
                Priority = TaskPriority.Medium
            };

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Have_Validation_Error_When_Title_Exceeds_Max_Length()
        {
            // Arrange
            var validator = new CreateTaskValidator();

            var command = new CreateTaskCommand
            {
                Title = new string('A', 51),
                Description = "Description",
                ProjectId = 1,
                Priority = TaskPriority.Medium
            };

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Have_Validation_Error_When_Description_Exceeds_Max_Length()
        {
            // Arrange
            var validator = new CreateTaskValidator();

            var command = new CreateTaskCommand
            {
                Title = "Task",
                Description = new string('A', 201),
                ProjectId = 1,
                Priority = TaskPriority.Medium
            };

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Validation_Error_When_ProjectId_Is_Invalid(int projectId)
        {
            // Arrange
            var validator = new CreateTaskValidator();

            var command = new CreateTaskCommand
            {
                Title = "Task",
                Description = "Description",
                ProjectId = projectId,
                Priority = TaskPriority.Medium
            };

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProjectId);
        }

        [Fact]
        public void Should_Have_Validation_Error_When_Priority_Is_Invalid()
        {
            // Arrange
            var validator = new CreateTaskValidator();

            var command = new CreateTaskCommand
            {
                Title = "Task",
                Description = "Description",
                ProjectId = 1,
                Priority = (TaskPriority)999
            };

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Priority);
        }

        [Fact]
        public void Should_Have_Validation_Error_When_DueDate_Is_In_The_Past()
        {
            // Arrange
            var validator = new CreateTaskValidator();

            var command = new CreateTaskCommand
            {
                Title = "Task",
                Description = "Description",
                ProjectId = 1,
                Priority = TaskPriority.Medium,
                DueDate = DateTime.UtcNow.AddDays(-1)
            };

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.DueDate);
        }

        [Fact]
        public void Should_Not_Have_Validation_Error_When_DueDate_Is_Null()
        {
            // Arrange
            var validator = new CreateTaskValidator();

            var command = new CreateTaskCommand
            {
                Title = "Task",
                Description = "Description",
                ProjectId = 1,
                Priority = TaskPriority.Medium,
                DueDate = null
            };

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.DueDate);
        }

        [Fact]
        public void Should_Not_Have_Validation_Error_When_DueDate_Is_In_The_Future()
        {
            // Arrange
            var validator = new CreateTaskValidator();

            var command = new CreateTaskCommand
            {
                Title = "Task",
                Description = "Description",
                ProjectId = 1,
                Priority = TaskPriority.Medium,
                DueDate = DateTime.UtcNow.AddDays(5)
            };

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.DueDate);
        }
    }
}