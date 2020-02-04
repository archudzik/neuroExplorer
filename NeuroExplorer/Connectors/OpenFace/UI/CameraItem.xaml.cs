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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NeuroExplorer.Connectors.OpenFace.UI
{
    /// <summary>
    /// Interaction logic for CameraItem.xaml
    /// </summary>
    public partial class CameraItem : UserControl
    {
        private int cameraIndex = -1;

        public CameraItem()
        {
            InitializeComponent();
        }

        public int CameraIndex { get => cameraIndex; set => cameraIndex = value; }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            if(resolutions.SelectedIndex == -1)
            {
                messageSnackbar.IsActive = true;
            }
        }

        private void SnackbarMessage_ActionClick(object sender, RoutedEventArgs e)
        {
            messageSnackbar.IsActive = false;
        }
    }
}
