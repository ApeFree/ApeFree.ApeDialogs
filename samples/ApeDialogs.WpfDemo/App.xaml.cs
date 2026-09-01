using System;
using System.Windows;

namespace ApeDialogs.WpfDemo
{
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += (s, e) =>
            {
                System.IO.File.WriteAllText("crash.txt", e.Exception.ToString());
                MessageBox.Show(e.Exception.ToString(), "Dispatcher Crash");
                e.Handled = true;
                Environment.Exit(1);
            };
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                System.IO.File.WriteAllText("crash.txt", e.ExceptionObject.ToString());
                MessageBox.Show(e.ExceptionObject.ToString(), "AppDomain Crash");
                Environment.Exit(1);
            };
        }
    }
}
