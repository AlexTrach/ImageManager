using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ImagesWcfService
{
    [DataContract]
    public class Tag
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string TagName { get; set; }
    }
}
