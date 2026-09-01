using System;
using System.Threading.Tasks;
using System.Windows;
using ApeFree.ApeDialogs;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.ApeDialogs.WPF;

namespace ApeDialogs.WpfDemo
{
    public partial class MainWindow : Window
    {
        private static readonly string[] Cities = { "北京", "上海", "广州", "深圳", "成都", "武汉", "西安", "南京", "杭州", "重庆" };
        private readonly ApeDialogsWpfProvider _provider = new ApeDialogsWpfProvider();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void MessageButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await _provider.ShowMessageDialogAsync(new MessageDialogSettings
            {
                Title = "Message",
                Content = "操作已完成。",
            });
            ShowResult(result);
        }

        private async void PromptButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await _provider.ShowPromptDialogAsync(new PromptDialogSettings
            {
                Title = "Prompt",
                Content = "是否继续执行？",
            });
            ShowResult(result);
        }

        private async void InputButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await _provider.ShowInputDialogAsync(new InputDialogSettings
            {
                Title = "Input",
                Content = "请输入名称：",
                DefaultText = "默认名称",
                Validator = v => string.IsNullOrWhiteSpace(v)
                    ? ValidationResult.Failure("名称不能为空")
                    : ValidationResult.Success,
            });
            ShowResult(result);
        }

        private async void PasswordButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await _provider.ShowPasswordDialogAsync(new PasswordDialogSettings
            {
                Title = "Password",
                Content = "请输入密码：",
            });
            ShowResult(result);
        }

        private async void TextButton_Click(object sender, RoutedEventArgs e)
        {
            var lines = new string[200];
            for (var i = 0; i < lines.Length; i++)
            {
                lines[i] = $"第 {i + 1} 行：ApeDialogs TextDialog 演示内容。";
            }

            var result = await _provider.ShowTextDialogAsync(new TextDialogSettings
            {
                Title = "Text",
                Content = "长文本查看",
                Text = string.Join(Environment.NewLine, lines),
            });
            ShowResult(result);
        }

        private async void ToastButton_Click(object sender, RoutedEventArgs e)
        {
            _ = _provider.ShowToastDialogAsync(new ToastDialogSettings
            {
                Content = "保存成功（2 秒后自动消失）",
                Duration = TimeSpan.FromSeconds(2),
            });
        }

        private async void SelectionButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await _provider.ShowSelectionDialogAsync(
                new SelectionDialogSettings<string> { Title = "Selection", Content = "选择城市：" },
                Cities,
                defaultSelectedItem: "北京");
            ShowResult(result);
        }

        private static readonly string[] TagOptions = { "A", "B", "C", "D", "E", "F" };

        private static readonly string[] DefaultTags = { "A", "C" };

        private async void MultipleSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await _provider.ShowMultipleSelectionDialogAsync(
                new MultipleSelectionDialogSettings<string> { Title = "MultipleSelection", Content = "选择标签：" },
                TagOptions,
                defaultSelectedItems: DefaultTags);
            ShowResult(result);
        }

        private async void ComboBoxInputButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await _provider.ShowComboBoxInputDialogAsync(new ComboBoxInputDialogSettings
            {
                Title = "ComboBoxInput",
                Content = "选择或输入城市：",
                Items = Cities,
                DefaultText = "北京",
            });
            ShowResult(result);
        }

        private async void DateTimeButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await _provider.ShowDateTimeDialogAsync(new DateTimeDialogSettings
            {
                Title = "DateTime",
                Content = "选择日期时间：",
                DefaultDateTime = DateTime.Now,
            });
            ShowResult(result);
        }

        private async void NumberInputButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await _provider.ShowNumberInputDialogAsync(new NumberInputDialogSettings
            {
                Title = "NumberInput",
                Content = "请输入超时秒数：",
                Minimum = 1,
                Maximum = 3600,
                Increment = 5,
                DecimalPlaces = 0,
                DefaultValue = 30,
            });
            ShowResult(result);
        }

        private async void SliderButton_Click(object sender, RoutedEventArgs e)
        {
            decimal lastLiveValue = 0;
            var result = await _provider.ShowSliderDialogAsync(new SliderDialogSettings
            {
                Title = "Slider",
                Content = "调节音量（拖动实时回调）：",
                Minimum = 0,
                Maximum = 100,
                DefaultValue = 50,
                OnValueChanged = value => lastLiveValue = value,
            });
            if (!result.IsCanceled)
            {
                ShowResult(DialogResult<decimal>.Confirmed(result.Value));
                _ = _provider.ShowToastDialogAsync(new ToastDialogSettings { Content = $"最终值 {result.Value}（实时回调最后值 {lastLiveValue}）" });
            }
        }

        private async void RangeSliderButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await _provider.ShowRangeSliderDialogAsync(new RangeSliderDialogSettings
            {
                Title = "RangeSlider",
                Content = "调节价格区间：",
                Minimum = 0,
                Maximum = 10000,
                DefaultStart = 1000,
                DefaultEnd = 5000,
            });
            ShowResult(result);
        }

        private async void ProgressButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = _provider.CreateProgressDialog(new ProgressDialogSettings
            {
                Title = "Progress",
                Content = "正在处理：",
                InitialMessage = "准备中…",
            });

            var task = dialog.ShowAsync();

            for (var i = 1; i <= 10; i++)
            {
                await Task.Delay(200);
                dialog.UpdateProgress(i / 10.0, $"处理中 {i * 10}%…");
            }

            var result = await task;
            ShowResult(result);
        }

        private async void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await _provider.ShowColorDialogAsync(new ColorDialogSettings
            {
                Title = "Color",
                Content = "选择主题色：",
                DefaultColor = new ApeColor(0, 122, 204),
                AllowAlpha = false,
                PresetColors = new[]
                {
                    new ApeColor(255, 0, 0),
                    new ApeColor(0, 255, 0),
                    new ApeColor(0, 0, 255),
                    new ApeColor(0, 122, 204),
                    new ApeColor(255, 165, 0),
                },
            });
            ShowResult(result);
        }

        private async void SheetButton_Click(object sender, RoutedEventArgs e)
        {
            var sheet = new DataEntrySheet()
                .AddField(new TextField
                {
                    Title = "姓名",
                    Validator = v => string.IsNullOrWhiteSpace(v)
                        ? ValidationResult.Failure("姓名必填")
                        : ValidationResult.Success,
                })
                .AddField(new PasswordField { Title = "密码" })
                .AddField(new NumberField { Title = "年龄", Minimum = 0, Maximum = 150, DecimalPlaces = 0 })
                                .AddField(new SingleChoiceField
                {
                    Title = "性别",
                    Items = new object[] { "男", "女" },
                })
                .AddField(new MultipleChoiceField
                {
                    Title = "爱好",
                    Items = new object[] { "编程", "音乐", "运动", "阅读" },
                })
                .AddField(new ComboBoxField
                {
                    Title = "城市",
                    Items = new object[] { "北京", "上海", "广州", "深圳" },
                })                .AddField(new DateTimeField { Title = "生日" });

            var result = await _provider.ShowDataEntrySheetDialogAsync(sheet, new DataEntrySheetDialogSettings
            {
                Title = "DataEntrySheet",
                Content = "请填写以下信息：",
            });

            if (!result.IsCanceled)
            {
                ShowResult(DialogResult<DataEntrySheet>.Confirmed(result.Value!));
                _ = _provider.ShowToastDialogAsync(new ToastDialogSettings { Content = "表单已提交" });
            }
        }

        private async void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await _provider.ShowOpenFileDialogAsync(new OpenFileDialogSettings
            {
                Title = "OpenFile",
                SearchPattern = "*",
                MultiSelect = true,
            });
            ShowResult(result);
        }

        private async void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await _provider.ShowOpenFolderDialogAsync(new OpenFolderDialogSettings
            {
                Title = "OpenFolder",
            });
            ShowResult(result);
        }

        private async void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await _provider.ShowSaveFileDialogAsync(new SaveFileDialogSettings
            {
                Title = "SaveFile",
                DefaultFileName = "output.txt",
            });
            ShowResult(result);
        }

        private static void ShowResult(object result)
        {
            MessageBox.Show($"Result: {result}", "ApeDialogs Demo");
        }
    }
}
