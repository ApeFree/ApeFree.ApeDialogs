using System;
using ApeFree.ApeDialogs.Core;
using Xunit;

namespace ApeFree.ApeDialogs.Tests
{
    public class DialogResultTests
    {
        [Fact]
        public void Confirmed_ReturnsValue()
        {
            var result = DialogResult<int>.Confirmed(42);
            Assert.False(result.IsCanceled);
            Assert.Equal(42, result.Value);
        }

        [Fact]
        public void Canceled_HasNoValue()
        {
            var result = DialogResult<string>.Canceled;
            Assert.True(result.IsCanceled);
            Assert.Null(result.Value);
        }

        [Fact]
        public void RequireValue_ReturnsValue_WhenConfirmed()
        {
            Assert.Equal("ok", DialogResult<string>.Confirmed("ok").RequireValue());
        }

        [Fact]
        public void RequireValue_Throws_WhenCanceled()
        {
            Assert.Throws<InvalidOperationException>(() => DialogResult<int>.Canceled.RequireValue());
        }

        [Fact]
        public void Match_RoutesToCorrectBranch()
        {
            var confirmed = DialogResult<string>.Confirmed("v").Match(ok => "C:" + ok, () => "X");
            var canceled = DialogResult<string>.Canceled.Match(ok => "C:" + ok, () => "X");

            Assert.Equal("C:v", confirmed);
            Assert.Equal("X", canceled);
        }

        [Fact]
        public void Match_Throws_OnNullBranch()
        {
            Assert.Throws<ArgumentNullException>(() => DialogResult<int>.Confirmed(1).Match(null!, () => 0));
        }

        [Fact]
        public void Record_ValueEquality()
        {
            Assert.Equal(DialogResult<int>.Confirmed(1), DialogResult<int>.Confirmed(1));
            Assert.NotEqual(DialogResult<int>.Confirmed(1), DialogResult<int>.Confirmed(2));
            Assert.Equal(DialogResult<int>.Canceled, DialogResult<int>.Canceled);
        }
    }
}
