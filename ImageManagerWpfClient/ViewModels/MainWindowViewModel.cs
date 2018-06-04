using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.ObjectModel;
using ImagesWcfServiceClient.Models;
using ImagesWcfServiceClient.DatabaseUpdateNotificationInfrastructure;

namespace ImageManagerWpfClient
{
    class MainWindowViewModel : INotifyPropertyChanged, IDisposable
    {
        public ICommand LoadImageFromFileCommand { get; set; } = new LoadImageFromFileCommand();
        public ICommand ShutdownApplicationCommand { get; set; } = new ShutdownApplicationCommand();
        public ICommand EditAvailableTagsCommand { get; set; } = new EditAvailableTagsCommand();
        public ICommand SearchImagesByTagsCommand { get; set; } = new SearchImagesByTagsCommand();

        public ICommand OpenFullSizeImageCommand { get; set; } = new OpenFullSizeImageCommand();

        public ICommand LoadMoreThumbnailsCommand { get; set; } = new LoadMoreThumbnailsCommand();

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

        public MainWindowViewModel()
        {
            ServiceClientWrapper.Instance.ImageChanged += ServiceClientWrapper_ImageChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
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
                            Thumbnails.Add(thumbnail);
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
                                Thumbnails.RemoveAt(i);
                                Thumbnails.Insert(i, thumbnail);
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
                }
            }
            _disposed = true;
        }

        ~MainWindowViewModel()
        {
            CleanUp(false);
        }
    }
}
