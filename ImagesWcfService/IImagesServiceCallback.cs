using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace ImagesWcfService
{
    public interface IImagesServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void NotifyAboutDatabaseUpdate(EntityChangeInfo entityChangeInfo);
    }
}
