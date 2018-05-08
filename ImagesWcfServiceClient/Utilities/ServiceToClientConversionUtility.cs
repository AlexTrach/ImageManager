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
    static class ServiceToClientConversionUtility
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
    }
}
