using System;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ApeFree.ApeDialogs.Tests
{
    public class DependencyInjectionTests
    {
        [Fact]
        public void AddApeDialogs_RegistersProvider()
        {
            var services = new ServiceCollection();
            var provider = new TestProvider();

            services.AddApeDialogs(provider);

            using var sp = services.BuildServiceProvider();
            Assert.Same(provider, sp.GetRequiredService<DialogProvider<object>>());
            Assert.Same(provider, sp.GetRequiredService<IDialogProvider<object>>());
        }

        [Fact]
        public void AddApeDialogs_GenericType_RegistersAndResolves()
        {
            var services = new ServiceCollection();
            services.AddApeDialogs<object, TestProvider>();

            using var sp = services.BuildServiceProvider();
            var concrete = sp.GetRequiredService<TestProvider>();
            var baseProvider = sp.GetRequiredService<DialogProvider<object>>();
            var interfaceProvider = sp.GetRequiredService<IDialogProvider<object>>();

            Assert.NotNull(concrete);
            Assert.Same(concrete, baseProvider);
            Assert.Same(concrete, interfaceProvider);
        }

        [Fact]
        public void AddApeDialogs_Factory_RegistersAndResolves()
        {
            var services = new ServiceCollection();
            var instance = new TestProvider();
            services.AddApeDialogs<object>(sp => instance);

            using var sp = services.BuildServiceProvider();
            var resolved = sp.GetRequiredService<IDialogProvider<object>>();
            Assert.Same(instance, resolved);
        }

        [Fact]
        public void AddApeDialogs_Throws_OnNullProvider()
        {
            var services = new ServiceCollection();
            Assert.Throws<System.ArgumentNullException>(() => services.AddApeDialogs<object>((DialogProvider<object>)null!));
            Assert.Throws<System.ArgumentNullException>(() => services.AddApeDialogs<object>((Func<System.IServiceProvider, DialogProvider<object>>)null!));
        }

        [Fact]
        public void AddApeDialogs_Throws_OnNullServices()
        {
            Assert.Throws<System.ArgumentNullException>(() => ((IServiceCollection?)null!).AddApeDialogs(new TestProvider()));
        }
    }
}
