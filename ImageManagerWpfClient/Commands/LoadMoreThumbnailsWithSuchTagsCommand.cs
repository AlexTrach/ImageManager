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
        public event EventHandler CanExecuteChanged;

        private bool _canExecute = true;
        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public async void Execute(object parameter)
        {
            SearchImagesByTagsWindowViewModel viewModel = (SearchImagesByTagsWindowViewModel) parameter;

            _canExecute = false;
            OnCanExecuteChanged(EventArgs.Empty);

            viewModel.Status = "Loading...";
            List<Image> thumbnails = await Task.Factory.StartNew(() => ServiceClientWrapper.Instance.GetNextThumbnailsWithSuchTags(new List<Tag>(viewModel.TagsToSearchBy), false));
            foreach (Image thumbnail in thumbnails)
            {
                viewModel.Thumbnails.Add(thumbnail);
            }

            if (thumbnails.Count != 0)
            {
                _canExecute = true;
                OnCanExecuteChanged(EventArgs.Empty);
                viewModel.Status = "Ready.";
            }
            else
            {
                viewModel.Status = "All available thumbnails were loaded.";
            }
        }

        protected void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged(this, e);
        }
    }
}
