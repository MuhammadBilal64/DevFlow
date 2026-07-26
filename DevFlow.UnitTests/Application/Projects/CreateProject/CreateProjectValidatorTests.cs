using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Projects.CreateProject;
using FluentValidation.TestHelper;
using Xunit;

namespace DevFlow.UnitTests.Application.Projects.CreateProject
{
    public class CreateProjectValidatorTests
    {
        [Theory]
        [InlineData("ai_feat","fajjfjag",1)]
        public void Should_Not_Have_Validation_Error_When_Command_Is_Valid(string Name,string description,int WorkspaceId)
        {
            //arrange
            var validator = new CreateProjectValidator();
            var command= new CreateProjectCommand
            {
                ProjectName=Name,
                Description =description,
                WorkspaceId=WorkspaceId

            };
            //act
            var result= validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.ProjectName);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
            result.ShouldNotHaveValidationErrorFor(x => x.WorkspaceId);
        }
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Have_Validation_Error_When_ProjectName_Is_Empty(string projectName)
        {
            // Arrange
            var validator = new CreateProjectValidator();

            var command = new CreateProjectCommand
            {
                ProjectName = projectName,
                Description = "Valid Description",
                WorkspaceId = 1
            };

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProjectName);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }
        [Fact]
        public void Should_Have_Validation_Error_When_ProjectName_Exceeds_Max_Length()
        {
            // Arrange
            var validator = new CreateProjectValidator();

            var command = new CreateProjectCommand
            {
                ProjectName = new string('A', 101),
                Description = "Valid Description",
                WorkspaceId = 1
            };

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProjectName);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Have_Validation_Error_When_Description_Is_Empty(string description)
        {
            // Arrange
            var validator = new CreateProjectValidator();

            var command = new CreateProjectCommand
            {
                ProjectName = "AI Project",
                Description = description,
                WorkspaceId = 1
            };

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Description);
            result.ShouldNotHaveValidationErrorFor(x => x.ProjectName);
        }
        [Fact]
        public void Should_Have_Validation_Error_When_Description_Exceeds_Max_Length()
        {
            // Arrange
            var validator = new CreateProjectValidator();

            var command = new CreateProjectCommand
            {
                ProjectName = "AI Project",
                Description = new string('A', 201),
                WorkspaceId = 1
            };

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Description);
            result.ShouldNotHaveValidationErrorFor(x => x.ProjectName);
        }
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Validation_Error_When_WorkspaceId_Is_Invalid(int workspaceId)
        {
            // Arrange
            var validator = new CreateProjectValidator();

            var command = new CreateProjectCommand
            {
                ProjectName = "AI Project",
                Description = "Valid Description",
                WorkspaceId = workspaceId
            };

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.WorkspaceId);
        }
    }
}
