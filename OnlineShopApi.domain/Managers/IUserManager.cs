using OnlineShopApi.domain.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShopApi.domain.Managers
{
    public interface IUserManager
    {
        UserVM Authenticate(string username, string password);
        IEnumerable<UserVM> GetAll();
        Task<UserVM> CreateUser(UserVM userVM, string password);
        Task<bool> UpdatePassword(string username, string oldPw, string newPw);
        Task<bool> UpdateUser(UserVM userVM);
        Task DeleteUser(string username);
    }
}
