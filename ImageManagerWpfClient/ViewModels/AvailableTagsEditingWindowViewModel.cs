﻿using System;
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
        public ICommand SelectTagToUpdateCommand { get; set; }
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

        private bool _canAddTag;
        public bool CanAddTag
        {
            get
            {
                return _canAddTag;
            }
            set
            {
                _canAddTag = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CanAddTag)));
            }
        }

        private Tag _tagToUpdate;
        public Tag TagToUpdate
        {
            get
            {
                return _tagToUpdate;
            }
            set
            {
                _tagToUpdate = value;
                TagNameToUpdate = _tagToUpdate.TagName;
            }
        }

        private bool _canUpdateTag;
        public bool CanUpdateTag
        {
            get
            {
                return _canUpdateTag;
            }
            set
            {
                _canUpdateTag = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CanUpdateTag)));
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

        private bool _canEnterTagNameToUpdate;
        public bool CanEnterTagNameToUpdate
        {
            get
            {
                return _canEnterTagNameToUpdate;
            }
            set
            {
                _canEnterTagNameToUpdate = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CanEnterTagNameToUpdate)));
            }
        }

        public ObservableCollection<Tag> AvailableTags { get; set; } = new ObservableCollection<Tag>();

        public event PropertyChangedEventHandler PropertyChanged;

        public AvailableTagsEditingWindowViewModel()
        {
            SelectTagToUpdateCommand = new SelectTagToUpdateCommand(this);
            DeleteTagCommand = new DeleteTagCommand(this);

            foreach (Tag tag in ServiceClientWrapper.Instance.GetAllTags())
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
                string error;
                switch (columnName)
                {
                    case nameof(TagNameToAdd):
                        error = ValidateTagName(TagNameToAdd);
                        if (error == string.Empty)
                        {
                            CanAddTag = true;
                            return error;
                        }
                        else
                        {
                            CanAddTag = false;
                            return error;
                        }
                    case nameof(TagNameToUpdate):
                        error = ValidateTagName(TagNameToUpdate);
                        if (error == string.Empty)
                        {
                            CanUpdateTag = true;
                            return error;
                        }
                        else
                        {
                            CanUpdateTag = false;
                            return error;
                        }
                    default:
                        return string.Empty;
                }
            }
        }

        private string ValidateTagName(string tagName)
        {
            string error = string.Empty;

            if(tagName != null)
            {
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
