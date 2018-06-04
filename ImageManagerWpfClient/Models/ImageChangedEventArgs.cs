using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImagesWcfServiceClient.DatabaseUpdateNotificationInfrastructure;

namespace ImageManagerWpfClient
{
    class ImageChangedEventArgs
    {
        public int Id { get; set; }
        public EntityState EntityState { get; set; }

        public ImageChangedEventArgs(int id, EntityState entityState)
        {
            Id = id;
            EntityState = entityState;
        }
    }
}
