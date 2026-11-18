using System;
using ApeFree.ApeDialogs.Maui;
using ApeFree.ApeDialogs.Maui.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;

namespace ApeFree.ApeDialogs.Maui.Extensions
{
    /// <summary>
    /// MAUI 应用启动注册扩展。
    /// </summary>
    public static class MauiAppBuilderExtensions
    {
        /// <summary>
        /// 注册 ApeDialogs MAUI 提供器。
        /// </summary>
        public static MauiAppBuilder UseApeDialogs(this MauiAppBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddSingleton<ApeDialogsMauiProvider>();
            builder.Services.AddSingleton<DialogProvider<IMauiDialogContext>>(sp => sp.GetRequiredService<ApeDialogsMauiProvider>());
            builder.Services.AddSingleton<IDialogProvider<IMauiDialogContext>>(sp => sp.GetRequiredService<ApeDialogsMauiProvider>());

            return builder;
        }
    }
}
