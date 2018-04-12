using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ImagesWcfServiceClient;
using System.Collections.ObjectModel;

namespace ImageManagerWpfClient
{
    class DeleteTagFromImageCommand : ICommand
    {
        public ObservableCollection<Tag> Tags { get; set; }
        public ObservableCollection<Tag> AvailableTags { get; set; }

        public event EventHandler CanExecuteChanged;

        public DeleteTagFromImageCommand(ObservableCollection<Tag> tags, ObservableCollection<Tag> availableTags)
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
            Tag tagToDelete = (Tag) parameter;

            Tags.Remove(tagToDelete);

            AvailableTags.Add(tagToDelete);
        }

        protected void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged(this, e);
        }
    }
}
