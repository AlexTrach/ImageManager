using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ImagesWcfServiceClient.Models;

namespace ImageManagerWpfClient
{
    class AvailableTagsEditingWindowViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public ICommand AddTagCommand { get; set; } = new AddTagCommand();
        public ICommand UpdateTagCommand { get; set; } = new UpdateTagCommand();
        public ICommand DeleteTagCommand { get; set; }

        private string _tagNameToAdd;
        public string TagNameToAdd
        {
            get
            {
                return _tagNameToAdd;
            }
            set
            {
                _tagNameToAdd = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(TagNameToAdd)));
            }
        }

        private string _tagNameToUpdate;
        public string TagNameToUpdate
        {
            get
            {
                return _tagNameToUpdate;
            }
            set
            {
                _tagNameToUpdate = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(TagNameToUpdate)));
            }
        }   

        public ObservableCollection<Tag> AvailableTags { get; set; } = new ObservableCollection<Tag>();

        public event PropertyChangedEventHandler PropertyChanged;

        public AvailableTagsEditingWindowViewModel()
        {
            DeleteTagCommand = new DeleteTagCommand(AvailableTags);
            TagNameToAdd = string.Empty;
            TagNameToUpdate = string.Empty;

            foreach (Tag tag in AvailableTagsLocalStorage.Instance.AvailableTags)
            {
                AvailableTags.Add(tag);
            }
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
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
                switch (columnName)
                {
                    case nameof(TagNameToAdd):
                        return ValidateTagName(TagNameToAdd);
                    case nameof(TagNameToUpdate):
                        return ValidateTagName(TagNameToUpdate);
                    default:
                        return string.Empty;
                }
            }
        }

        private string ValidateTagName(string tagName)
        {
            string error = string.Empty;

            if (ValidateTagNameLength(tagName))
            {
                if (ValidateTagNameUniqueness(tagName))
                {
                    return string.Empty;
                }
                else
                {
                    error = "Name must be unique!";
                }
            }
            else
            {
                error = "Name must not exceed 100 characters and must not be empty!";
            }

            return error;
        }

        private bool ValidateTagNameLength(string tagName)
        {
            if (tagName.Length > 0 && tagName.Length <= 100)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool ValidateTagNameUniqueness(string tagName)
        {
            return (from tag in AvailableTags
                    where tag.TagName == tagName
                    select tag).Count() == 0;
        }
    }
}
