using System;
using System.Collections.Generic;

namespace OnlineShopApi.domain.Models.ViewModels
{
    public class MediaPostVM : PostBaseVM
    {
        public string PostId { get; set; }
        public string MediaPlatform { get; set; }
        public List<AttachmentVM> Attachments { get; set; }
        public string AddedBy { get; set; }
        public DateTime? TimeAdded { get; set; }
    }
}
