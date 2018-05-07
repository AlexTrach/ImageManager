using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media.Imaging;

namespace ImagesWcfService.Utilities
{
    static class Utility
    {
        public static Image[] CreateThumnailsToSendToClient(List<ImagesDal.Image> imagesFromDatabase, int widthOfThumbnail)
        {
            Image[] thumbnailsToSendToClient = new Image[imagesFromDatabase.Count];

            for (int i = 0; i < thumbnailsToSendToClient.Length; i++)
            {
                thumbnailsToSendToClient[i] = CreateThumbnailToSendToClient(imagesFromDatabase[i], widthOfThumbnail);
            }

            return thumbnailsToSendToClient;
        }

        public static Image CreateThumbnailToSendToClient(ImagesDal.Image imageFromDatabase, int widthOfThumbnail)
        {
            return new Image()
            {
                Id = imageFromDatabase.Id,
                ImageName = imageFromDatabase.ImageName,
                ImageContent = CreateThumbnailContent(imageFromDatabase.ImageContent, widthOfThumbnail),
                Tags = CreateTagsToSendToClient(new List<ImagesDal.Tag>(imageFromDatabase.Tags))
            };
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

        public static Tag[] CreateTagsToSendToClient(List<ImagesDal.Tag> tagsFromDatabase)
        {
            Tag[] tagsToSendToClient = new Tag[tagsFromDatabase.Count];

            for (int i = 0; i < tagsToSendToClient.Length; i++)
            {
                tagsToSendToClient[i] = CreateTagToSendToClient(tagsFromDatabase[i]);
            }

            return tagsToSendToClient;
        }

        public static Tag CreateTagToSendToClient(ImagesDal.Tag tagFromDatabase)
        {
            return new Tag()
            {
                Id = tagFromDatabase.Id,
                TagName = tagFromDatabase.TagName
            };
        }

        public static bool TagArraysAreEqual(Tag[] firstTagArray, Tag[] secondTagArray)
        {
            if (firstTagArray.Length != secondTagArray.Length)
            {
                return false;
            }

            for (int i = 0; i < firstTagArray.Length; i++)
            {
                bool isTagFromFirstArrayPresentInSecondArray = false;

                for (int j = 0; j < secondTagArray.Length; j++)
                {
                    if (firstTagArray[i].Id == secondTagArray[j].Id)
                    {
                        isTagFromFirstArrayPresentInSecondArray = true;
                        break;
                    }
                }

                if (!isTagFromFirstArrayPresentInSecondArray)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
