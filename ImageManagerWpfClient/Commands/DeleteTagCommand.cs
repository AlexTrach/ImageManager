using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using ImagesWcfServiceClient.Models;
using System.Windows.Input;

namespace ImageManagerWpfClient
{
    class DeleteTagCommand : ICommand
    {
        public ObservableCollection<Tag> AvailableTags { get; set; }

        public event EventHandler CanExecuteChanged;

        public DeleteTagCommand(ObservableCollection<Tag> availableTags)
        {
            AvailableTags = availableTags;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Tag tagToDelete = (Tag) parameter;

            AvailableTags.Remove(tagToDelete);

            //AvailableTagsLocalStorage.Instance.AvailableTags.Remove(tagToDelete);
            //ServiceClientWrapper.Instance.DeleteTag(tagToDelete);
        }

        protected void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged(this, e);
        }
    }
}
