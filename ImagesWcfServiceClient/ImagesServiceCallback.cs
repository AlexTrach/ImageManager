using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImagesWcfServiceClient.DatabaseUpdateNotificationInfrastructure;

namespace ImagesWcfServiceClient.ImagesWcfServiceReference
{
    class ImagesServiceCallback : IImagesServiceCallback
    {
        public event EventHandler<DatabaseUpdatedEventArgs> DatabaseUpdated;

        public ImagesServiceCallback(IDatabaseUpdateListener listener)
        {
            DatabaseUpdated += listener.ImagesServiceCallback_DatabaseUpdated;
        }

        public void NotifyAboutDatabaseUpdate(EntityChangeInfo entityChangeInfo)
        {
            OnDatabaseUpdated(new DatabaseUpdatedEventArgs(entityChangeInfo.EntityId, entityChangeInfo.EntityType, entityChangeInfo.EntityState));
        }

        protected virtual void OnDatabaseUpdated(DatabaseUpdatedEventArgs e)
        {
            DatabaseUpdated?.Invoke(this, e);
        }
    }
}
