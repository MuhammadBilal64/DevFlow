using DevFlow.Application.Common.Models;
using DevFlow.Application.Common.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace DevFlow.UnitTests.Application.Common.Validation
{
    public class PaginationRequestValidatorTests
    {
        private readonly PaginationRequestValidator _validator =
            new();

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(10)]
        public void Should_Not_Have_Error_When_PageNumber_Is_Valid(
            int pageNumber)
        {
            var request = new PaginationRequest
            {
                PageNumber = pageNumber,
                PageSize = 10
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor(
                x => x.PageNumber);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_PageNumber_Is_Invalid(
            int pageNumber)
        {
            var request = new PaginationRequest
            {
                PageNumber = pageNumber,
                PageSize = 10
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(
                x => x.PageNumber);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(100)]
        public void Should_Not_Have_Error_When_PageSize_Is_Valid(
            int pageSize)
        {
            var request = new PaginationRequest
            {
                PageNumber = 1,
                PageSize = pageSize
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor(
                x => x.PageSize);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(101)]
        public void Should_Have_Error_When_PageSize_Is_Invalid(
            int pageSize)
        {
            var request = new PaginationRequest
            {
                PageNumber = 1,
                PageSize = pageSize
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(
                x => x.PageSize);
        }
    }
}