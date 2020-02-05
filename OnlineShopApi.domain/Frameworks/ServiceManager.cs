using Microsoft.Extensions.DependencyInjection;

namespace OnlineShopApi.domain.Frameworks
{
    public class ServiceManager
    {
        public static void InjectServices(IServiceCollection services)
        {
            services.AddHttpClient<IWooliesService, WooliesService>();
        }
    }
}
