using DevFlow.Application.Projects.UpdateProject;
using FluentValidation.TestHelper;
using Xunit;

namespace DevFlow.UnitTests.Application.Projects.UpdateProject
{
    public class UpdateProjectValidatorTests
    {
        [Theory]
        [InlineData(1, "AI Project", "Description")]
        [InlineData(10, "CRM", "CRM Description")]
        public void Should_Not_Have_Validation_Error_When_Command_Is_Valid(
            int projectId,
            string name,
            string description)
        {
            // Arrange
            var validator = new UpdateProjectValidator();

            var command = new UpdateProjectCommand
            {
                ProjectId = projectId,
                Name = name,
                Description = description
            };

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.ProjectId);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Validation_Error_When_ProjectId_Is_Invalid(int projectId)
        {
            // Arrange
            var validator = new UpdateProjectValidator();

            var command = new UpdateProjectCommand
            {
                ProjectId = projectId,
                Name = "AI Project",
                Description = "Description"
            };

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProjectId);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Have_Validation_Error_When_Name_Is_Empty(string name)
        {
            // Arrange
            var validator = new UpdateProjectValidator();

            var command = new UpdateProjectCommand
            {
                ProjectId = 1,
                Name = name,
                Description = "Description"
            };

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Have_Validation_Error_When_Name_Exceeds_Max_Length()
        {
            // Arrange
            var validator = new UpdateProjectValidator();

            var command = new UpdateProjectCommand
            {
                ProjectId = 1,
                Name = new string('A', 101),
                Description = "Description"
            };

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Have_Validation_Error_When_Description_Is_Empty(string description)
        {
            // Arrange
            var validator = new UpdateProjectValidator();

            var command = new UpdateProjectCommand
            {
                ProjectId = 1,
                Name = "AI Project",
                Description = description
            };

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Description);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Validation_Error_When_Description_Exceeds_Max_Length()
        {
            // Arrange
            var validator = new UpdateProjectValidator();

            var command = new UpdateProjectCommand
            {
                ProjectId = 1,
                Name = "AI Project",
                Description = new string('A', 201)
            };

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }
    }
}