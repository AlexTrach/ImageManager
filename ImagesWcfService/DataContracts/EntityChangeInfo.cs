using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ImagesWcfService
{
    [DataContract]
    public class EntityChangeInfo
    {
        [DataMember]
        public int EntityId { get; set; }
        [DataMember]
        public EntityType EntityType { get; set; }
        [DataMember]
        public EntityState EntityState { get; set; }

        public EntityChangeInfo(int entityId, EntityType entityType, EntityState entityState)
        {
            EntityId = entityId;
            EntityType = entityType;
            EntityState = entityState;
        }
    }
}
