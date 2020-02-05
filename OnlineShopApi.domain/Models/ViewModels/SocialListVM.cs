using System.Collections.Generic;

namespace OnlineShopApi.domain.Models.ViewModels
{
    public class SocialListVM : SocialListBaseVM
    {
        public List<MediaPostVM> MediaPosts { get; set; }

        public SocialListVM() { }
    }
}
