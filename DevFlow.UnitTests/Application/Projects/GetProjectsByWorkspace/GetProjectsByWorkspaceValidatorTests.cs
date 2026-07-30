using DevFlow.Application.Projects.GetProjectsByWorkspace;
using FluentValidation.TestHelper;
using Xunit;

namespace DevFlow.UnitTests.Application.Projects.GetProjectsByWorkspace
{
    public class GetProjectsByWorkspaceValidatorTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        public void Should_Not_Have_Validation_Error_When_Query_Is_Valid(int workspaceId)
        {
            // Arrange
            var validator = new GetProjectsByWorkspaceValidator();

            var query = new GetMyProjectsByWorkspaceQuery
            {
                WorkspaceId = workspaceId,
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = validator.TestValidate(query);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.WorkspaceId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Validation_Error_When_WorkspaceId_Is_Invalid(int workspaceId)
        {
            // Arrange
            var validator = new GetProjectsByWorkspaceValidator();

            var query = new GetMyProjectsByWorkspaceQuery
            {
                WorkspaceId = workspaceId,
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.WorkspaceId);
        }

    }
}