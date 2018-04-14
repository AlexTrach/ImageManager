using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ImagesWcfServiceClient.Models;

namespace ImageManagerWpfClient
{
    class AddTagCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            AvailableTagsEditingWindowViewModel viewModel = (AvailableTagsEditingWindowViewModel) parameter;

            Tag tagToAdd = new Tag { TagName = viewModel.TagNameToAdd };

            viewModel.AvailableTags.Add(tagToAdd);
            //AvailableTagsLocalStorage.Instance.AvailableTags.Add(tagToAdd);
            //ServiceClientWrapper.Instance.AddTag(tagToAdd);
        }

        protected void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged(this, e);
        }
    }
}
