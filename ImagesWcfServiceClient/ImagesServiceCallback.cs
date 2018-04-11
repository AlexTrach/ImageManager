using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagesWcfServiceClient.ImagesWcfServiceReference
{
    class ImagesServiceCallback : IImagesServiceCallback
    {
        public event EventHandler DatabaseUpdated;

        public ImagesServiceCallback(IDatabaseUpdateListener listener)
        {
            DatabaseUpdated += listener.ImagesServiceCallback_DatabaseUpdated;
        }

        public void NotifyAboutDatabaseUpdate()
        {
            OnDatabaseUpdated(EventArgs.Empty);
        }

        protected virtual void OnDatabaseUpdated(EventArgs e)
        {
            DatabaseUpdated?.Invoke(this, e);
        }
    }
}
