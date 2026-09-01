using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.WPF;
using ApeFree.ApeDialogs.WPF.WinForms;

namespace ApeDialogs.WinFormsDemo
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    public class MainForm : Form
    {
        private readonly ApeDialogsWpfProvider _provider = new ApeDialogsWpfProvider();

        public MainForm()
        {
            Text = "ApeDialogs WinForms Demo";
            Size = new System.Drawing.Size(560, 200);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);

            var messageButton = CreateButton("Message (WPF Dialog)", 24, ShowMessage);
            var inputButton = CreateButton("Input (WPF Dialog)", 80, ShowInput);
            var sliderButton = CreateButton("Slider (WPF Dialog)", 136, ShowSlider);

            Controls.Add(messageButton);
            Controls.Add(inputButton);
            Controls.Add(sliderButton);
        }

        private static Button CreateButton(string text, int top, Func<Task> handler)
        {
            var button = new Button
            {
                Text = text,
                Left = 24,
                Top = top,
                Width = 220,
                Height = 40,
            };
            button.Click += async (_, _) => await handler();
            return button;
        }

        private async Task ShowMessage()
        {
            // 关键：以当前 Form 作为宿主（内部包装为 FormOwner）
            var result = await _provider.ShowMessageDialogAsync(new MessageDialogSettings
            {
                Title = "Message",
                Content = "这是从 WinForms 应用弹出的 WPF 对话框（Win11 风格）。",
            }, this);

            MessageBox.Show(this, $"Result: {result}", "ApeDialogs Demo");
        }

        private async Task ShowInput()
        {
            var result = await _provider.ShowInputDialogAsync(new InputDialogSettings
            {
                Title = "Input",
                Content = "请输入内容：",
                DefaultText = "Hello ApeDialogs",
            }, this);

            MessageBox.Show(this, $"Result: {result}", "ApeDialogs Demo");
        }

        private async Task ShowSlider()
        {
            decimal liveValue = 0;
            var result = await _provider.ShowSliderDialogAsync(new SliderDialogSettings
            {
                Title = "Slider",
                Content = "拖动实时回调：",
                OnValueChanged = value => liveValue = value,
            }, this);

            MessageBox.Show(this, $"Result: {result}（实时回调最后值 {liveValue}）", "ApeDialogs Demo");
        }
    }
}
