using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ImagesWcfServiceClient.Models;

namespace ImageManagerWpfClient
{
    class UpdateTagCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            AvailableTagsEditingWindowViewModel viewModel = (AvailableTagsEditingWindowViewModel) parameter;

            Tag tagToUpdate = viewModel.TagToUpdate;

            tagToUpdate.TagName = viewModel.TagNameToUpdate;
            viewModel.TagNameToUpdate = null;
            viewModel.CanEnterTagNameToUpdate = false;

            ServiceClientWrapper.Instance.UpdateTag(tagToUpdate);
        }

        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged(this, e);
        }
    }
}
