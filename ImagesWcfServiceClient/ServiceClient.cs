using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using ImagesWcfServiceClient.Utilities;
using ImagesWcfServiceClient.Models;

namespace ImagesWcfServiceClient
{
    public class ServiceClient : IDisposable
    {
        private const int NUMBER_OF_THUMBNAILS = 10;
        private const int WIDTH_OF_THUMBNAIL = 100;

        private ImagesWcfServiceReference.ImagesServiceClient _client;

        private bool _receivedAllAvailableThumbnails = false;
        private bool _receivedAllAvailableThumbnailsWithSuchTags = false;
        private List<Tag> _previousTagsToSearchBy = new List<Tag>();

        public ServiceClient(IDatabaseUpdateListener listener)
        {
            _client = new ImagesWcfServiceReference.ImagesServiceClient(new InstanceContext(new ImagesWcfServiceReference.ImagesServiceCallback(listener)));
            _client.Open();
            _client.Subscribe();
        }

        public List<Image> GetNextThumbnails(bool resetToBeginning)
        {
            if (resetToBeginning)
            {
                _receivedAllAvailableThumbnails = false;
            }

            if (!_receivedAllAvailableThumbnails)
            {
                List<Image> thumbnails = Utility.CreateImages(_client.GetNextThumbnails(NUMBER_OF_THUMBNAILS, WIDTH_OF_THUMBNAIL, resetToBeginning));

                if (thumbnails.Count == 0)
                {
                    _receivedAllAvailableThumbnails = true;
                }

                return thumbnails;
            }
            else
            {
                return new List<Image>();
            }
        }
        
        public List<Image> GetNextThumbnailsWithSuchTags(List<Tag> tags, bool resetToBeginning)
        {
            if (resetToBeginning)
            {
                _receivedAllAvailableThumbnailsWithSuchTags = false;
            }

            if (!Utility.CheckWhetherTagCollectionsAreEqual(_previousTagsToSearchBy, tags))
            {
                _previousTagsToSearchBy.Clear();
                _previousTagsToSearchBy.AddRange(tags);

                _receivedAllAvailableThumbnailsWithSuchTags = false;
            }

            if (tags != null && tags.Count > 0)
            {
                if (!_receivedAllAvailableThumbnailsWithSuchTags)
                {
                    List<Image> thumbnails = Utility.CreateImages(_client.GetNextThumbnailsWithSuchTags(NUMBER_OF_THUMBNAILS, WIDTH_OF_THUMBNAIL, Utility.CreateTagsToSendToService(tags), resetToBeginning));

                    if (thumbnails.Count == 0)
                    {
                        _receivedAllAvailableThumbnailsWithSuchTags = true;
                    }

                    return thumbnails;
                }
                else
                {
                    return new List<Image>();
                }
            }
            else
            {
                throw new ArgumentException("List of tags must not be null and must not be empty.");
            }
        }

        public Image GetThumbnail(int id)
        {
            ImagesWcfServiceReference.Image thumbnailFromService = _client.GetThumbnail(WIDTH_OF_THUMBNAIL, id);
            if (thumbnailFromService == null)
            {
                return null;
            }
            else
            {
                return Utility.CreateImage(thumbnailFromService);
            }
        }
        
        public Image GetFullSizeImage(int id)
        {
            ImagesWcfServiceReference.Image imageFromService = _client.GetFullSizeImage(id);
            if (imageFromService == null)
            {
                return null;
            }
            else
            {
                return Utility.CreateImage(imageFromService);
            }
        }
        
        public List<Tag> GetAllTags()
        {
            return Utility.CreateTags(_client.GetAllTags());
        }

        public Tag GetTag(int id)
        {
            ImagesWcfServiceReference.Tag tagFromService = _client.GetTag(id);
            if (tagFromService == null)
            {
                return null;
            }
            else
            {
                return Utility.CreateTag(tagFromService);
            }
        }
        
        public void AddImage(Image image)
        {
            _client.AddImage(Utility.CreateImageToSendToService(image));
        }
        
        public void UpdateImage(Image image)
        {
            _client.UpdateImage(Utility.CreateImageToSendToService(image));
        }

        public void DeleteImage(Image image)
        {
            _client.DeleteImage(image.Id);
        }
        
        public void AddTag(Tag tag)
        {
            _client.AddTag(Utility.CreateTagToSendToService(tag));
        }
        
        public void UpdateTag(Tag tag)
        {
            _client.UpdateTag(Utility.CreateTagToSendToService(tag));
        }
        
        public void DeleteTag(Tag tag)
        {
            _client.DeleteTag(tag.Id);
        }

        private bool _disposed = false;

        public void Dispose()
        {
            CleanUp(true);
            GC.SuppressFinalize(this);
        }

        private void CleanUp(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _client.Unsubscribe();
                    _client.Close();
                }
            }
            _disposed = true;
        }

        ~ServiceClient()
        {
            CleanUp(false);
        }
    }
}
