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

        public ICommand OpenFullSizeImageCommand { get; set; } = new OpenFullSizeImageCommand();

        public ICommand LoadMoreThumbnailsWithSuchTagsCommand { get; set; } = new LoadMoreThumbnailsWithSuchTagsCommand();

        public ObservableCollection<Tag> TagsToSearchBy { get; set; } = new ObservableCollection<Tag>();
        public ObservableCollection<Tag> AvailableTags { get; set; } = new ObservableCollection<Tag>();

        public ObservableCollection<Image> Thumbnails { get; set; } = new ObservableCollection<Image>();

        private bool _canLoadMore;
        public bool CanLoadMore
        {
            get
            {
                return _canLoadMore;
            }
            set
            {
                _canLoadMore = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CanLoadMore)));
            }
        }

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

            TagsToSearchBy.CollectionChanged += TagsToSearchBy_CollectionChanged;

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
            if (!CheckWhetherTagsToSearchByAreEqualToThePrevious())
            {
                TagsToSearchByChanged = true;
            }
            else
            {
                TagsToSearchByChanged = false;
            }

            if (TagsToSearchBy.Count != 0 && TagsToSearchByChanged)
            {
                CanLoadMore = true;
            }
            else
            {
                CanLoadMore = false;
            }
        }

        public List<Tag> PreviousTagsToSearchBy { get; set; } = new List<Tag>();

        public bool TagsToSearchByChanged { get; set; }

        private bool CheckWhetherTagsToSearchByAreEqualToThePrevious()
        {
            if (TagsToSearchBy.Count != PreviousTagsToSearchBy.Count)
            {
                return false;
            }

            for (int i = 0; i < TagsToSearchBy.Count; i++)
            {
                bool isTagFromFirstCollectionPresentInSecondCollection = false;

                for (int j = 0; j < PreviousTagsToSearchBy.Count; j++)
                {
                    if (TagsToSearchBy[i].Id == PreviousTagsToSearchBy[j].Id)
                    {
                        isTagFromFirstCollectionPresentInSecondCollection = true;
                        break;
                    }
                }

                if (!isTagFromFirstCollectionPresentInSecondCollection)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
