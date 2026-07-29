using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Workspaces.CreateWorkspace;
using FluentValidation.TestHelper;
using Xunit;

namespace DevFlow.UnitTests.Application.Workspaces
{
    public class CreateWorkspaceValidatorTests
    {

        [Theory]
        [InlineData("abc")]          // Minimum valid (3)
        [InlineData("Development")]  // Normal valid
        public void Should_Not_Have_Error_When_Name_Is_Valid(string name)
        {
            // Arrange
            var validator = new CreateWorkspaceValidator();

            var command = new CreateWorkspaceCommand
            {
                Name = name
            };

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }


        [Theory]
        [InlineData("")]
        [InlineData("ab")]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // 101 chars
        public void Should_Have_Error_When_Name_Is_Invalid(string name)
        {
            // Arrange

            var validator = new CreateWorkspaceValidator();
            var command = new CreateWorkspaceCommand
            {
                Name = name
            };
            // Act
            var result = validator.TestValidate(command);
            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }


    }
}
