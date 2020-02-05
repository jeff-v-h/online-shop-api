using System;
using System.Collections.Generic;

namespace OnlineShopApi.data.Models
{
    public class MediaPost : PostBase
    {
        public string PostId { get; set; }
        public string MediaPlatform { get; set; }
        public List<Attachment> Attachments { get; set; }
        public string AddedBy { get; set; }
        public DateTime? TimeAdded { get; set; }
    }
}
