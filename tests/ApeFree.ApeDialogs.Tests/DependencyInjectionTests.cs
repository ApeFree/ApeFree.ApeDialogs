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
        public void AddApeDialogs_Throws_OnNullProvider()
        {
            var services = new ServiceCollection();
            Assert.Throws<System.ArgumentNullException>(() => services.AddApeDialogs<object>(null!));
        }

        [Fact]
        public void AddApeDialogs_Throws_OnNullServices()
        {
            Assert.Throws<System.ArgumentNullException>(() => ((IServiceCollection?)null!).AddApeDialogs(new TestProvider()));
        }
    }
}
