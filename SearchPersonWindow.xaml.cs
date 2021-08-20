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
    /// SearchPersonWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SearchPersonWindow : Window
    {
        public Person selectPerson = null;

        public SearchPersonWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        private void rectSearchPersonTop_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btSearchPersonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public bool? ShowSearchDialog()
        {
            this.Owner = Application.Current.Windows
                            .Cast<Window>()
                            .FirstOrDefault(window => window is MainWindow) as MainWindow;
            this.ShowInTaskbar = false;
            return this.ShowDialog();
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

            spSearchSearchResult.Children.Clear();

            if (index < 0 || index >= Person.persons.Count)
            {
                AlertDialogWindow alertDialogWindow = new AlertDialogWindow("您输入的编号非法，请检查。");
                alertDialogWindow.ShowAlertDialog();
            }
            else
            {
                selectPerson = Person.persons[index];
                this.Close();
            }
        }

        private void btSearchSearchName_Click(object sender, RoutedEventArgs e)
        {
            spSearchSearchResult.Children.Clear();
            string key = tbSearchIdxName.Text;

            // todo parallel
            foreach (Person person in Person.persons)
            {
                // todo parallel
                if (person.enable && MainWindow.FuzzySearch(person.name, key))
                {
                    RoutedEventHandler routedEventHandler = (sArg, eArg) =>
                    {
                        selectPerson = person;
                        this.Close();
                    };
                    if (MainWindow.mainWindow.role.friends.Contains(person))
                    {
                        spSearchSearchResult.Children.Add(new UserInfoDetailWithName(person, "移除", routedEventHandler));
                    }
                    else
                    {
                        spSearchSearchResult.Children.Add(new UserInfoDetailWithName(person, "添加", routedEventHandler));
                    }

                }
            }
        }
    }
}
