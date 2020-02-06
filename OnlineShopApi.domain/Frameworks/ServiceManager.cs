using Microsoft.Extensions.DependencyInjection;
using OnlineShopApi.data.Repositories;

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
