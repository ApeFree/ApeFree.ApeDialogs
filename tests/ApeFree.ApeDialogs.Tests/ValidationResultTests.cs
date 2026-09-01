using System;
using ApeFree.ApeDialogs.Core;
using Xunit;

namespace ApeFree.ApeDialogs.Tests
{
    public class ValidationResultTests
    {
        [Fact]
        public void Success_IsSuccess()
        {
            Assert.True(ValidationResult.Success.IsSuccess);
            Assert.Null(ValidationResult.Success.ErrorMessage);
        }

        [Fact]
        public void Failure_CarriesMessage()
        {
            var result = ValidationResult.Failure("bad input");
            Assert.False(result.IsSuccess);
            Assert.Equal("bad input", result.ErrorMessage);
        }

        [Fact]
        public void Failure_Throws_OnNullMessage()
        {
            Assert.Throws<ArgumentNullException>(() => ValidationResult.Failure(null!));
        }

        [Fact]
        public void ToString_ReflectsState()
        {
            Assert.Equal("Success", ValidationResult.Success.ToString());
            Assert.Equal("oops", ValidationResult.Failure("oops").ToString());
        }
    }
}
