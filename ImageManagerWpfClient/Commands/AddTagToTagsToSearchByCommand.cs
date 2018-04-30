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
        public event EventHandler CanExecuteChanged;

        public SearchImagesByTagsWindowViewModel ViewModel { get; set; }

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

        protected void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged(this, e);
        }
    }
}
