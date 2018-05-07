using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ImageManagerWpfClient
{
    /// <summary>
    /// Interaction logic for AvailableTagsEditingWindow.xaml
    /// </summary>
    public partial class AvailableTagsEditingWindow : Window
    {
        public AvailableTagsEditingWindow()
        {
            InitializeComponent();
            DataContext = new AvailableTagsEditingWindowViewModel();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ((IDisposable) DataContext).Dispose();
        }
    }
}
