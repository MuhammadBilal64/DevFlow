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
        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            //arrange
            var validator = new CreateWorkspaceValidator();
            var command = new CreateWorkspaceCommand
            {
                Name = string.Empty
            };
            //act
            var result = validator.TestValidate(command);
            //assert
            result.ShouldHaveValidationErrorFor(x => x.Name);




        }


    }
}
