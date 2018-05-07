using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media.Imaging;
using ImagesWcfServiceClient.Models;

namespace ImagesWcfServiceClient.Utilities
{
    static class Utility
    {
        public static List<Image> CreateImages(ImagesWcfServiceReference.Image[] imagesFromService)
        {
            List<Image> images = new List<Image>(imagesFromService.Length);

            foreach (ImagesWcfServiceReference.Image imageFromService in imagesFromService)
            {
                images.Add(CreateImage(imageFromService));
            }

            return images;
        }

        public static Image CreateImage(ImagesWcfServiceReference.Image imageFromService)
        {
            return new Image()
            {
                Id = imageFromService.Id,
                ImageName = imageFromService.ImageName,
                ImageContent = ConvertByteArrayToBitmapImage(imageFromService.ImageContent),
                Tags = CreateTags(imageFromService.Tags)
            };
        }

        private static BitmapImage ConvertByteArrayToBitmapImage(byte[] imageContent)
        {
            BitmapImage bitmapImage = new BitmapImage();

            using (MemoryStream memorySteam = new MemoryStream(imageContent))
            {
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memorySteam;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }

            return bitmapImage;
        }

        public static ImagesWcfServiceReference.Image CreateImageToSendToService(Image image)
        {
            if (ValidateName(image.ImageName))
            {
                return new ImagesWcfServiceReference.Image()
                {
                    Id = image.Id,
                    ImageName = image.ImageName,
                    ImageContent = ConvertBitmapImageToByteArray(image.ImageContent),
                    Tags = CreateTagsToSendToService(image.Tags)
                };
            }
            else
            {
                throw new ArgumentException("Image name must be not empty and must not exceed 100 characters.");
            }
        }

        private static byte[] ConvertBitmapImageToByteArray(BitmapImage image)
        {
            byte[] byteArray;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(memoryStream);
                byteArray = memoryStream.ToArray();
            }

            return byteArray;
        }

        public static ImagesWcfServiceReference.Tag[] CreateTagsToSendToService(List<Tag> tags)
        {
            ImagesWcfServiceReference.Tag[] tagsToSendToService = new ImagesWcfServiceReference.Tag[tags.Count];

            for (int i = 0; i < tagsToSendToService.Length; i++)
            {
                tagsToSendToService[i] = CreateTagToSendToService(tags[i]);
            }

            return tagsToSendToService;
        }

        public static ImagesWcfServiceReference.Tag CreateTagToSendToService(Tag tag)
        {
            if (ValidateName(tag.TagName))
            {
                return new ImagesWcfServiceReference.Tag() { Id = tag.Id, TagName = tag.TagName };
            }
            else
            {
                throw new ArgumentException("Tag name must be not empty and must not exceed 100 characters.");
            }
        }

        private static bool ValidateName(string name)
        {
            return name.Length <= 100 && name != string.Empty;
        }

        public static List<Tag> CreateTags(ImagesWcfServiceReference.Tag[] tagsFromService)
        {
            List<Tag> tags = new List<Tag>(tagsFromService.Length);

            foreach (ImagesWcfServiceReference.Tag tagFromService in tagsFromService)
            {
                tags.Add(CreateTag(tagFromService));
            }

            return tags;
        }

        public static Tag CreateTag(ImagesWcfServiceReference.Tag tagFromService)
        {
            return new Tag() { Id = tagFromService.Id, TagName = tagFromService.TagName };
        }

        public static bool CheckWhetherTagCollectionsAreEqual(List<Tag> firstTagCollection, List<Tag> secondTagCollection)
        {
            if (firstTagCollection.Count != secondTagCollection.Count)
            {
                return false;
            }

            for (int i = 0; i < firstTagCollection.Count; i++)
            {
                bool isTagFromFirstArrayPresentInSecondArray = false;

                for (int j = 0; j < secondTagCollection.Count; j++)
                {
                    if (firstTagCollection[i].Id == secondTagCollection[j].Id)
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
