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
    static class ClientToServiceConversionUtility
    {
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
    }
}
