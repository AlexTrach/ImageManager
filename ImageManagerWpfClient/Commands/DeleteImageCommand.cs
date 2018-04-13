using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace ImageManagerWpfClient
{
    class DeleteImageCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ImageOperationsWindowViewModel viewModel = (ImageOperationsWindowViewModel) parameter;

            Application.Current.MainWindow.Close();
            Application.Current.MainWindow = Application.Current.Windows[0];

            Task.Factory.StartNew(() => ServiceClientWrapper.Instance.DeleteImage(viewModel.Image));
        }

        protected void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged(this, e);
        }
    }
}
