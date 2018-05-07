using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows.Media.Imaging;

namespace ImageManagerWpfClient
{
    class ChangeImageContentCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ImageOperationsWindowViewModel viewModel = (ImageOperationsWindowViewModel) parameter;

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files | *.jpeg;*.jpg;*.png";
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                viewModel.ImageContent = new BitmapImage(new Uri(dialog.FileName));
            }
        }

        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged(this, e);
        }
    }
}
