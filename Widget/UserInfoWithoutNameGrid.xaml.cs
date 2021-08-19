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
using Relationship.Class;

namespace Relationship.Widget
{
    /// <summary>
    /// UserInfoGrid.xaml 的交互逻辑
    /// </summary>
    public partial class UserInfoWithoutNameGrid : Grid
    {
        private Person relatedPerson;
        public UserInfoWithoutNameGrid(Person person)
        {
            InitializeComponent();

            relatedPerson = person;
            lbUserInfoID.Content = person.id;
            lbUserInfoAge.Content = person.age;
            lbUserInfoGender.Content = person.gender;
            lbUserInfoEdu.Content = person.GetRecentEdu(out _);
            lbUserInfoWork.Content = person.GetRecentWork(out _);
            lbUserInfoLive.Content = person.GetRecentLive(out _);
        }

        private void btUserInfoSelect_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindowInst = Application.Current.Windows
                                    .Cast<Window>()
                                    .FirstOrDefault(window => window is MainWindow) as MainWindow;

            mainWindowInst.SetRole(relatedPerson);
            mainWindowInst.InitPanel(1, relatedPerson.id);
            mainWindowInst.SwitchPanel(1, null);
        }
    }
}
