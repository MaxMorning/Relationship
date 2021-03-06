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

            if (MainWindow.mainWindow.currentPanelIdx == 4)
            {
                btSearchSearchName.Click += SearchPerson;
            }
            else
            {
                btSearchSearchName.Click += btSearchSearchName_Click;
            }

            this.Loaded += SearchPersonWindow_Loaded;
        }

        private void SearchPersonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard storyboard = (Storyboard)this.Resources["showAnim"];
            storyboard.Begin();
        }

        private void rectSearchPersonTop_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btSearchPersonClose_Click(object sender, RoutedEventArgs e)
        {
            this.CloseWindow();
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
                this.CloseWindow();
            }
        }

        private void btSearchSearchName_Click(object sender, RoutedEventArgs e)
        {
            spSearchSearchResult.Children.Clear();
            string key = tbSearchIdxName.Text;

            foreach (Person person in Person.persons)
            {
                if (person.enable && MainWindow.mainWindow.role != person && MainWindow.FuzzySearch(person.name, key))
                {
                    RoutedEventHandler routedEventHandler = (sArg, eArg) =>
                    {
                        selectPerson = person;
                        this.CloseWindow();
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

        private void SearchPerson(object sender, RoutedEventArgs e)
        {
            spSearchSearchResult.Children.Clear();
            string key = tbSearchIdxName.Text;

            foreach (Person person in Person.persons)
            {
                if (person.enable && person != MainWindow.mainWindow.role && MainWindow.FuzzySearch(person.name, key))
                {
                    RoutedEventHandler routedEventHandler = (sArg, eArg) =>
                    {
                        selectPerson = person;
                        this.CloseWindow();
                    };
                    spSearchSearchResult.Children.Add(new UserInfoDetailWithName(person, "选择", routedEventHandler));
                }
            }
        }

        private void CloseWindow()
        {
            Storyboard storyboard = (Storyboard)this.Resources["exitAnim"];
            storyboard.Completed += (sArg, eArg) =>
            {
                this.Close();
            };
            storyboard.Begin();
        }
    }
}
