using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImagesWcfServiceClient.DatabaseUpdateNotificaionInfrastructure;

namespace ImagesWcfServiceClient
{
    public interface IDatabaseUpdateListener
    {
        void ImagesServiceCallback_DatabaseUpdated(object sender, DatabaseUpdatedEventArgs e);
    }
}
