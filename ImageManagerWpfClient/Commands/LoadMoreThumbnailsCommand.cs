using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ImagesWcfServiceClient.Models;

namespace ImageManagerWpfClient
{
    class LoadMoreThumbnailsCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private bool _canExecute = true;
        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public async void Execute(object parameter)
        {
            MainWindowViewModel viewModel = (MainWindowViewModel) parameter;

            _canExecute = false;
            OnCanExecuteChanged(EventArgs.Empty);

            viewModel.Status = "Loading...";
            List<Image> thumbnails = await Task.Factory.StartNew(() => ServiceClientWrapper.Instance.GetNextThumbnails(false));
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
                viewModel.AllAvailableThumbnailsWereLoaded = true;
                viewModel.Status = "All available thumbnails were loaded.";
            }
        }

        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged(this, e);
        }
    }
}
