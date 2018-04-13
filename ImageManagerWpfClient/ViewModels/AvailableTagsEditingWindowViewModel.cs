using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ImagesWcfServiceClient;

namespace ImageManagerWpfClient
{
    class AvailableTagsEditingWindowViewModel : IDataErrorInfo
    {
        public ICommand AddTagCommand { get; set; } = new AddTagCommand();
        public ICommand UpdateTagCommand { get; set; } = new UpdateTagCommand();
        public ICommand DeleteTagCommand { get; set; } = new DeleteTagCommand();

        public ObservableCollection<Tag> AvailableTags { get; set; } = new ObservableCollection<Tag>();

        public AvailableTagsEditingWindowViewModel()
        {
            foreach (Tag tag in AvailableTagsLocalStorage.Instance.AvailableTags)
            {
                AvailableTags.Add(tag);
            }
            AvailableTags.Add(new Tag { TagName = "Test" });
            AvailableTags.Add(new Tag { TagName = "1" });
            AvailableTags.Add(new Tag { TagName = "Test11111" });
            AvailableTags.Add(new Tag { TagName = "11111111111111111111111111111111111111111111111111" });
            AvailableTags.Add(new Tag { TagName = "1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111" });
        }

        public string Error
        {
            get
            {
                return string.Empty;
            }
        }

        public string this[string columnName]
        {
            get
            {
                return string.Empty;
            }
        }
    }
}
