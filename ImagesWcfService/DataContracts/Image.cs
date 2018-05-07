using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ImagesWcfService
{
    [DataContract]
    public class Image
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string ImageName { get; set; }
        [DataMember]
        public byte[] ImageContent { get; set; }
        [DataMember]
        public Tag[] Tags { get; set; }
    }
}
