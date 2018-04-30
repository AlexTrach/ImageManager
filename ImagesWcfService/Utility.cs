using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media.Imaging;

namespace ImagesWcfService
{
    static class Utility
    {
        public static Image[] CreateThumnailsToSendToClient(List<ImagesDal.Image> imagesFromDatabase, int widthOfThumbnail)
        {
            Image[] thumbnailsToSendToClient = new Image[imagesFromDatabase.Count];

            for (int i = 0; i < thumbnailsToSendToClient.Length; i++)
            {
                thumbnailsToSendToClient[i] = new Image()
                {
                    Id = imagesFromDatabase[i].Id,
                    ImageName = imagesFromDatabase[i].ImageName,
                    ImageContent = CreateThumbnailContent(imagesFromDatabase[i].ImageContent, widthOfThumbnail),
                    Tags = CreateTagsToSendToClient(imagesFromDatabase[i].Tags)
                };
            }

            return thumbnailsToSendToClient;
        }

        public static byte[] CreateThumbnailContent(byte[] imageContent, int widthOfThumbnail)
        {
            byte[] thumbnailContent;
            BitmapImage thumbnail = new BitmapImage();

            using (MemoryStream memoryStream = new MemoryStream(imageContent))
            {
                thumbnail.BeginInit();
                thumbnail.CacheOption = BitmapCacheOption.OnLoad;
                thumbnail.StreamSource = memoryStream;
                thumbnail.DecodePixelWidth = widthOfThumbnail;
                thumbnail.EndInit();
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(thumbnail));
                encoder.Save(memoryStream);
                thumbnailContent = memoryStream.ToArray();
            }

            return thumbnailContent;
        }

        public static Tag[] CreateTagsToSendToClient(ICollection<ImagesDal.Tag> tagsFromDatabase)
        {
            Tag[] tagsToSendToClient = new Tag[tagsFromDatabase.Count];

            for (int i = 0; i < tagsToSendToClient.Length; i++)
            {
                ImagesDal.Tag tagFromDatabase = tagsFromDatabase.ElementAt(i);
                tagsToSendToClient[i] = new Tag()
                {
                    Id = tagFromDatabase.Id,
                    TagName = tagFromDatabase.TagName
                };
            }

            return tagsToSendToClient;
        }

        public static bool TagArraysAreEqual(Tag[] firstTagArray, Tag[] secondTagArray)
        {
            if (firstTagArray.Length != secondTagArray.Length)
            {
                return false;
            }

            for (int i = 0; i < firstTagArray.Length; i++)
            {
                if (firstTagArray[i].Id != secondTagArray[i].Id)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
