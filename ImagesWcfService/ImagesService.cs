using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ImagesWcfService.Utilities;

namespace ImagesWcfService
{
    public class ImagesService : IImagesService, IDisposable
    {
        private static Dictionary<string, IImagesServiceCallback> _clients = new Dictionary<string, IImagesServiceCallback>();
        private static object _clientsSyncObject = new object();

        private ImagesDal.ImagesRepository _imagesRepository = new ImagesDal.ImagesRepository();

        private int _numberOfSentThumbnails;

        private Tag[] _previousTagsToSearchBy = new Tag[] { };
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

            List<ImagesDal.Image> imagesFromDatabase = _imagesRepository.GetSpecifiedRangeOfImages(_numberOfSentThumbnails, numberOfThumbnails);
            _numberOfSentThumbnails += imagesFromDatabase.Count;

            return Utility.CreateThumnailsToSendToClient(imagesFromDatabase, widthOfThumbnail);
        }

        public Image[] GetNextThumbnailsWithSuchTags(int numberOfThumbnails, int widthOfThumbnail, Tag[] tags, bool resetToBeginning)
        {
            if (resetToBeginning)
            {
                _numberOfSentThumbnailsWithSpecifiedTags = 0;
            }

            if (!Utility.TagArraysAreEqual(_previousTagsToSearchBy, tags))
            {
                _previousTagsToSearchBy = new Tag[tags.Length];
                for (int i = 0; i < tags.Length; i++)
                {
                    _previousTagsToSearchBy[i] = tags[i];
                }

                _numberOfSentThumbnailsWithSpecifiedTags = 0;
            }

            List<ImagesDal.Tag> tagsToSearchBy = new List<ImagesDal.Tag>();
            foreach (Tag tag in tags)
            {
                tagsToSearchBy.Add(_imagesRepository.GetOneTag(tag.Id));
            }

            List<ImagesDal.Image> imagesFromDatabase = _imagesRepository.GetSpecifiedRangeFromImagesWithSuchTags(_numberOfSentThumbnailsWithSpecifiedTags, numberOfThumbnails, tagsToSearchBy);
            _numberOfSentThumbnailsWithSpecifiedTags += imagesFromDatabase.Count;

            return Utility.CreateThumnailsToSendToClient(imagesFromDatabase, widthOfThumbnail);
        }

        public Image GetThumbnail(int widthOfThumbnail, int id)
        {
            ImagesDal.Image imageFromDatabase = _imagesRepository.GetOneImage(id);
            if (imageFromDatabase == null)
            {
                return null;
            }
            else
            {
                return Utility.CreateThumbnailToSendToClient(imageFromDatabase, widthOfThumbnail);
            }   
        }

        public Image GetFullSizeImage(int id)
        {
            ImagesDal.Image imageFromDatabase = _imagesRepository.GetOneImage(id);
            if (imageFromDatabase == null)
            {
                return null;
            }
            else
            {
                return new Image()
                {
                    Id = imageFromDatabase.Id,
                    ImageName = imageFromDatabase.ImageName,
                    ImageContent = imageFromDatabase.ImageContent,
                    Tags = Utility.CreateTagsToSendToClient(new List<ImagesDal.Tag>(imageFromDatabase.Tags))
                };
            }
        }

        
        public Tag[] GetAllTags()
        {
            return Utility.CreateTagsToSendToClient(_imagesRepository.GetAllTags());
        }

        public Tag GetTag(int id)
        {
            ImagesDal.Tag tagFromDatabase = _imagesRepository.GetOneTag(id);
            if (tagFromDatabase == null)
            {
                return null;
            }
            else
            {
                return Utility.CreateTagToSendToClient(tagFromDatabase);
            }
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
                ImagesDal.Tag tagToAddToImage = _imagesRepository.GetOneTag(tag.Id);
                imageToAdd.Tags.Add(tagToAddToImage);
            }

            _imagesRepository.AddImage(imageToAdd);

            NotifyOtherClientsAboutDatabaseUpdate(new EntityChangeInfo(imageToAdd.Id, EntityType.Image, EntityState.Added));
        }

        public void UpdateImage(Image image)
        {
            ImagesDal.Image imageToUpdate = _imagesRepository.GetOneImage(image.Id);

            imageToUpdate.ImageName = image.ImageName;
            imageToUpdate.ImageContent = image.ImageContent;
            imageToUpdate.Tags.Clear();
            foreach (Tag tag in image.Tags)
            {
                ImagesDal.Tag tagToAddToImage = _imagesRepository.GetOneTag(tag.Id);
                imageToUpdate.Tags.Add(tagToAddToImage);
            }
 
            if (_imagesRepository.UpdateImage(imageToUpdate) != 0)
            {
                NotifyOtherClientsAboutDatabaseUpdate(new EntityChangeInfo(imageToUpdate.Id, EntityType.Image, EntityState.Modified));
            }
        }

        public void DeleteImage(int id)
        {
            if (_imagesRepository.DeleteImage(id) != 0)
            {
                NotifyOtherClientsAboutDatabaseUpdate(new EntityChangeInfo(id, EntityType.Image, EntityState.Deleted));
            }
        }

        public void AddTag(Tag tag)
        {
            ImagesDal.Tag tagToAdd = new ImagesDal.Tag()
            {
                TagName = tag.TagName
            };

            _imagesRepository.AddTag(tagToAdd);

            NotifyOtherClientsAboutDatabaseUpdate(new EntityChangeInfo(tagToAdd.Id, EntityType.Tag, EntityState.Added));
        }

        public void UpdateTag(Tag tag)
        {
            ImagesDal.Tag tagToUpdate = _imagesRepository.GetOneTag(tag.Id);

            tagToUpdate.TagName = tag.TagName;

            if (_imagesRepository.UpdateTag(tagToUpdate) != 0)
            {
                NotifyOtherClientsAboutDatabaseUpdate(new EntityChangeInfo(tagToUpdate.Id, EntityType.Tag, EntityState.Modified));
            }
        }

        public void DeleteTag(int id)
        {
            if (_imagesRepository.DeleteTag(id) != 0)
            {
                NotifyOtherClientsAboutDatabaseUpdate(new EntityChangeInfo(id, EntityType.Tag, EntityState.Deleted));
            }
        }

        public void Unsubscribe()
        {
            lock (_clientsSyncObject)
            {
                _clients.Remove(OperationContext.Current.SessionId);
            }
        }

        private void NotifyOtherClientsAboutDatabaseUpdate(EntityChangeInfo entityChangeInfo)
        {
            lock (_clientsSyncObject)
            {
                List<string> clientsToRemove = new List<string>();

                foreach (var client in _clients)
                {
                    if (((ICommunicationObject) client.Value).State == CommunicationState.Opened)
                    {
                        try
                        {
                            client.Value.NotifyAboutDatabaseUpdate(entityChangeInfo);
                        }
                        catch (Exception)
                        {
                            clientsToRemove.Add(client.Key);
                        }
                    }
                    else
                    {
                        clientsToRemove.Add(client.Key);
                    }
                }

                foreach (string client in clientsToRemove)
                {
                    _clients.Remove(client);
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
                    _imagesRepository.Dispose();
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
