using System;
using ApeFree.ApeDialogs.Maui.Theme;
using ApeFree.ApeDialogs.Theme;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace ApeFree.ApeDialogs.Maui.Controls
{
    /// <summary>
    /// ApeDialogs MAUI 选项按钮控件，具备语义标签着色与跨平台统一外观。
    /// </summary>
    public partial class ApeButton : Button
    {
        private DialogOptionTag _tag = DialogOptionTag.Neutral;

        public ApeButton()
        {
            HeightRequest = 40;
            CornerRadius = 8;
            Padding = new Thickness(16, 0);
            FontSize = 14;
            FontAutoScalingEnabled = true;
            ApplyDefaultStyle();
        }

        /// <summary>
        /// 语义标签。
        /// </summary>
        public DialogOptionTag Tag
        {
            get => _tag;
            set
            {
                _tag = value;
                ApplyTagStyle();
            }
        }

        private void ApplyDefaultStyle()
        {
            BackgroundColor = MauiThemeManager.ControlBackground;
            TextColor = MauiThemeManager.TextPrimary;
            BorderColor = MauiThemeManager.StrokeDefault;
            BorderWidth = 1;
        }

        private void ApplyTagStyle()
        {
            switch (_tag)
            {
                case DialogOptionTag.Positive:
                    BackgroundColor = MauiThemeManager.AccentColor;
                    TextColor = MauiThemeManager.AccentTextColor;
                    BorderWidth = 0;
                    break;

                case DialogOptionTag.Negative:
                    BackgroundColor = Colors.Transparent;
                    TextColor = MauiThemeManager.NegativeColor;
                    BorderColor = MauiThemeManager.NegativeColor;
                    BorderWidth = 1;
                    break;

                case DialogOptionTag.Special:
                    BackgroundColor = Colors.Transparent;
                    TextColor = MauiThemeManager.SpecialColor;
                    BorderColor = MauiThemeManager.SpecialColor;
                    BorderWidth = 1;
                    break;

                case DialogOptionTag.Neutral:
                default:
                    ApplyDefaultStyle();
                    break;
            }
        }
    }
}
