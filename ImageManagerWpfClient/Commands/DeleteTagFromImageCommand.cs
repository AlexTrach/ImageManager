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
    class DeleteTagFromImageCommand : ICommand
    {
        public ImageOperationsWindowViewModel ViewModel { get; set; }

        public event EventHandler CanExecuteChanged;

        public DeleteTagFromImageCommand(ImageOperationsWindowViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Tag tagToDelete = (Tag) parameter;

            ViewModel.Tags.Remove(tagToDelete);
            ViewModel.Image.Tags.Remove(tagToDelete);

            ViewModel.AvailableTags.Add(tagToDelete);
        }

        protected void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged(this, e);
        }
    }
}
