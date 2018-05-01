using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ImagesWcfServiceClient.Models;

namespace ImageManagerWpfClient
{
    class LoadMoreThumbnailsWithSuchTagsCommand : ICommand
    {
        private bool _hasAlreadyBeenExecutedOnce;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            SearchImagesByTagsWindowViewModel viewModel = (SearchImagesByTagsWindowViewModel) parameter;

            viewModel.CanLoadMore = false;

            viewModel.Status = "Loading...";

            if (viewModel.TagsToSearchByChanged)
            {
                viewModel.TagsToSearchByChanged = false;

                viewModel.PreviousTagsToSearchBy.Clear();
                viewModel.PreviousTagsToSearchBy.AddRange(viewModel.TagsToSearchBy);

                viewModel.Thumbnails.Clear();
            }

            List<Image> thumbnails = null;
            if (_hasAlreadyBeenExecutedOnce)
            {
                thumbnails = await Task.Factory.StartNew(() => ServiceClientWrapper.Instance.GetNextThumbnailsWithSuchTags(new List<Tag>(viewModel.TagsToSearchBy), false));
            }
            else
            {
                thumbnails = await Task.Factory.StartNew(() => ServiceClientWrapper.Instance.GetNextThumbnailsWithSuchTags(new List<Tag>(viewModel.TagsToSearchBy), true));
                _hasAlreadyBeenExecutedOnce = true;
            }

            
            foreach (Image thumbnail in thumbnails)
            {
                viewModel.Thumbnails.Add(thumbnail);
            }

            if (thumbnails.Count != 0)
            {
                viewModel.CanLoadMore = true;
                OnCanExecuteChanged(EventArgs.Empty);
                viewModel.Status = "Ready.";
            }
            else
            {
                viewModel.CanLoadMore = false;
                viewModel.Status = "All available thumbnails were loaded.";
            }
        }

        protected void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged(this, e);
        }
    }
}
