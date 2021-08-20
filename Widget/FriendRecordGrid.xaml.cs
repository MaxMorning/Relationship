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
    /// FriendRecordGrid.xaml 的交互逻辑
    /// </summary>
    public partial class FriendRecordGrid : Grid
    {
        private Person relatedPerson;
        private int commonNum;
        public FriendRecordGrid(Person person, int num)
        {
            InitializeComponent();

            relatedPerson = person;
            commonNum = num;

            lbFriendRecordID.Content = person.id;
            lbFriendRecordName.Content = person.name + (num < 0 ? "" : (" (" + num.ToString() + ")"));
            lbFriendRecordGender.Content = person.gender;
            lbFriendRecordAge.Content = person.age;

            if (num == -1)
            {
                this.ContextMenu.Items.Remove(btFriendRecordDelete);
                this.ContextMenu.Items.Remove(btFriendRecordGetInfo);
            }
            else
            {
                this.ContextMenu.Items.Remove(btFriendRecordAddFriend);
            }
        }

        private void btFriendRecordDelete_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.role.BreakUpFriend(relatedPerson);
            MainWindow.mainWindow.FreshFriendList();
            MainWindow.mainWindow.Focus();
        }

        private void btFriendRecordGetInfo_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.InitPanel(1, relatedPerson.id);
            MainWindow.mainWindow.SwitchPanel(1, null);
            MainWindow.mainWindow.Focus();
        }

        private void btFriendRecordAddFriend_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.role.MakeFriend(relatedPerson);
            MainWindow.mainWindow.role.friends.Sort(Person.Compare);
            MainWindow.mainWindow.btRelationPossibleFriend_Click(null, null);

            MainWindow.mainWindow.Focus();
        }
    }
}
