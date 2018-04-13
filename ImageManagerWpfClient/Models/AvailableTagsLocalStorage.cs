using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImagesWcfServiceClient;

namespace ImageManagerWpfClient
{
    class AvailableTagsLocalStorage
    {
        public static AvailableTagsLocalStorage Instance { get; private set; }

        static AvailableTagsLocalStorage()
        {
            Instance = new AvailableTagsLocalStorage();
        }

        public List<Tag> AvailableTags { get; private set; }

        private AvailableTagsLocalStorage()
        {
            AvailableTags = ServiceClientWrapper.Instance.GetAllTags();
        }
    }
}
