using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;

namespace ApeFree.ApeDialogs
{
    /// <summary>
    /// Dialog 提供器接口（跨 UI 框架的统一入口）。
    /// </summary>
    /// <typeparam name="TContext">上下文类型（如主窗体、页面、Activity）</typeparam>
    public interface IDialogProvider<TContext>
    {
        /// <summary>
        /// 创建消息对话框。
        /// </summary>
        IDialog<bool> CreateMessageDialog(MessageDialogSettings settings, TContext? context = default);

        /// <summary>
        /// 创建提示对话框。
        /// </summary>
        IDialog<bool> CreatePromptDialog(PromptDialogSettings settings, TContext? context = default);

        /// <summary>
        /// 创建输入对话框。
        /// </summary>
        IDialog<string> CreateInputDialog(InputDialogSettings settings, TContext? context = default);

        /// <summary>
        /// 创建密码对话框。
        /// </summary>
        IDialog<string> CreatePasswordDialog(PasswordDialogSettings settings, TContext? context = default);

        /// <summary>
        /// 创建单选对话框。
        /// </summary>
        /// <remarks>注意：默认选中项参数使用未加注解的 T（实现方重写时可正常使用；可为 null）。</remarks>
        IDialog<T> CreateSelectionDialog<T>(SelectionDialogSettings<T> settings, IEnumerable<T> collection, T defaultSelectedItem = default!, TContext? context = default);

        /// <summary>
        /// 创建多选对话框。
        /// </summary>
        IDialog<IReadOnlyList<T>> CreateMultipleSelectionDialog<T>(MultipleSelectionDialogSettings<T> settings, IEnumerable<T> collection, IEnumerable<T>? defaultSelectedItems = null, TContext? context = default);

        /// <summary>
        /// 创建日期时间对话框。
        /// </summary>
        IDialog<DateTime> CreateDateTimeDialog(DateTimeDialogSettings settings, TContext? context = default);

        /// <summary>
        /// 创建多字段表单对话框。
        /// </summary>
        IDialog<DataEntrySheet> CreateDataEntrySheetDialog(DataEntrySheet sheet, DataEntrySheetDialogSettings settings, TContext? context = default);

        /// <summary>
        /// 创建打开文件对话框。
        /// </summary>
        IDialog<IReadOnlyList<string>> CreateOpenFileDialog(OpenFileDialogSettings settings, TContext? context = default);

        /// <summary>
        /// 创建打开文件夹对话框。
        /// </summary>
        IDialog<IReadOnlyList<string>> CreateOpenFolderDialog(OpenFolderDialogSettings settings, TContext? context = default);

        /// <summary>
        /// 创建数值输入对话框。
        /// </summary>
        IDialog<decimal> CreateNumberInputDialog(NumberInputDialogSettings settings, TContext? context = default);

        /// <summary>
        /// 创建单滑块调节对话框（调节过程实时触发 <see cref="IAdjustDialog{T}.ValueChanged"/>）。
        /// </summary>
        IAdjustDialog<decimal> CreateSliderDialog(SliderDialogSettings settings, TContext? context = default);

        /// <summary>
        /// 创建双滑块范围调节对话框。
        /// </summary>
        IAdjustDialog<ValueRange<decimal>> CreateRangeSliderDialog(RangeSliderDialogSettings settings, TContext? context = default);

        /// <summary>
        /// 创建进度对话框。
        /// </summary>
        IProgressDialog CreateProgressDialog(ProgressDialogSettings settings, TContext? context = default);

        /// <summary>
        /// 创建轻提示对话框（按 Duration 自动消失，点击返回确认结果）。
        /// </summary>
        IDialog<bool> CreateToastDialog(ToastDialogSettings settings, TContext? context = default);

        /// <summary>
        /// 创建长文本查看对话框。
        /// </summary>
        IDialog<bool> CreateTextDialog(TextDialogSettings settings, TContext? context = default);

        /// <summary>
        /// 创建颜色选择对话框。
        /// </summary>
        IDialog<ApeColor> CreateColorDialog(ColorDialogSettings settings, TContext? context = default);

        /// <summary>
        /// 创建保存文件对话框。
        /// </summary>
        IDialog<string> CreateSaveFileDialog(SaveFileDialogSettings settings, TContext? context = default);

        /// <summary>
        /// 创建可编辑下拉输入对话框。
        /// </summary>
        IDialog<string> CreateComboBoxInputDialog(ComboBoxInputDialogSettings settings, TContext? context = default);
    }
}
