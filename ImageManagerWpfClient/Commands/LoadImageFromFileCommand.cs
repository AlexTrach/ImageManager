using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Imaging;
using ImagesWcfServiceClient.Models;
using System.Windows;

namespace ImageManagerWpfClient
{
    class LoadImageFromFileCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files | *.jpeg;*.jpg;*.png";
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                BitmapImage bitmapImage = new BitmapImage(new Uri(dialog.FileName));
                ImageOperationsWindowViewModel viewModel = new ImageOperationsWindowViewModel(new Image()
                {
                    ImageContent = bitmapImage,
                    ImageName = Path.GetFileNameWithoutExtension(dialog.FileName),
                });
                ImageOperationsWindow imageOperationsWindow = new ImageOperationsWindow(viewModel);

                Application.Current.MainWindow = imageOperationsWindow;
                imageOperationsWindow.ShowDialog();
            }
        }

        protected void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged(this, e);
        }
    }
}
