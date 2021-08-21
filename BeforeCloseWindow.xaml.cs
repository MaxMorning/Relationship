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
using Relationship.Class;

namespace Relationship
{
    /// <summary>
    /// BeforeCloseWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BeforeCloseWindow : Window
    {
        public bool shouldClose = false;
        public BeforeCloseWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        private void DragWidget_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public bool? ShowSaveDialog()
        {
            this.Owner = Application.Current.Windows
                            .Cast<Window>()
                            .FirstOrDefault(window => window is MainWindow) as MainWindow;
            this.ShowInTaskbar = false;
            return this.ShowDialog();
        }

        private void btSaveDialogSave_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.filePath == null)
            {
                AlertDialogWindow alertDialogWindow = new AlertDialogWindow("未加载相关文件。");
                alertDialogWindow.ShowAlertDialog();
                return;
            }
            bool storeSuccess = Database.StoreToFile(MainWindow.filePath);
            if (!storeSuccess)
            {
                AlertDialogWindow alertDialogWindow = new AlertDialogWindow("未能成功保存至相应文件。");
                alertDialogWindow.ShowAlertDialog();
                return;
            }
            this.shouldClose = false;
            this.Close();
        }

        private void btSaveDialogSaveQuit_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.filePath == null)
            {
                AlertDialogWindow alertDialogWindow = new AlertDialogWindow("未加载相关文件。");
                alertDialogWindow.ShowAlertDialog();
                return;
            }
            bool storeSuccess = Database.StoreToFile(MainWindow.filePath);
            if (!storeSuccess)
            {
                AlertDialogWindow alertDialogWindow = new AlertDialogWindow("未能成功保存至相应文件。");
                alertDialogWindow.ShowAlertDialog();
                return;
            }
            this.shouldClose = true;
            this.Close();
        }

        private void btSaveDialogQuit_Click(object sender, RoutedEventArgs e)
        {
            this.shouldClose = true;
            this.Close();
        }

        private void btSaveDialogCancel_Click(object sender, RoutedEventArgs e)
        {
            this.shouldClose = false;
            this.Close();
        }
    }
}
