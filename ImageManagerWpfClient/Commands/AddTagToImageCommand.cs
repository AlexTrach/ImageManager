using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ImagesWcfServiceClient.Models;
using System.Collections.ObjectModel;

namespace ImageManagerWpfClient
{
    class AddTagToImageCommand : ICommand
    {
        public ObservableCollection<Tag> Tags { get; set; }
        public ObservableCollection<Tag> AvailableTags { get; set; }

        public event EventHandler CanExecuteChanged;

        public AddTagToImageCommand(ObservableCollection<Tag> tags, ObservableCollection<Tag> availableTags)
        {
            Tags = tags;
            AvailableTags = availableTags;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Tag tagToAdd = (Tag) parameter;

            Tags.Add(tagToAdd);

            AvailableTags.Remove(tagToAdd);
        }

        protected void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged(this, e);
        }
    }
}
