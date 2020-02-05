using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopApi.presentation.RequestModels
{
    public class NewUser
    {
        [BindRequired]
        public string FirstName { get; set; }

        [BindRequired]
        public string LastName { get; set; }

        [BindRequired]
        [StringLength(128, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 6)]
        public string Username { get; set; }

        [BindRequired]
        [StringLength(128, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 8)]
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
