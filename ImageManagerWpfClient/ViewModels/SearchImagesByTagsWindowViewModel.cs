using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using ImagesWcfServiceClient.Models;
using ImagesWcfServiceClient.DatabaseUpdateNotificationInfrastructure;

namespace ImageManagerWpfClient
{
    class SearchImagesByTagsWindowViewModel : INotifyPropertyChanged, IDisposable
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
            
            ServiceClientWrapper.Instance.ImageChanged += ServiceClientWrapper_ImageChanged;
            ServiceClientWrapper.Instance.TagChanged += ServiceClientWrapper_TagChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
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

        public bool AllAvailableThumbnailsWereLoaded { get; set; }

        private async void ServiceClientWrapper_ImageChanged(object sender, ImageChangedEventArgs e)
        {
            switch (e.EntityState)
            {
                case EntityState.Added:
                    if (AllAvailableThumbnailsWereLoaded)
                    {
                        Image thumbnail = await Task.Factory.StartNew(() => ServiceClientWrapper.Instance.GetThumbnail(e.Id));
                        if (thumbnail != null)
                        {
                            if ((from previousTagToSearchBy in PreviousTagsToSearchBy
                                join tag in thumbnail.Tags
                                on previousTagToSearchBy.Id equals tag.Id
                                select tag).Count() == PreviousTagsToSearchBy.Count)
                            {
                                Thumbnails.Add(thumbnail);
                            }
                        }
                    }
                    break;
                case EntityState.Modified:
                    for (int i = 0; i < Thumbnails.Count; i++)
                    {
                        if (Thumbnails[i].Id == e.Id)
                        {
                            Image thumbnail = await Task.Factory.StartNew(() => ServiceClientWrapper.Instance.GetThumbnail(e.Id));
                            if (thumbnail != null)
                            {
                                if ((from previousTagToSearchBy in PreviousTagsToSearchBy
                                     join tag in thumbnail.Tags
                                     on previousTagToSearchBy.Id equals tag.Id
                                     select tag).Count() == PreviousTagsToSearchBy.Count)
                                {
                                    Thumbnails.RemoveAt(i);
                                    Thumbnails.Insert(i, thumbnail);
                                }
                            }
                            break;
                        }
                    }
                    break;
                case EntityState.Deleted:
                    foreach (Image thumbnail in Thumbnails)
                    {
                        if (thumbnail.Id == e.Id)
                        {
                            Thumbnails.Remove(thumbnail);
                            break;
                        }
                    }
                    break;
            }
        }

        private async void ServiceClientWrapper_TagChanged(object sender, TagChangedEventArgs e)
        {
            switch (e.EntityState)
            {
                case EntityState.Added:
                    Tag tagToAdd = await Task.Factory.StartNew(() => ServiceClientWrapper.Instance.GetTag(e.Id));
                    if (tagToAdd != null)
                    {
                        AvailableTags.Add(tagToAdd);
                    }
                    break;
                case EntityState.Modified:
                    for (int i = 0; i < TagsToSearchBy.Count; i++)
                    {
                        if (TagsToSearchBy[i].Id == e.Id)
                        {
                            Tag tag = await Task.Factory.StartNew(() => ServiceClientWrapper.Instance.GetTag(e.Id));
                            if (tag != null)
                            {
                                TagsToSearchBy.RemoveAt(i);
                                TagsToSearchBy.Insert(i, tag);
                            }
                            break;
                        }
                    }

                    for (int i = 0; i < AvailableTags.Count; i++)
                    {
                        if (AvailableTags[i].Id == e.Id)
                        {
                            Tag tag = await Task.Factory.StartNew(() => ServiceClientWrapper.Instance.GetTag(e.Id));
                            if (tag != null)
                            {
                                AvailableTags.RemoveAt(i);
                                AvailableTags.Insert(i, tag);
                            }
                            break;
                        }
                    }
                    break;
                case EntityState.Deleted:
                    foreach (Tag tag in TagsToSearchBy)
                    {
                        if (tag.Id == e.Id)
                        {
                            TagsToSearchBy.Remove(tag);
                            break;
                        }
                    }

                    foreach (Tag tag in AvailableTags)
                    {
                        if (tag.Id == e.Id)
                        {
                            AvailableTags.Remove(tag);
                            break;
                        }
                    }
                    break;
            }
        }

        private bool _disposed = false;

        public void Dispose()
        {
            CleanUp(true);
            GC.SuppressFinalize(this);
        }

        private void CleanUp(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    ServiceClientWrapper.Instance.ImageChanged -= ServiceClientWrapper_ImageChanged;
                    ServiceClientWrapper.Instance.TagChanged -= ServiceClientWrapper_TagChanged;
                }
            }
            _disposed = true;
        }

        ~SearchImagesByTagsWindowViewModel()
        {
            CleanUp(false);
        }
    }
}
