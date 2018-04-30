using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImagesWcfServiceClient;
using ImagesWcfServiceClient.Models;

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

        public event EventHandler DatabaseUpdated;

        void IDatabaseUpdateListener.ImagesServiceCallback_DatabaseUpdated(object sender, EventArgs e)
        {
            OnDatabaseUpdated(EventArgs.Empty);
        }

        protected void OnDatabaseUpdated(EventArgs e)
        {
            DatabaseUpdated(this, e);
        }

        public List<Image> GetNextThumbnails(bool resetToBeginning)
        {
            return _serviceClient.GetNextThumbnails(resetToBeginning);
        }

        public List<Image> GetNextThumbnailsWithSuchTags(List<Tag> tags, bool resetToBeginning)
        {
            return _serviceClient.GetNextThumbnailsWithSuchTags(tags, resetToBeginning);
        }

        public Image GetFullSizeImage(int id)
        {
            return _serviceClient.GetFullSizeImage(id);
        }

        public List<Tag> GetAllTags()
        {
            return _serviceClient.GetAllTags();
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
