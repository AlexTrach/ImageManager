using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImagesWcfServiceClient.Models;

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
            AvailableTags = new List<Tag>();//ServiceClientWrapper.Instance.GetAllTags();
            AvailableTags.Add(new Tag { TagName = "Test" });
            AvailableTags.Add(new Tag { TagName = "1" });
            AvailableTags.Add(new Tag { TagName = "Test11111" });
            AvailableTags.Add(new Tag { TagName = "11111111111111111111111111111111111111111111111111" });
            AvailableTags.Add(new Tag { TagName = "1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111" });
        }
    }
}
