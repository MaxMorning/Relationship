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
    /// GroupRecordGrid.xaml 的交互逻辑
    /// </summary>
    public partial class GroupRecordGrid : Grid
    {
        private SocialGroup relatedSocialGroup;

        public GroupRecordGrid(SocialGroup socialGroup, bool showContext)
        {
            InitializeComponent();

            relatedSocialGroup = socialGroup;
            lbGroupRecordID.Content = socialGroup.id;
            lbGroupRecordName.Content = socialGroup.name;
            int memberCount = 0;
            for (int i = 0; i < socialGroup.members.Count; ++i)
            {
                if (socialGroup.members[i].enable)
                {
                    ++memberCount;
                }
            }
            lbGroupRecordSize.Content = memberCount;

            if (!showContext)
            {
                this.ContextMenu = null;
            }
        }

        private void btGroupRecordDelete_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.role.QuitSocialGroup(relatedSocialGroup);
            MainWindow.mainWindow.FreshGroupList();
            MainWindow.mainWindow.Focus();
        }
    }
}
