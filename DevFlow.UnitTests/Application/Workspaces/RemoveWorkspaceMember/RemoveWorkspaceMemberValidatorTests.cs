using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Workspaces.RemoveWorkspaceMember;
using FluentValidation.TestHelper;
using Xunit;

namespace DevFlow.UnitTests.Application.Workspaces.RemoveWorkspaceMember
{
    public class RemoveWorkspaceMemberValidatorTests
    {
        [Theory]
        [InlineData(1,2)]
        [InlineData(2,1)]
   
       public void Should_Not_Have_Validation_Error_When_Command_Is_Valid(int UserId,int WorkspaceId)
        {
            //arrange
            var validator= new RemoveWorkspaceMemberValidator();
            var command = new DevFlow.Application.Workspaces.RemoveWorkspaceMember.RemoveWorkspaceMemberCommand
            {
                UserId= UserId,
                WorkspaceId= WorkspaceId
            };
            //act
            var result = validator.TestValidate(command);
            //assert
            result.ShouldNotHaveValidationErrorFor(x => x.UserId);
            result.ShouldNotHaveValidationErrorFor(x => x.WorkspaceId);
           

           
          
        }
        [Theory]
        [InlineData(0, 2)]
        [InlineData(-1, 1)]
        public void Should_Have_Validation_Error_When_UserId_Is_Invalid(int UserId,int WorkspaceId)
        {
            //arrange
            var validator=new RemoveWorkspaceMemberValidator();
            var command = new RemoveWorkspaceMemberCommand
            {
                UserId=UserId, WorkspaceId=WorkspaceId
            };
            //act
            var result=validator.TestValidate(command);
            //assert
            result.ShouldHaveValidationErrorFor(x => x.UserId);
            result.ShouldNotHaveValidationErrorFor(x => x.WorkspaceId);
           

        }

        [Theory]
        [InlineData(2, 0)]
        [InlineData(1, -1)]
        public void Should_Have_Validation_Error_When_WorkspaceId_Is_Invalid(int UserId, int WorkspaceId)
        {
            //arrange
            var validator = new RemoveWorkspaceMemberValidator();
            var command = new RemoveWorkspaceMemberCommand
            {
                UserId = UserId,
                WorkspaceId = WorkspaceId
            };
            //act
            var result = validator.TestValidate(command);
            //assert
            result.ShouldHaveValidationErrorFor(x => x.WorkspaceId);
            result.ShouldNotHaveValidationErrorFor(x => x.UserId);


        }

    }
}
