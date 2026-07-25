using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Workspaces.AddWorkspaceMember;
using DevFlow.Domain.Enum;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace DevFlow.UnitTests.Application.Workspaces.AddWorkspaceMember
{
    public class AddWorkspaceMemberValidatorTests
    {
        [Theory]
        [InlineData(1,1,WorkspaceRole.Owner)]
        [InlineData(1,1,WorkspaceRole.Admin)]
     public void  Should_Not_Have_Validation_Error_When_Command_Is_Valid(int userId,int WorkspaceId,WorkspaceRole Role)
        {
            //arrange
            var validator = new AddWorkspaceMemberValidator();
            var command=new AddWorkspaceMemberCommand
            {
               UserId = userId,
               WorkspaceId= WorkspaceId,
               Role = Role
            };
            //act
            var result=validator.TestValidate(command);
            //assert
            result.ShouldNotHaveValidationErrorFor(x => x.UserId);
            result.ShouldNotHaveValidationErrorFor(x => x.WorkspaceId);
            result.ShouldNotHaveValidationErrorFor(x => x.Role);

        }
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Validation_Error_When_UserId_Is_Invalid(int userId)
        {
              //arrange
              var validator=new AddWorkspaceMemberValidator();
            var command = new AddWorkspaceMemberCommand
            {
                UserId = userId,
                WorkspaceId = 1,
                Role = WorkspaceRole.Owner
            };
            
              //act
              var result= validator.TestValidate(command);
              //assert
              result.ShouldHaveValidationErrorFor(x => x.UserId);



        }
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void Should_Have_Validation_Error_When_WorkspaceId_Is_Invalid(int WorkspaceId)
        {
            //arrange
            var validator= new AddWorkspaceMemberValidator();
            var command = new AddWorkspaceMemberCommand
            {
                UserId=1,
                WorkspaceId=WorkspaceId,
                Role=WorkspaceRole.Owner
            };
            //act
            var result=validator.TestValidate(command);
            //assert
            result.ShouldHaveValidationErrorFor(x => x.WorkspaceId);
        }
        [Theory]
        [InlineData((WorkspaceRole)999)]
        public void Should_Have_Validation_Error_When_Role_Is_Invalid(WorkspaceRole Role)
        {
            //arrange
            var validator = new AddWorkspaceMemberValidator();
            var command = new AddWorkspaceMemberCommand
            {
                UserId=1,
                WorkspaceId=1,
                Role=Role
            };
            //act
            var result = validator.TestValidate(command);
            //assert
            result.ShouldHaveValidationErrorFor(x => x.Role);

        }




    }
}
