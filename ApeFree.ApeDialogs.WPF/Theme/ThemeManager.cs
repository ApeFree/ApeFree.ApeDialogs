using System;
using System.Windows;
using System.Windows.Media;
using ApeFree.ApeDialogs.Theme;

namespace ApeFree.ApeDialogs.WPF.Theme
{
    /// <summary>
    /// 主题管理器：加载浅/深色令牌字典、跟随系统深浅色、提供 DialogOptionTag → 颜色映射。
    /// 必须在 UI 线程调用 <see cref="EnsureLoaded"/>（对话框宿主构造时自动调用）。
    /// </summary>
    public static class ThemeManager
    {
        private const string LightDictionaryUri = "pack://application:,,,/ApeFree.ApeDialogs.WPF;component/Theme/Brushes.Light.xaml";
        private const string DarkDictionaryUri = "pack://application:,,,/ApeFree.ApeDialogs.WPF;component/Theme/Brushes.Dark.xaml";
        private const string GenericDictionaryUri = "pack://application:,,,/ApeFree.ApeDialogs.WPF;component/Themes/Generic.xaml";

        private static readonly object SyncRoot = new object();
        private static ResourceDictionary? _genericDictionary;
        private static ResourceDictionary? _lightDictionary;
        private static ResourceDictionary? _darkDictionary;
        private static ResourceDictionary? _currentDictionary;
        private static bool _followSystemTheme = true;
        private static bool _systemEventsHooked;

        /// <summary>
        /// 当前是否为深色模式。
        /// </summary>
        public static bool IsDarkMode { get; private set; }

        /// <summary>
        /// 主题切换事件（始终在 UI 线程触发）。
        /// </summary>
        public static event EventHandler? ThemeChanged;

        /// <summary>
        /// 是否跟随系统深浅色（默认 true）。
        /// </summary>
        public static bool FollowSystemTheme
        {
            get => _followSystemTheme;
            set
            {
                _followSystemTheme = value;
                if (value)
                {
                    ApplyTheme(DetectSystemDarkMode());
                    HookSystemThemeEvents();
                }
            }
        }

        /// <summary>
        /// 确保主题资源已加载到 Application 资源中（幂等）。
        /// </summary>
        public static void EnsureLoaded(Application? application = null)
        {
            if (application is null && Application.Current is null)
            {
                try
                {
                    _ = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
                }
                catch
                {
                    // Ignore if already created or in unsupported environment
                }
            }

            application = application ?? Application.Current;
            if (application is null)
            {
                return;
            }

            lock (SyncRoot)
            {
                if (_genericDictionary is null)
                {
                    _genericDictionary = new ResourceDictionary { Source = new Uri(GenericDictionaryUri, UriKind.Absolute) };
                }

                if (_lightDictionary is null)
                {
                    _lightDictionary = new ResourceDictionary { Source = new Uri(LightDictionaryUri, UriKind.Absolute) };
                    _darkDictionary = new ResourceDictionary { Source = new Uri(DarkDictionaryUri, UriKind.Absolute) };
                }

                EnsureGenericDictionaryMerged(application);

                if (_currentDictionary is null)
                {
                    ApplyTheme(DetectSystemDarkMode());
                }
                else
                {
                    EnsureDictionaryMerged(application);
                }

                HookSystemThemeEvents();
            }
        }

        /// <summary>
        /// 手动设置深浅色。
        /// </summary>
        public static void SetDarkMode(bool isDark)
        {
            _followSystemTheme = false;
            ApplyTheme(isDark);
        }

        /// <summary>
        /// 获取主题资源（Brush / FontFamily / CornerRadius 等）；不存在返回 null。
        /// </summary>
        public static object? GetResource(string key)
        {
            var application = Application.Current;
            if (application is null)
            {
                return null;
            }

            return application.TryFindResource(key);
        }

        /// <summary>
        /// 获取主题色（供非 XAML 代码使用）。
        /// </summary>
        public static Color GetColor(string key)
        {
            return (GetResource(key) as SolidColorBrush)?.Color ?? Colors.Transparent;
        }

        private static void HookSystemThemeEvents()
        {
            if (_systemEventsHooked)
            {
                return;
            }

            _systemEventsHooked = true;
            Microsoft.Win32.SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
        }

        private static void OnUserPreferenceChanged(object sender, Microsoft.Win32.UserPreferenceChangedEventArgs e)
        {
            if (!_followSystemTheme)
            {
                return;
            }

            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher is null)
            {
                return;
            }

            if (dispatcher.CheckAccess())
            {
                ApplyTheme(DetectSystemDarkMode());
            }
            else
            {
                dispatcher.BeginInvoke(new Action(() => ApplyTheme(DetectSystemDarkMode())));
            }
        }

        /// <summary>
        /// 检测系统深浅色（注册表 AppsUseLightTheme，0 = 深色）。
        /// </summary>
        private static bool DetectSystemDarkMode()
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    var value = key?.GetValue("AppsUseLightTheme");
                    return value is int i && i == 0;
                }
            }
            catch
            {
                return false;
            }
        }

        private static void ApplyTheme(bool isDark)
        {
            lock (SyncRoot)
            {
                IsDarkMode = isDark;
                _currentDictionary = isDark ? _darkDictionary : _lightDictionary;

                var application = Application.Current;
                if (application is null)
                {
                    return;
                }

                var merged = application.Resources.MergedDictionaries;
                merged.Remove(_darkDictionary);
                merged.Remove(_lightDictionary);

                if (_currentDictionary is not null)
                {
                    merged.Insert(0, _currentDictionary);
                }

                EnsureGenericDictionaryMerged(application);

                ThemeChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        private static void EnsureGenericDictionaryMerged(Application application)
        {
            var merged = application.Resources.MergedDictionaries;
            if (_genericDictionary is not null && !merged.Contains(_genericDictionary))
            {
                merged.Add(_genericDictionary);
            }
        }

        private static void EnsureDictionaryMerged(Application application)
        {
            var merged = application.Resources.MergedDictionaries;
            if (_currentDictionary is not null && !merged.Contains(_currentDictionary))
            {
                merged.Insert(0, _currentDictionary);
            }
        }
    }

    /// <summary>
    /// Win11 语义色主题：将 DialogOptionTag 映射为 Win11 语义色（随深浅色变化）。
    /// </summary>
    public sealed class Win11Theme : IDialogTheme
    {
        /// <summary>
        /// 单例。
        /// </summary>
        public static Win11Theme Instance { get; } = new Win11Theme();

        private Win11Theme()
        {
        }

        /// <inheritdoc/>
        public ApeColor ReminderColor
        {
            get
            {
                var color = ThemeManager.GetColor(ThemeManager.IsDarkMode ? "ApeAccentTextPrimary" : "ApeAccentTextPrimary");
                return new ApeColor(color.A, color.R, color.G, color.B);
            }
        }

        /// <inheritdoc/>
        public ApeColor GetColor(DialogOptionTag tag)
        {
            var key = tag switch
            {
                DialogOptionTag.Positive => "ApePositiveBrush",
                DialogOptionTag.Negative => "ApeNegativeBrush",
                DialogOptionTag.Special => "ApeSpecialBrush",
                DialogOptionTag.Functional => "ApeFunctionalBrush",
                _ => "ApeAccentFillDefault",
            };

            var color = ThemeManager.GetColor(key);
            return new ApeColor(color.A, color.R, color.G, color.B);
        }
    }
}
