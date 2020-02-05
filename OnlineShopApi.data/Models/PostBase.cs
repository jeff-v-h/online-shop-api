using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShopApi.data.Models
{
    public class PostBase
    {
        public string DisplayName { get; set; }
        public string MediaHandle { get; set; }
        public string MainContent { get; set; }
        public string SecondaryContent { get; set; }
    }
}
