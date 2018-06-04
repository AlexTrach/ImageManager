using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImagesWcfServiceClient;
using ImagesWcfServiceClient.Models;
using ImagesWcfServiceClient.DatabaseUpdateNotificationInfrastructure;

namespace ImageManagerWpfClient
{
    class ServiceClientWrapper : IDatabaseUpdateListener, IDisposable
    {
        public static ServiceClientWrapper Instance { get; private set; }

        static ServiceClientWrapper()
        {
            Instance = new ServiceClientWrapper();
        }

        private ServiceClient _serviceClient;

        private ServiceClientWrapper()
        {
            _serviceClient = new ServiceClient(this);
        }

        public event EventHandler<ImageChangedEventArgs> ImageChanged;
        public event EventHandler<TagChangedEventArgs> TagChanged;

        void IDatabaseUpdateListener.ImagesServiceCallback_DatabaseUpdated(object sender, DatabaseUpdatedEventArgs e)
        {
            switch (e.EntityType)
            {
                case EntityType.Image:
                    OnImageChanged(new ImageChangedEventArgs(e.EntityId, e.EntityState));
                    break;
                case EntityType.Tag:
                    OnTagChanged(new TagChangedEventArgs(e.EntityId, e.EntityState));
                    break;
            }
        }

        protected virtual void OnImageChanged(ImageChangedEventArgs e)
        {
            ImageChanged?.Invoke(this, e);
        }

        protected virtual void OnTagChanged(TagChangedEventArgs e)
        {
            TagChanged?.Invoke(this, e);
        }

        public List<Image> GetNextThumbnails(bool resetToBeginning)
        {
            return _serviceClient.GetNextThumbnails(resetToBeginning);
        }

        public List<Image> GetNextThumbnailsWithSuchTags(List<Tag> tags, bool resetToBeginning)
        {
            return _serviceClient.GetNextThumbnailsWithSuchTags(tags, resetToBeginning);
        }

        public Image GetThumbnail(int id)
        {
            return _serviceClient.GetThumbnail(id);
        }

        public Image GetFullSizeImage(int id)
        {
            return _serviceClient.GetFullSizeImage(id);
        }

        public List<Tag> GetAllTags()
        {
            return _serviceClient.GetAllTags();
        }

        public Tag GetTag(int id)
        {
            return _serviceClient.GetTag(id);
        }

        public void AddImage(Image image)
        {
            _serviceClient.AddImage(image);
        }

        public void UpdateImage(Image image)
        {
            _serviceClient.UpdateImage(image);
        }

        public void DeleteImage(Image image)
        {
            _serviceClient.DeleteImage(image);
        }

        public void AddTag(Tag tag)
        {
            _serviceClient.AddTag(tag);
        }

        public void UpdateTag(Tag tag)
        {
            _serviceClient.UpdateTag(tag);
        }

        public void DeleteTag(Tag tag)
        {
            _serviceClient.DeleteTag(tag);
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
                    _serviceClient.Dispose();
                }
            }
            _disposed = true;
        }

        ~ServiceClientWrapper()
        {
            CleanUp(false);
        }
    }
}
