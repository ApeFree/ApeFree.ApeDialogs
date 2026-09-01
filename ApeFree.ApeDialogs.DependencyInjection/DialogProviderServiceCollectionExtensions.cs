using System;
using Microsoft.Extensions.DependencyInjection;

namespace ApeFree.ApeDialogs
{
    /// <summary>
    /// DialogProvider 的依赖注入注册扩展。
    /// </summary>
    public static class DialogProviderServiceCollectionExtensions
    {
        /// <summary>
        /// 注册 DialogProvider（以具体类型与接口两种方式注册，便于解析与替换）。
        /// </summary>
        /// <typeparam name="TContext">上下文类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="provider">Dialog 提供器实例</param>
        public static IServiceCollection AddApeDialogs<TContext>(this IServiceCollection services, DialogProvider<TContext> provider)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (provider is null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            services.AddSingleton(provider);
            services.AddSingleton<IDialogProvider<TContext>>(provider);
            return services;
        }
    }
}
