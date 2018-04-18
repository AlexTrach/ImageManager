using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Collections.ObjectModel;
using ImagesWcfServiceClient.Models;
using System.ComponentModel;

namespace ImageManagerWpfClient
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public ICommand LoadImageFromFileCommand { get; set; } = new LoadImageFromFileCommand();
        public ICommand ShutdownApplicationCommand { get; set; } = new ShutdownApplicationCommand();
        public ICommand EditAvailableTagsCommand { get; set; } = new EditAvailableTagsCommand();

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}
