using ApeFree.ApeDialogs.Theme;
using Xunit;

namespace ApeFree.ApeDialogs.Tests
{
    public class ThemeTests
    {
        [Fact]
        public void ApeColor_ToArgb_FromArgb_RoundTrip()
        {
            var color = new ApeColor(255, 0, 122, 204);
            Assert.Equal(color, ApeColor.FromArgb(color.ToArgb()));
        }

        [Fact]
        public void ApeColor_ToString_IsHex()
        {
            Assert.Equal("#FF007ACC", new ApeColor(0, 122, 204).ToString());
        }

        [Fact]
        public void ApeSize_Empty()
        {
            Assert.True(ApeSize.Empty.IsEmpty);
            Assert.False(new ApeSize(100, 50).IsEmpty);
        }

        [Fact]
        public void DefaultTheme_ProvidesAllTags()
        {
            var theme = DefaultTheme.Instance;
            Assert.NotEqual(theme.GetColor(DialogOptionTag.Neutral), theme.GetColor(DialogOptionTag.Special));
            Assert.Equal(new ApeColor(0, 122, 204), theme.ReminderColor);
        }
    }
}
