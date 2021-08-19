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
    /// ExpRecordGrid.xaml 的交互逻辑
    /// </summary>
    public partial class ExpRecordGrid : Grid
    {
        private Experience relatedExp;
        private int expType;

        public ExpRecordGrid(Experience experience, int expType)
        {
            InitializeComponent();
            this.relatedExp = experience;
            this.expType = expType;
            lbExpRecordValue.Content = experience.relatedGroup.name;

            lbExpRecordTime.Content = ConvertToMonth(experience.beginMonthIdx) + "-" + ConvertToMonth(experience.endMonthIdx);
        }

        public static string ConvertToMonth(int monthIdx)
        {
            string retStr = string.Format("{0:D4}", monthIdx / 12);
            string monthStr = string.Format("{0:D2}", monthIdx % 12 + 1);
            return retStr + "." + monthStr;
        }

        private void btExpRecordEdit_Click(object sender, RoutedEventArgs e)
        {
            EditExpWindow editExpWindow = new EditExpWindow(relatedExp, expType);
            editExpWindow.ShowEditDialog();

            relatedExp.owner.RepaintExp(expType);
        }

        private void btExpRecordDelete_Click(object sender, RoutedEventArgs e)
        {
            switch (expType)
            {
                case 0:
                    {
                        relatedExp.owner.liveExp.Remove(relatedExp);

                        ((LiveGroup)relatedExp.relatedGroup).relatedExp.Remove(relatedExp);
                        if (((LiveGroup)relatedExp.relatedGroup).relatedExp.Count == 0)
                        {
                            LiveGroup.liveGroups.Remove(relatedExp.relatedGroup.name);
                        }
                        break;
                    }

                case 1:
                    {
                        relatedExp.owner.eduExp.Remove(relatedExp);

                        ((EduGroup)relatedExp.relatedGroup).relatedExp.Remove(relatedExp);
                        if (((EduGroup)relatedExp.relatedGroup).relatedExp.Count == 0)
                        {
                            EduGroup.eduGroups.Remove(relatedExp.relatedGroup.name);
                        }
                        break;
                    }

                case 2:
                    {
                        relatedExp.owner.workExp.Remove(relatedExp);

                        ((WorkGroup)relatedExp.relatedGroup).relatedExp.Remove(relatedExp);
                        if (((WorkGroup)relatedExp.relatedGroup).relatedExp.Count == 0)
                        {
                            WorkGroup.workGroups.Remove(relatedExp.relatedGroup.name);
                        }
                        break;
                    }
            }

            relatedExp.owner.RepaintExp(expType);
            MainWindow.mainWindow.Focus();
        }
    }
}
