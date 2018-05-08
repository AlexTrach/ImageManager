using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ImagesWcfService.Utilities;
using System.Data.Entity.Infrastructure;

namespace ImagesWcfService
{
    public class ImagesService : IImagesService
    {
        private static Dictionary<string, IImagesServiceCallback> _clients = new Dictionary<string, IImagesServiceCallback>();
        private static object _clientsSyncObject = new object();

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
            using (ImagesDal.ImagesContext imagesContext = new ImagesDal.ImagesContext())
            {
                if (resetToBeginning)
                {
                    _numberOfSentThumbnails = 0;
                }

                List<ImagesDal.Image> imagesFromDatabase = imagesContext.Images.OrderBy(image => image.Id).Skip(_numberOfSentThumbnails).Take(numberOfThumbnails).ToList();
                _numberOfSentThumbnails += imagesFromDatabase.Count;

                return DatabaseToServiceConversionUtility.CreateThumnailsToSendToClient(imagesFromDatabase, widthOfThumbnail);
            } 
        }

        public Image[] GetNextThumbnailsWithSuchTags(int numberOfThumbnails, int widthOfThumbnail, Tag[] tags, bool resetToBeginning)
        {
            using (ImagesDal.ImagesContext imagesContext = new ImagesDal.ImagesContext())
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

                int[] ids = new int[tags.Length];
                for (int i = 0; i < tags.Length; i++)
                {
                    ids[i] = tags[i].Id;
                }

                IQueryable<ImagesDal.Tag> tagsToSearchBy = imagesContext.Tags.Where(tagToSearchBy => ids.Contains(tagToSearchBy.Id));

                IQueryable<ImagesDal.Image> queryResult = imagesContext.Images.Where(image => image.Tags.Intersect(tagsToSearchBy).Count() == tagsToSearchBy.Count());
                
                List<ImagesDal.Image> imagesFromDatabase = queryResult.OrderBy(image => image.Id).Skip(_numberOfSentThumbnailsWithSpecifiedTags).Take(numberOfThumbnails).ToList();
                _numberOfSentThumbnailsWithSpecifiedTags += imagesFromDatabase.Count;

                return DatabaseToServiceConversionUtility.CreateThumnailsToSendToClient(imagesFromDatabase, widthOfThumbnail);
            }
        }

        public Image GetThumbnail(int widthOfThumbnail, int id)
        {
            using (ImagesDal.ImagesContext imagesContext = new ImagesDal.ImagesContext())
            {
                ImagesDal.Image imageFromDatabase = imagesContext.Images.Find(id);
                if (imageFromDatabase == null)
                {
                    return null;
                }
                else
                {
                    return DatabaseToServiceConversionUtility.CreateThumbnailToSendToClient(imageFromDatabase, widthOfThumbnail);
                }
            }
        }

        public Image GetFullSizeImage(int id)
        {
            using (ImagesDal.ImagesContext imagesContext = new ImagesDal.ImagesContext())
            {
                ImagesDal.Image imageFromDatabase = imagesContext.Images.Find(id);
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
                        Tags = DatabaseToServiceConversionUtility.CreateTagsToSendToClient(new List<ImagesDal.Tag>(imageFromDatabase.Tags))
                    };
                }
            }
        }

        
        public Tag[] GetAllTags()
        {
            using (ImagesDal.ImagesContext imagesContext = new ImagesDal.ImagesContext())
            {
                return DatabaseToServiceConversionUtility.CreateTagsToSendToClient(imagesContext.Tags.ToList());
            }
        }

        public Tag GetTag(int id)
        {
            using (ImagesDal.ImagesContext imagesContext = new ImagesDal.ImagesContext())
            {
                ImagesDal.Tag tagFromDatabase = imagesContext.Tags.Find(id);
                if (tagFromDatabase == null)
                {
                    return null;
                }
                else
                {
                    return DatabaseToServiceConversionUtility.CreateTagToSendToClient(tagFromDatabase);
                }
            }
        }

        public void AddImage(Image image)
        {
            using (ImagesDal.ImagesContext imagesContext = new ImagesDal.ImagesContext())
            {
                ImagesDal.Image imageToAdd = new ImagesDal.Image()
                {
                    ImageName = image.ImageName,
                    ImageContent = image.ImageContent,
                };
                foreach (Tag tag in image.Tags)
                {
                    ImagesDal.Tag tagToAddToImage = imagesContext.Tags.Find(tag.Id);
                    if (tagToAddToImage != null)
                    {
                        imageToAdd.Tags.Add(tagToAddToImage);
                    }
                }

                imagesContext.Images.Add(imageToAdd);
                if (SaveChanges(imagesContext) != 0)
                {
                    NotifyOtherClientsAboutDatabaseUpdate(new EntityChangeInfo(imageToAdd.Id, EntityType.Image, EntityState.Added));
                }
            }
        }

        public void UpdateImage(Image image)
        {
            using (ImagesDal.ImagesContext imagesContext = new ImagesDal.ImagesContext())
            {
                ImagesDal.Image imageToUpdate = imagesContext.Images.Find(image.Id);

                if (imageToUpdate != null)
                {
                    imageToUpdate.ImageName = image.ImageName;
                    imageToUpdate.ImageContent = image.ImageContent;
                    imageToUpdate.Tags.Clear();
                    foreach (Tag tag in image.Tags)
                    {
                        ImagesDal.Tag tagToAddToImage = imagesContext.Tags.Find(tag.Id);
                        if (tagToAddToImage != null)
                        {
                            imageToUpdate.Tags.Add(tagToAddToImage);
                        }
                    }

                    if (SaveChanges(imagesContext) != 0)
                    {
                        NotifyOtherClientsAboutDatabaseUpdate(new EntityChangeInfo(imageToUpdate.Id, EntityType.Image, EntityState.Modified));
                    }
                }
            }
        }

        public void DeleteImage(int id)
        {
            using (ImagesDal.ImagesContext imagesContext = new ImagesDal.ImagesContext())
            {
                imagesContext.Entry(new ImagesDal.Image() { Id = id }).State = System.Data.Entity.EntityState.Deleted;
                if (SaveChanges(imagesContext) != 0)
                {
                    NotifyOtherClientsAboutDatabaseUpdate(new EntityChangeInfo(id, EntityType.Image, EntityState.Deleted));
                }
            }
        }

        public void AddTag(Tag tag)
        {
            using (ImagesDal.ImagesContext imagesContext = new ImagesDal.ImagesContext())
            {
                ImagesDal.Tag tagToAdd = new ImagesDal.Tag()
                {
                    TagName = tag.TagName
                };

                imagesContext.Tags.Add(tagToAdd);
                if (SaveChanges(imagesContext) != 0)
                {
                    NotifyOtherClientsAboutDatabaseUpdate(new EntityChangeInfo(tagToAdd.Id, EntityType.Tag, EntityState.Added));
                }
            }
        }

        public void UpdateTag(Tag tag)
        {
            using (ImagesDal.ImagesContext imagesContext = new ImagesDal.ImagesContext())
            {
                ImagesDal.Tag tagToUpdate = imagesContext.Tags.Find(tag.Id);

                if (tagToUpdate != null)
                {
                    tagToUpdate.TagName = tag.TagName;

                    imagesContext.Entry(tagToUpdate).State = System.Data.Entity.EntityState.Modified;
                    if (SaveChanges(imagesContext) != 0)
                    {
                        NotifyOtherClientsAboutDatabaseUpdate(new EntityChangeInfo(tagToUpdate.Id, EntityType.Tag, EntityState.Modified));
                    }
                }
            }
        }

        public void DeleteTag(int id)
        {
            using (ImagesDal.ImagesContext imagesContext = new ImagesDal.ImagesContext())
            {
                imagesContext.Entry(new ImagesDal.Tag() { Id = id }).State = System.Data.Entity.EntityState.Deleted;
                if (SaveChanges(imagesContext) != 0)
                {
                    NotifyOtherClientsAboutDatabaseUpdate(new EntityChangeInfo(id, EntityType.Tag, EntityState.Deleted));
                }
            }
        }
        
        private int SaveChanges(ImagesDal.ImagesContext imagesContext)
        {
            try
            {
                return imagesContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return 0;
            }
            catch (Exception)
            {
                throw;
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
    }
}
