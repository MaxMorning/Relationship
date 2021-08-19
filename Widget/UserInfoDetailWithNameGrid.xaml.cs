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
    /// UserInfoDetailWithName.xaml 的交互逻辑
    /// </summary>
    public partial class UserInfoDetailWithName : Grid
    {
        private Person relatedPerson;

        public UserInfoDetailWithName(Person person, string buttonName, RoutedEventHandler eventHandler)
        {
            InitializeComponent();

            relatedPerson = person;
            lbUserInfoID.Content = person.id;
            lbUserInfoName.Content = person.name;
            lbUserInfoGender.Content = person.gender;
            lbUserInfoArea.Content = person.GetRecentLive(out _);

            string recentEdu = person.GetRecentEdu(out int eduMonthIdx);
            string recentWork = person.GetRecentWork(out int workMonthIdx);
            lbUserInfoPlace.Content = eduMonthIdx >= workMonthIdx ? recentEdu : recentWork;

            btUserInfo.Content = buttonName;
            btUserInfo.Click += eventHandler;
        }
    }
}
