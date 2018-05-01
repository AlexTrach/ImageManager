using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace ImageManagerWpfClient
{
    class UpdateImageCommand : ICommand
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

            Task.Factory.StartNew(() => ServiceClientWrapper.Instance.UpdateImage(viewModel.Image));
        }

        protected void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged(this, e);
        }
    }
}
