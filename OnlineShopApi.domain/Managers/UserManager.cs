using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OnlineShopApi.common.Helpers;

using OnlineShopApi.data.Helpers;
using OnlineShopApi.data.Models.User;
using OnlineShopApi.data.Repositories;
using OnlineShopApi.domain.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopApi.domain.Managers
{
    public class UserManager : IUserManager
    {
        private IUserRepository _repo;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _hasher;
        private readonly AuthSettings _authSettings;

        public UserManager(IUserRepository repo, IMapper mapper, IOptions<AuthSettings> options, IPasswordHasher hasher)
        {
            _repo = repo;
            _mapper = mapper;
            _hasher = hasher;
            _authSettings = options.Value;
        }

        // If returning null, no username matches.
        // If no JWT token, username found but password incorrect
        public UserVM Authenticate(string username, string password)
        {
            var user = _repo.Get(username);
            if (user == null) return null;

            var pwCheck = _hasher.Check(user.Password, password);
            if (!pwCheck.Verified) return null;

            // Authentication successful so generate jwt token
            user.Token = GetJWT(user);

            return _mapper.Map<UserVM>(user.WithoutPassword());
        }

        private string GetJWT(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_authSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public IEnumerable<UserVM> GetAll()
        {
            var users = _repo.GetAll();
            return _mapper.Map<IEnumerable<User>, List<UserVM>>(users.WithoutPasswords());
        }

        public async Task<UserVM> CreateUser(UserVM userVM, string password)
        {
            // Ensure username is unique
            if (_repo.Get(userVM.Username) != null) return null;

            if (userVM.Role == null) userVM.Role = Role.User;
            var user = _mapper.Map<User>(userVM);
            user.Created = DateTime.UtcNow;
            user.Password = _hasher.Hash(password);
            await _repo.CreateAsync(user);

            userVM.Id = user.Id;
            return userVM;
        }

        public async Task<bool> UpdatePassword(string username, string oldPw, string newPw)
        {
            var user = _repo.Get(username);
            if (user == null) return false;

            var pwCheck = _hasher.Check(user.Password, oldPw);
            if (!pwCheck.Verified) return false;

            user.Password = _hasher.Hash(newPw);
            await _repo.UpdateAsync(user);

            return true;
        }

        // Update pw separately in another method
        public async Task<bool> UpdateUser(UserVM userVM)
        {
            var user = _repo.Get(userVM.Username);
            if (user == null) return false;

            var updatedUser = _mapper.Map<User>(userVM);
            updatedUser.Created = user.Created;
            updatedUser.Password = user.Password;

            await _repo.UpdateAsync(updatedUser);

            return true;
        }

        public async Task DeleteUser(string username) =>
            await _repo.DeleteAsync(username);
    }
}
