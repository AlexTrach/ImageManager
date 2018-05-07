using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImagesWcfServiceClient.DatabaseUpdateNotificaionInfrastructure;

namespace ImageManagerWpfClient
{
    class TagChangedEventArgs
    {
        public int Id { get; set; }
        public EntityState EntityState { get; set; }

        public TagChangedEventArgs(int id, EntityState entityState)
        {
            Id = id;
            EntityState = entityState;
        }
    }
}
