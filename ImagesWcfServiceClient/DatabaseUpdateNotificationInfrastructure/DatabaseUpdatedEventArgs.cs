using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagesWcfServiceClient.DatabaseUpdateNotificationInfrastructure
{
    public class DatabaseUpdatedEventArgs : EventArgs
    {
        public int EntityId { get; set; }
        public EntityType EntityType { get; set; }
        public EntityState EntityState { get; set; }

        internal DatabaseUpdatedEventArgs(int entityId, ImagesWcfServiceReference.EntityType entityType, ImagesWcfServiceReference.EntityState entityState)
        {
            EntityId = entityId;

            switch (entityType)
            {
                case ImagesWcfServiceReference.EntityType.Image:
                    EntityType = EntityType.Image;
                    break;
                case ImagesWcfServiceReference.EntityType.Tag:
                    EntityType = EntityType.Tag;
                    break;
            }

            switch (entityState)
            {
                case ImagesWcfServiceReference.EntityState.Added:
                    EntityState = EntityState.Added;
                    break;
                case ImagesWcfServiceReference.EntityState.Modified:
                    EntityState = EntityState.Modified;
                    break;
                case ImagesWcfServiceReference.EntityState.Deleted:
                    EntityState = EntityState.Deleted;
                    break;
            }
        }
    }
}
