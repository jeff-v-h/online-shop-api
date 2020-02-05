using System;
using System.Collections.Generic;

namespace OnlineShopApi.domain.Models.ViewModels
{
    public class SocialMediaListsDetailsVM
    {
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public List<SocialListBaseVM> Lists { get; set; }
    }
}
