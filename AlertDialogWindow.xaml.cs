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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Relationship
{
    /// <summary>
    /// AlertDialogWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AlertDialogWindow : Window
    {
        private string alertMessage;
        public AlertDialogWindow(string message)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            alertMessage = message;
            lbAlertDialogMessage.Content = message;

            this.Loaded += AlertDialogWindow_Loaded;
        }

        private void AlertDialogWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard storyboard = (Storyboard)this.Resources["showAnim"];
            storyboard.Begin();
        }

        public bool? ShowAlertDialog()
        {
            this.Owner = Application.Current.Windows
                            .Cast<Window>()
                            .FirstOrDefault(window => window is MainWindow) as MainWindow;
            this.ShowInTaskbar = false;
            return this.ShowDialog();
        }

        private void btAlertDialogOK_Click(object sender, RoutedEventArgs e)
        {
            Storyboard storyboard = (Storyboard)this.Resources["exitAnim"];
            storyboard.Completed += (sArg, eArg) =>
            {
                this.Close();
            };
            storyboard.Begin();
        }

        private void DragWidget_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
