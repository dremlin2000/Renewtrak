using FluentValidation.TestHelper;
using Glossary.Core.Validators;
using Glossary.Core.ViewModels;
using System;
using System.Linq;
using Xunit;

namespace Glossary.Core.Tests.Validators
{
    public class GlossaryVmValidatorTests
    {
        [Fact]
        public void Given_GlossaryVmValidator_When_ValidationIsTriggered_Then_NoValidationErrors()
        {
            //Arrange
            var testVm = new GlossaryVm
            {
                Id = Guid.NewGuid(),
                Term = "testterm",
                Definition = "testdefinition"
            };
            var validator = new GlossaryVmValidator();

            //Act
            var validatorResult = validator.Validate(testVm);

            //Assert
            Assert.True(validatorResult.IsValid);
        }

        [Fact]
        public void Given_GlossaryVmValidator_When_EmptyDataProvided_Then_ValidationErrorsRetreived()
        {
            //Arrange
            var testVm = new GlossaryVm
            {
            };
            var validator = new GlossaryVmValidator();

            //Act
            var validatorResult = validator.Validate(testVm);

            //Assert
            Assert.False(validatorResult.IsValid);
            PropertyShouldBeInvalid(validatorResult, nameof(testVm.Id));
            PropertyShouldBeInvalid(validatorResult, nameof(testVm.Term));
            PropertyShouldBeInvalid(validatorResult, nameof(testVm.Definition));
        }

        private void PropertyShouldBeInvalid(FluentValidation.Results.ValidationResult result, string propertyName)
        {
            Assert.True(result.Errors.Where(x => x.PropertyName.Equals(propertyName)).Any());
        }
    }
}
