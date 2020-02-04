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

namespace NeuroExplorer.Connectors.EyeTracker.UI
{
    /// <summary>
    /// Interaction logic for EyeTribeUI.xaml
    /// </summary>
    public partial class EyeTribeUI : Window
    {
        public EyeTribeUI()
        {
            InitializeComponent();
        }

        private void SnackbarMessage_ActionClick(object sender, RoutedEventArgs e)
        {
            messageSnackbar.IsActive = false;
        }
    }
}
