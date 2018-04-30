using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ImagesWcfServiceClient.Models;
using System.ComponentModel;

namespace ImageManagerWpfClient
{
    class SearchImagesByTagsWindowViewModel : INotifyPropertyChanged
    {
        public ICommand AddTagToTagsToSearchByCommand { get; set; }
        public ICommand DeleteTagFromTagsToSearchByCommand { get; set; }
        public ICommand LoadMoreThumbnailsWithSuchTagsCommand { get; set; } = new LoadMoreThumbnailsWithSuchTagsCommand();

        public ObservableCollection<Tag> TagsToSearchBy { get; set; } = new ObservableCollection<Tag>();
        public ObservableCollection<Tag> AvailableTags { get; set; } = new ObservableCollection<Tag>();
        public ObservableCollection<Image> Thumbnails { get; set; } = new ObservableCollection<Image>();

        private string _status = "Ready.";
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Status)));
            }
        }

        public SearchImagesByTagsWindowViewModel()
        {
            AddTagToTagsToSearchByCommand = new AddTagToTagsToSearchByCommand(this);
            DeleteTagFromTagsToSearchByCommand = new DeleteTagFromTagsToSearchByCommand(this);

            foreach (Tag tag in ServiceClientWrapper.Instance.GetAllTags())
            {
                AvailableTags.Add(tag);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        private void TagsToSearchBy_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Thumbnails.Clear();
        }
    }
}
