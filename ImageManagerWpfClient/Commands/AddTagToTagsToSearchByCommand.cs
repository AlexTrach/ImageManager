using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ImagesWcfServiceClient.Models;

namespace ImageManagerWpfClient
{
    class AddTagToTagsToSearchByCommand : ICommand
    {
        public SearchImagesByTagsWindowViewModel ViewModel { get; set; }

        public event EventHandler CanExecuteChanged;

        public AddTagToTagsToSearchByCommand(SearchImagesByTagsWindowViewModel viewModel)
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

            ViewModel.TagsToSearchBy.Add(tagToAdd);
            ViewModel.AvailableTags.Remove(tagToAdd);
        }

        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged(this, e);
        }
    }
}
