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

        /// <summary>
        /// 通过类型自动注册 DialogProvider（由 DI 容器解析其构造函数依赖项）。
        /// </summary>
        /// <typeparam name="TContext">上下文类型</typeparam>
        /// <typeparam name="TProvider">具体提供器类型</typeparam>
        /// <param name="services">服务集合</param>
        public static IServiceCollection AddApeDialogs<TContext, TProvider>(this IServiceCollection services)
            where TProvider : DialogProvider<TContext>
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<TProvider>();
            services.AddSingleton<DialogProvider<TContext>>(sp => sp.GetRequiredService<TProvider>());
            services.AddSingleton<IDialogProvider<TContext>>(sp => sp.GetRequiredService<TProvider>());
            return services;
        }

        /// <summary>
        /// 通过工厂委托注册 DialogProvider。
        /// </summary>
        /// <typeparam name="TContext">上下文类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="implementationFactory">提供器工厂委托</param>
        public static IServiceCollection AddApeDialogs<TContext>(
            this IServiceCollection services,
            Func<IServiceProvider, DialogProvider<TContext>> implementationFactory)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (implementationFactory is null)
            {
                throw new ArgumentNullException(nameof(implementationFactory));
            }

            services.AddSingleton(implementationFactory);
            services.AddSingleton<IDialogProvider<TContext>>(implementationFactory);
            return services;
        }

        /// <summary>
        /// 通过工厂委托注册具体 DialogProvider。
        /// </summary>
        /// <typeparam name="TContext">上下文类型</typeparam>
        /// <typeparam name="TProvider">具体提供器类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="implementationFactory">提供器工厂委托</param>
        public static IServiceCollection AddApeDialogs<TContext, TProvider>(
            this IServiceCollection services,
            Func<IServiceProvider, TProvider> implementationFactory)
            where TProvider : DialogProvider<TContext>
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (implementationFactory is null)
            {
                throw new ArgumentNullException(nameof(implementationFactory));
            }

            services.AddSingleton(implementationFactory);
            services.AddSingleton<DialogProvider<TContext>>(sp => sp.GetRequiredService<TProvider>());
            services.AddSingleton<IDialogProvider<TContext>>(sp => sp.GetRequiredService<TProvider>());
            return services;
        }
    }
}
