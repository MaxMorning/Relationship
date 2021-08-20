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
using Relationship.Widget;

namespace Relationship
{
    /// <summary>
    /// SearchGroupWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SearchGroupWindow : Window
    {
        public SocialGroup selectSocialGroup = null;

        public SearchGroupWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

            spSearchGroup.Children.Clear();
            spSearchGroupButton.Children.Clear();
        }

        private void btSearchGroupClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void rectSearchGroupTop_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btSearchGroupName_Click(object sender, RoutedEventArgs e)
        {
            spSearchGroup.Children.Clear();
            spSearchGroupButton.Children.Clear();
            string key = tbSearchIdxName.Text;

            // todo parallel
            foreach (SocialGroup socialGroup in SocialGroup.socialGroups)
            {
                // todo parallel
                if (MainWindow.FuzzySearch(socialGroup.name, key))
                {
                    spSearchGroup.Children.Add(new GroupRecordGrid(socialGroup, false));
                    RoutedEventHandler routedEventHandler = (sArg, eArg) =>
                    {
                        selectSocialGroup = socialGroup;
                        this.Close();
                    };
                    Button confirmButton = new Button();
                    confirmButton.Margin = new Thickness(5);
                    confirmButton.FontSize = 26;
                    confirmButton.MinWidth = 1;
                    confirmButton.Click += routedEventHandler;
                    confirmButton.SetValue(Button.StyleProperty, Application.Current.Resources["SwitchButton"]);
                    if (MainWindow.mainWindow.role.socialGroups.Contains(socialGroup))
                    {
                        confirmButton.Content = "移除";
                    }
                    else
                    {
                        confirmButton.Content = "添加";
                    }
                    spSearchGroupButton.Children.Add(confirmButton);
                }
            }
        }

        private void btSearchConfirmIdx_Click(object sender, RoutedEventArgs e)
        {
            int index;
            try
            {
                index = int.Parse(tbSearchIdxName.Text);
            }
            catch (FormatException)
            {
                index = -1;
            }

            spSearchGroup.Children.Clear();
            spSearchGroupButton.Children.Clear();

            if (index < 0 || index >= SocialGroup.socialGroups.Count)
            {
                AlertDialogWindow alertDialogWindow = new AlertDialogWindow("您输入的编号非法，请检查。");
                alertDialogWindow.ShowAlertDialog();
            }
            else
            {
                selectSocialGroup = SocialGroup.socialGroups[index];
                this.Close();
            }
        }

        public bool? ShowSearchDialog()
        {
            this.Owner = MainWindow.mainWindow;
            this.ShowInTaskbar = false;
            return this.ShowDialog();
        }
    }
}
