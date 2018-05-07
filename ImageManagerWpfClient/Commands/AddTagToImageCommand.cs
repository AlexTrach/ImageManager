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
        public ImageOperationsWindowViewModel ViewModel { get; set; }

        public event EventHandler CanExecuteChanged;

        public AddTagToImageCommand(ImageOperationsWindowViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Tag tagToAdd = (Tag) parameter;

            ViewModel.ImageTags.Add(tagToAdd);
            ViewModel.Image.Tags.Add(tagToAdd);

            ViewModel.AvailableTags.Remove(tagToAdd);
        }

        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged(this, e);
        }
    }
}
