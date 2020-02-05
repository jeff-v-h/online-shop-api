using OnlineShopApi.data.Models.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShopApi.data.Repositories
{
    public interface IUserRepository
    {
        User Get(string username);
        IEnumerable<User> GetAll();
        Task CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(string username);
    }
}
