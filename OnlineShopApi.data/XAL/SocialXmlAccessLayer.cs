using Microsoft.Extensions.Configuration;
using OnlineShopApi.data.Models;
using System.IO;
using System.Xml;

namespace OnlineShopApi.data.XAL
{
    public class SocialXmlAccessLayer : ISocialXmlAccessLayer
    {
        private string _publishDirectory { get; set; }

        public SocialXmlAccessLayer(IConfiguration config)
        {
            _publishDirectory = config.GetSection("PublishSettings")["DirectoryPath"];
        }

        public void PublishList(SocialList list)
        {
            XmlDocument xml = new XmlDocument();

            XmlElement xSocialList = xml.CreateElement("socialList");
            xSocialList.SetAttribute("name", list.Name);
            xSocialList.SetAttribute("id", list.Id);

            foreach (var post in list.MediaPosts)
            {
                XmlElement xMediaPost = xml.CreateElement("mediaPost");
                xMediaPost.SetAttribute("platform", post.MediaPlatform);
                xMediaPost.SetAttribute("postId", post.PostId);

                XmlElement xUser = xml.CreateElement("user");
                xUser.SetAttribute("displayName", post.DisplayName);
                xUser.SetAttribute("mediaHandle", post.MediaHandle);
                xMediaPost.AppendChild(xUser);

                XmlElement xMain = xml.CreateElement("mainContent");
                xMain.SetAttribute("value", post.MainContent);
                xMediaPost.AppendChild(xMain);

                XmlElement xSecondary = xml.CreateElement("secondaryContent");
                xSecondary.SetAttribute("value", post.SecondaryContent);
                xMediaPost.AppendChild(xSecondary);

                XmlElement xAttachments = xml.CreateElement("attachments");
                foreach (var attachment in post.Attachments)
                {
                    XmlElement xAttachment = xml.CreateElement("attachment");

                    XmlElement xAttachmentUser = xml.CreateElement("user");
                    xAttachmentUser.SetAttribute("displayName", attachment.DisplayName);
                    xAttachmentUser.SetAttribute("mediaHandle", attachment.MediaHandle);
                    xAttachment.AppendChild(xAttachmentUser);

                    XmlElement xAttachmentMain = xml.CreateElement("mainContent");
                    xAttachmentMain.SetAttribute("value", attachment.MainContent);
                    xAttachment.AppendChild(xAttachmentMain);

                    XmlElement xAttachmentSecondary = xml.CreateElement("secondaryContent");
                    xAttachmentSecondary.SetAttribute("value", attachment.SecondaryContent);
                    xAttachment.AppendChild(xAttachmentSecondary);

                    xAttachments.AppendChild(xAttachment);
                }

                xMediaPost.AppendChild(xAttachments);
                xSocialList.AppendChild(xMediaPost);
            }

            xml.AppendChild(xSocialList);
            Directory.CreateDirectory(_publishDirectory);
            var filename = list.Name.Replace(' ', '_') + ".xml";
            xml.Save(Path.Combine(_publishDirectory, filename));
        }
    }
}
