using DevFlow.Application.Projects.GetProjectById;
using FluentValidation.TestHelper;
using Xunit;

namespace DevFlow.UnitTests.Application.Projects.GetProjectById
{
    public class GetProjectByIdValidatorTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void Should_Not_Have_Validation_Error_When_Id_Is_Valid(int id)
        {
            // Arrange
            var validator = new GetProjectByIdValidator();

            var query = new GetProjectByIdQuery
            {
                Id = id
            };

            // Act
            var result = validator.TestValidate(query);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Id);
        }

        [Theory]
        [InlineData(0)]
        public void Should_Have_Validation_Error_When_Id_Is_Zero(int id)
        {
            // Arrange
            var validator = new GetProjectByIdValidator();

            var query = new GetProjectByIdQuery
            {
                Id = id
            };

            // Act
            var result = validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-10)]
        public void Should_Have_Validation_Error_When_Id_Is_Negative(int id)
        {
            // Arrange
            var validator = new GetProjectByIdValidator();

            var query = new GetProjectByIdQuery
            {
                Id = id
            };

            // Act
            var result = validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }
    }
}