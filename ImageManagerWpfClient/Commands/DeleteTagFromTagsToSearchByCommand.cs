using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ImagesWcfServiceClient.Models;

namespace ImageManagerWpfClient
{
    class DeleteTagFromTagsToSearchByCommand : ICommand
    {
        public SearchImagesByTagsWindowViewModel ViewModel { get; set; }

        public event EventHandler CanExecuteChanged;

        public DeleteTagFromTagsToSearchByCommand(SearchImagesByTagsWindowViewModel viewModel)
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

            ViewModel.TagsToSearchBy.Remove(tagToDelete);
            ViewModel.AvailableTags.Add(tagToDelete);
        }

        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged(this, e);
        }
    }
}
