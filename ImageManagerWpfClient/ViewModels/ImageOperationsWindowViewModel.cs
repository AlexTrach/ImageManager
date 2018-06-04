using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using ImagesWcfServiceClient.Models;
using ImagesWcfServiceClient.DatabaseUpdateNotificationInfrastructure;

namespace ImageManagerWpfClient
{
    public class ImageOperationsWindowViewModel : INotifyPropertyChanged, IDataErrorInfo, IDisposable
    {
        public ICommand ChangeImageContentCommand { get; set; } = new ChangeImageContentCommand();

        public ICommand AddTagToImageCommand { get; set; }
        public ICommand DeleteTagFromImageCommand { get; set; }

        public ICommand AddImageCommand { get; set; } = new AddImageCommand();
        public ICommand UpdateImageCommand { get; set; } = new UpdateImageCommand();
        public ICommand DeleteImageCommand { get; set; } = new DeleteImageCommand();

        public Image Image { get; private set; }

        public string ImageName
        {
            get
            {
                return Image.ImageName;
            }
            set
            {
                Image.ImageName = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(ImageName)));
            }
        }

        public BitmapImage ImageContent
        {
            get
            {
                return Image.ImageContent;
            }
            set
            {
                Image.ImageContent = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(ImageContent)));
            }
        }

        public ObservableCollection<Tag> ImageTags { get; set; } = new ObservableCollection<Tag>();
        public ObservableCollection<Tag> AvailableTags { get; set; } = new ObservableCollection<Tag>();

        private bool _canAdd;
        public bool CanAdd
        {
            get
            {
                return _canAdd;
            }
            private set
            {
                _canAdd = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CanAdd)));
            }
        }

        private bool _canUpdate;
        public bool CanUpdate
        {
            get
            {
                return _canUpdate;
            }
            private set
            {
                _canUpdate = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CanUpdate)));
            }
        }

        private bool _canDelete;
        public bool CanDelete
        {
            get
            {
                return _canDelete;
            }
            private set
            {
                _canDelete = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CanDelete)));
            }
        }

        public ImageOperationsWindowViewModel(Image image)
        {
            AddTagToImageCommand = new AddTagToImageCommand(this);
            DeleteTagFromImageCommand = new DeleteTagFromImageCommand(this);

            Image = image;

            foreach (Tag tag in Image.Tags)
            {
                ImageTags.Add(tag);
            }

            foreach (Tag tag in ServiceClientWrapper.Instance.GetAllTags())
            {
                if ((from imageTag in ImageTags
                     where imageTag.Id == tag.Id
                     select imageTag).Count() == 0)
                {
                    AvailableTags.Add(tag);
                }
            }
            
            ServiceClientWrapper.Instance.ImageChanged += ServiceClientWrapper_ImageChanged;
            ServiceClientWrapper.Instance.TagChanged += ServiceClientWrapper_TagChanged;

            EnableRespectiveOperations();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        public string Error
        {
            get
            {
                return String.Empty;
            }
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(ImageName):
                        if (!(ImageName.Length > 0 && ImageName.Length <= 100))
                        {
                            DisableRespectiveOperations();
                            return "Name must not exceed 100 characters and must not be empty!";
                        }
                        else
                        {
                            EnableRespectiveOperations();
                            return string.Empty;
                        }
                    default:
                        return string.Empty;
                }
            }
        }

        private void EnableRespectiveOperations()
        {
            if (Image.Id == 0)
            {
                CanAdd = true;
            }
            else
            {
                CanUpdate = true;
                CanDelete = true;
            }
        }

        private void DisableRespectiveOperations()
        {
            if (Image.Id == 0)
            {
                CanAdd = false;
            }
            else
            {
                CanUpdate = false;
            }
        }

        private async void ServiceClientWrapper_ImageChanged(object sender, ImageChangedEventArgs e)
        {
            if (Image.Id == e.Id)
            {
                switch (e.EntityState)
                {
                    case EntityState.Modified:
                        Image image = await Task.Factory.StartNew(() => ServiceClientWrapper.Instance.GetFullSizeImage(e.Id));
                        if (image != null)
                        {
                            Image = image;
                            ImageName = image.ImageName;
                            ImageContent = image.ImageContent;

                            ImageTags.Clear();
                            foreach (Tag tag in Image.Tags)
                            {
                                ImageTags.Add(tag);
                            }

                            AvailableTags.Clear();
                            foreach (Tag tag in ServiceClientWrapper.Instance.GetAllTags())
                            {
                                if ((from imageTag in ImageTags
                                     where imageTag.Id == tag.Id
                                     select imageTag).Count() == 0)
                                {
                                    AvailableTags.Add(tag);
                                }
                            }
                        }
                        else
                        {
                            Application.Current.MainWindow.Close();
                        }
                        break;
                    case EntityState.Deleted:
                        Application.Current.MainWindow.Close();
                        break;
                }
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
                    for (int i = 0; i < ImageTags.Count; i++)
                    {
                        if (ImageTags[i].Id == e.Id)
                        {
                            Tag tag = await Task.Factory.StartNew(() => ServiceClientWrapper.Instance.GetTag(e.Id));
                            if (tag != null)
                            {
                                ImageTags.RemoveAt(i);
                                ImageTags.Insert(i, tag);
                                Image.Tags.RemoveAt(i);
                                Image.Tags.Insert(i, tag);
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
                    foreach (Tag tag in ImageTags)
                    {
                        if (tag.Id == e.Id)
                        {
                            ImageTags.Remove(tag);
                            Image.Tags.Remove(tag);
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

        ~ImageOperationsWindowViewModel()
        {
            CleanUp(false);
        }
    }
}
