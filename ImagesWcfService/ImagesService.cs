using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ImagesWcfService
{
    public class ImagesService : IImagesService, IDisposable
    {
        private static Dictionary<string, IImagesServiceCallback> _clients = new Dictionary<string, IImagesServiceCallback>();
        private static object _clientsSyncObject = new object();

        private ImagesDal.ImageRepository _imageRepository = new ImagesDal.ImageRepository();
        private ImagesDal.TagRepository _tagRepository = new ImagesDal.TagRepository();

        private int _numberOfSentThumbnails;

        private Tag[] _tagsToSearchBy = new Tag[] { };
        private int _numberOfSentThumbnailsWithSpecifiedTags;

        public void Subscribe()
        {
            lock (_clientsSyncObject)
            {
                _clients[OperationContext.Current.SessionId] = OperationContext.Current.GetCallbackChannel<IImagesServiceCallback>();
            }
        }

        public Image[] GetNextThumbnails(int numberOfThumbnails, int widthOfThumbnail, bool resetToBeginning)
        {
            if (resetToBeginning)
            {
                _numberOfSentThumbnails = 0;
            }

            List<ImagesDal.Image> imagesFromDatabase = _imageRepository.GetSpecifiedRange(_numberOfSentThumbnails, numberOfThumbnails);
            _numberOfSentThumbnails += imagesFromDatabase.Count;

            return Utility.CreateThumnailsToSendToClient(imagesFromDatabase, widthOfThumbnail);
        }

        public Image[] GetNextThumbnailsWithSuchTags(int numberOfThumbnails, int widthOfThumbnail, Tag[] tags, bool resetToBeginning)
        {
            if (resetToBeginning || Utility.TagArraysAreEqual(_tagsToSearchBy, tags))
            {
                _numberOfSentThumbnailsWithSpecifiedTags = 0;
            }

            List<ImagesDal.Tag> tagsToSearchBy = new List<ImagesDal.Tag>();
            foreach (Tag tag in tags)
            {
                tagsToSearchBy.Add(_tagRepository.GetOne(tag.Id));
            }

            List<ImagesDal.Image> imagesFromDatabase = _imageRepository.GetSpecifiedRangeFromImagesWithSuchTags(_numberOfSentThumbnailsWithSpecifiedTags, numberOfThumbnails, tagsToSearchBy);
            _numberOfSentThumbnails += imagesFromDatabase.Count;

            return Utility.CreateThumnailsToSendToClient(imagesFromDatabase, widthOfThumbnail);
        }

        public Image GetFullSizeImage(int id)
        {
            ImagesDal.Image imageFromDatabase = _imageRepository.GetOne(id);
            Image imageToSendToClient = new Image()
            {
                Id = imageFromDatabase.Id,
                ImageName = imageFromDatabase.ImageName,
                ImageContent = imageFromDatabase.ImageContent,
                Tags = Utility.CreateTagsToSendToClient(imageFromDatabase.Tags)
            };

            return imageToSendToClient;
        }

        
        public Tag[] GetAllTags()
        {
            return Utility.CreateTagsToSendToClient(_tagRepository.GetAll());
        }

        public void AddImage(Image image)
        {
            ImagesDal.Image imageToAdd = new ImagesDal.Image()
            {
                ImageName = image.ImageName,
                ImageContent = image.ImageContent,  
            };
            foreach (Tag tag in image.Tags)
            {
                ImagesDal.Tag tagToAddToImage = _tagRepository.GetOne(tag.Id);
                imageToAdd.Tags.Add(tagToAddToImage);
            }

            _imageRepository.Add(imageToAdd);

            NotifyOtherClientsAboutDatabaseUpdate();
        }

        public void UpdateImage(Image image)
        {
            ImagesDal.Image imageToUpdate = _imageRepository.GetOne(image.Id);

            imageToUpdate.ImageName = image.ImageName;
            imageToUpdate.ImageContent = image.ImageContent;
            imageToUpdate.Tags.Clear();
            foreach (Tag tag in image.Tags)
            {
                ImagesDal.Tag tagToAddToImage = _tagRepository.GetOne(tag.Id);
                imageToUpdate.Tags.Add(tagToAddToImage);
            }

            _imageRepository.Update(imageToUpdate);

            NotifyOtherClientsAboutDatabaseUpdate();
        }

        public void DeleteImage(int id)
        {
            _imageRepository.Delete(id);

            NotifyOtherClientsAboutDatabaseUpdate();
        }

        public void AddTag(Tag tag)
        {
            ImagesDal.Tag newTag = new ImagesDal.Tag()
            {
                TagName = tag.TagName
            };

            _tagRepository.Add(newTag);

            NotifyOtherClientsAboutDatabaseUpdate();
        }

        public void UpdateTag(Tag tag)
        {
            ImagesDal.Tag tagToUpdate = _tagRepository.GetOne(tag.Id);

            tagToUpdate.TagName = tag.TagName;

            _tagRepository.Update(tagToUpdate);

            NotifyOtherClientsAboutDatabaseUpdate();
        }

        public void DeleteTag(int id)
        {
            _tagRepository.Delete(id);

            NotifyOtherClientsAboutDatabaseUpdate();
        }

        public void Unsubscribe()
        {
            lock (_clientsSyncObject)
            {
                _clients.Remove(OperationContext.Current.SessionId);
            }
        }

        private void NotifyOtherClientsAboutDatabaseUpdate()
        {
            lock (_clientsSyncObject)
            {
                foreach (var client in _clients)
                {
                    if (client.Key != OperationContext.Current.SessionId)
                    {
                        if (((ICommunicationObject) client.Value).State == CommunicationState.Opened)
                        {
                            try
                            {
                                client.Value.NotifyAboutDatabaseUpdate();
                            }
                            catch (Exception)
                            {
                                _clients.Remove(client.Key);
                            }
                        }
                        else
                        {
                            _clients.Remove(client.Key);
                        }
                    }
                }
            }
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
                    _imageRepository.Dispose();
                    _tagRepository.Dispose();
                }
            }
            _disposed = true;
        }

        ~ImagesService()
        {
            CleanUp(false);
        }
    }
}
