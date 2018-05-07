using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ImagesWcfServiceClient.Models;

namespace ImageManagerWpfClient
{
    class OpenFullSizeImageCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Image thumbnail = (Image) parameter;

            Image fullSizeImage = ServiceClientWrapper.Instance.GetFullSizeImage(thumbnail.Id);

            if (fullSizeImage != null)
            {
                ImageOperationsWindow imageOperationsWindow = new ImageOperationsWindow(new ImageOperationsWindowViewModel(fullSizeImage));
                Application.Current.MainWindow = imageOperationsWindow;
                imageOperationsWindow.ShowDialog();
            }
        }

        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged(this, e);
        }
    }
}
