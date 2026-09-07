using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.ApeDialogs.WPF.Controls;
using ApeFree.ApeDialogs.WPF.Interop;

namespace ApeFree.ApeDialogs.WPF.Dialogs
{
    /// <summary>
    /// 日期时间对话框（WPF 实现）。按 <see cref="DateTimeDialogSettings.Precision"/> 展示日期/时间/日期时间。
    /// </summary>
    public class WpfDateTimeDialog : WpfDialogBase<DateTime>
    {
        private readonly DateTimeDialogSettings _settings;
        private readonly Calendar _calendar;
        private readonly ApeComboBox _hourCombo;
        private readonly ApeComboBox _minuteCombo;
        private readonly ApeComboBox _secondCombo;
        private readonly StackPanel _timePanel;

        public WpfDateTimeDialog(DateTimeDialogSettings settings, IWin32Owner? owner, IDialogTheme theme)
            : base(settings, owner, theme)
        {
            _settings = settings;

            Host.Width = 520;
            Host.Height = 420;

            _calendar = new Calendar
            {
                SelectedDate = settings.DefaultDateTime.Date,
                DisplayDate = settings.DefaultDateTime.Date,
                SelectionMode = CalendarSelectionMode.SingleDate,
            };

            _hourCombo = CreateTimeCombo(Enumerable.Range(0, 24));
            _minuteCombo = CreateTimeCombo(Enumerable.Range(0, 60));
            _secondCombo = CreateTimeCombo(Enumerable.Range(0, 60));

            SetTime(settings.DefaultDateTime.Hour, settings.DefaultDateTime.Minute, settings.DefaultDateTime.Second);

            _timePanel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(16, 0, 0, 0),
            };

            var content = new Grid();
            content.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            content.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var datePanel = new StackPanel();
            datePanel.Children.Add(_calendar);

            if (settings.Precision != DateTimePrecision.Date)
            {
                _timePanel.Children.Add(CreateTimeRow("Hour", _hourCombo));
                _timePanel.Children.Add(CreateTimeRow("Minute", _minuteCombo));
                _timePanel.Children.Add(CreateTimeRow("Second", _secondCombo));
            }

            Grid.SetColumn(datePanel, 0);
            Grid.SetColumn(_timePanel, 1);
            content.Children.Add(datePanel);
            content.Children.Add(_timePanel);

            if (settings.Precision == DateTimePrecision.Time)
            {
                _calendar.Visibility = Visibility.Collapsed;
            }

            var panel = new StackPanel();
            panel.Children.Add(ContentTextBlock);
            panel.Children.Add(content);
            Host.SetDialogContent(panel);

            AddDefaultOptions();
        }

        private static ApeComboBox CreateTimeCombo(IEnumerable<int> values)
        {
            return new ApeComboBox
            {
                Width = 72,
                ItemsSource = values.Select(v => v.ToString("00", System.Globalization.CultureInfo.InvariantCulture)).ToList(),
            };
        }

        private static Grid CreateTimeRow(string label, ApeComboBox combo)
        {
            var row = new Grid();
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var text = new TextBlock
            {
                Text = label,
                Width = 52,
                TextAlignment = TextAlignment.Right,
                FontSize = 13,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 8, 0),
            };
            text.SetResourceReference(TextBlock.ForegroundProperty, "ApeTextFillSecondary");

            Grid.SetColumn(text, 0);
            Grid.SetColumn(combo, 1);
            row.Children.Add(text);
            row.Children.Add(combo);
            row.Margin = new Thickness(0, 0, 0, 8);
            return row;
        }

        private void SetTime(int hour, int minute, int second)
        {
            _hourCombo.SelectedIndex = Math.Max(0, Math.Min(23, hour));
            _minuteCombo.SelectedIndex = Math.Max(0, Math.Min(59, minute));
            _secondCombo.SelectedIndex = Math.Max(0, Math.Min(59, second));
        }

        private DateTime GetSelectedDateTime()
        {
            var date = _calendar.SelectedDate ?? _settings.DefaultDateTime.Date;
            var hour = _hourCombo.SelectedIndex >= 0 ? _hourCombo.SelectedIndex : _settings.DefaultDateTime.Hour;
            var minute = _minuteCombo.SelectedIndex >= 0 ? _minuteCombo.SelectedIndex : _settings.DefaultDateTime.Minute;
            var second = _secondCombo.SelectedIndex >= 0 ? _secondCombo.SelectedIndex : _settings.DefaultDateTime.Second;
            return new DateTime(date.Year, date.Month, date.Day, hour, minute, second);
        }

        /// <inheritdoc/>
        public override ApeButton AddOption(DialogOption dialogOption)
        {
            var button = base.AddOption(dialogOption);

            if (ReferenceEquals(dialogOption, _settings.ConfirmOption))
            {
                button.Click += (_, _) => TrySubmit(GetSelectedDateTime());
            }
            else if (ReferenceEquals(dialogOption, _settings.CurrentTimeOption))
            {
                button.Click += (_, _) => SetCurrentTimeToNow();
            }

            return button;
        }

        /// <inheritdoc/>
        protected override void OnConfirmRequested() => TrySubmit(GetSelectedDateTime());

        private void SetCurrentTimeToNow()
        {
            var now = DateTime.Now;
            _calendar.SelectedDate = now.Date;
            _calendar.DisplayDate = now.Date;
            SetTime(now.Hour, now.Minute, now.Second);
        }
    }
}
