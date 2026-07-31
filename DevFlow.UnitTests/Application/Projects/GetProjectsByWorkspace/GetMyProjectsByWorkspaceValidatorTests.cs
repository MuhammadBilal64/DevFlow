using DevFlow.Application.Projects.GetMyProjectsByWorkspace;
using FluentValidation.TestHelper;
using Xunit;

namespace DevFlow.UnitTests.Application.Projects.GetMyProjectsByWorkspace
{
    public class GetMyProjectsByWorkspaceValidatorTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        public void Should_Not_Have_Validation_Error_When_Query_Is_Valid(int workspaceId)
        {
            var validator = new GetMyProjectsByWorkspaceValidator();

            var query = new GetMyProjectsByWorkspaceQuery
            {
                WorkspaceId = workspaceId,
                PageNumber = 1,
                PageSize = 10
            };

            var result = validator.TestValidate(query);

            result.ShouldNotHaveValidationErrorFor(x => x.WorkspaceId);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Validation_Error_When_WorkspaceId_Is_Invalid(int workspaceId)
        {
            var validator = new GetMyProjectsByWorkspaceValidator();

            var query = new GetMyProjectsByWorkspaceQuery
            {
                WorkspaceId = workspaceId,
                PageNumber = 1,
                PageSize = 10
            };

            var result = validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.WorkspaceId);
        }
    }
}