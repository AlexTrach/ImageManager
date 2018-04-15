using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ImagesWcfServiceClient.Models;

namespace ImageManagerWpfClient
{
    public class ImageOperationsWindowViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public ICommand ChangeImageContentCommand { get; set; } = new ChangeImageContentCommand();

        public ICommand AddTagToImageCommand { get; set; }
        public ICommand DeleteTagFromImageCommand { get; set; }

        public ICommand AddImageCommand { get; set; } = new AddImageCommand();
        public ICommand UpdateImageCommand { get; set; } = new UpdateImageCommand();
        public ICommand DeleteImageCommand { get; set; } = new DeleteImageCommand();

        public Image Image { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

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

        public ObservableCollection<Tag> Tags { get; set; } = new ObservableCollection<Tag>();
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
            Image = image;

            foreach (Tag tag in Image.Tags)
            {
                Tags.Add(tag);
            }

            foreach (Tag tag in ServiceClientWrapper.Instance.GetAllTags())
            {
                if ((from imageTag in Tags
                     where imageTag.Id == tag.Id
                     select imageTag).Count() == 0)
                {
                    AvailableTags.Add(tag);
                }
            }
            
            AddTagToImageCommand = new AddTagToImageCommand(Tags, AvailableTags);
            DeleteTagFromImageCommand = new DeleteTagFromImageCommand(Tags, AvailableTags);

            EnableRespectiveOperations();
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
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
    }
}
