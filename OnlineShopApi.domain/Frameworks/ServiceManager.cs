using Microsoft.Extensions.DependencyInjection;
using PLP.Social.DataService.common.Helpers;
using PLP.Social.DataService.data.Models;
using PLP.Social.DataService.data.Repositories;
using PLP.Social.DataService.data.XAL;

namespace OnlineShopApi.domain.Frameworks
{
    public class ServiceManager
    {
        public static void InjectServices(IServiceCollection services)
        {
            services.AddScoped<ISocialRepository, SocialRepository>();
            services.AddScoped<IDocumentRepository<SocialList>, MongoBaseRepository<SocialList>>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<ISocialXmlAccessLayer, SocialXmlAccessLayer>();

            services.AddScoped<IPasswordHasher, PasswordHasher>();
        }
    }
}
