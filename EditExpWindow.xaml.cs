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
    /// EditExpWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditExpWindow : Window
    {
        private Experience relatedExp;
        private int expType;
        public EditExpWindow(Experience experience, int expType)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

            relatedExp = experience;
            this.expType = expType;

            tbEditExpBeginTime.Text = ExpRecordGrid.ConvertToMonth(experience.beginMonthIdx);
            tbEditExpEndTime.Text = ExpRecordGrid.ConvertToMonth(experience.endMonthIdx);
            if (experience.relatedGroup == null)
            {
                tbEditExpValue.Text = "";
            }
            else
            {
                tbEditExpValue.Text = experience.relatedGroup.name;
            }

            this.Loaded += EditExpWindow_Loaded; ;
        }

        private void EditExpWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard storyboard = (Storyboard)this.Resources["showAnim"];
            storyboard.Begin();
        }

        private void rectEditExpTop_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btEditExpClose_Click(object sender, RoutedEventArgs e)
        {
            this.CloseWindow();
        }

        private void btEditExpConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int beginYear = int.Parse(tbEditExpBeginTime.Text.Substring(0, 4));
                int beginMonth = int.Parse(tbEditExpBeginTime.Text.Substring(5, 2)) - 1;

                int endYear = int.Parse(tbEditExpEndTime.Text.Substring(0, 4));
                int endMonth = int.Parse(tbEditExpEndTime.Text.Substring(5, 2)) - 1;

                if (tbEditExpValue.Text.Length == 0)
                {
                    throw new Exception();
                }

                // 检查合法性
                int beginMonthIdx = 12 * beginYear + beginMonth;
                int endMonthIdx = 12 * endYear + endMonth;

                if (beginMonthIdx >= endMonthIdx)
                {
                    throw new Exception();
                }
                
                Person person = relatedExp.owner;
                switch (expType)
                {
                    case 0:
                        {
                            foreach (Experience experience in person.liveExp)
                            {
                                if (experience != relatedExp && ((beginMonthIdx > experience.beginMonthIdx && beginMonthIdx < experience.endMonthIdx) || (endMonthIdx > experience.beginMonthIdx && endMonthIdx < experience.endMonthIdx)))
                                {
                                    throw new Exception();
                                }
                            }
                            break;
                        }

                    case 1:
                        {
                            foreach (Experience experience in person.eduExp)
                            {
                                if (experience != relatedExp && ((beginMonthIdx > experience.beginMonthIdx && beginMonthIdx < experience.endMonthIdx) || (endMonthIdx > experience.beginMonthIdx && endMonthIdx < experience.endMonthIdx)))
                                {
                                    throw new Exception();
                                }
                            }
                            break;
                        }

                    case 2:
                        {
                            foreach (Experience experience in person.workExp)
                            {
                                if (experience != relatedExp && ((beginMonthIdx > experience.beginMonthIdx && beginMonthIdx < experience.endMonthIdx) || (endMonthIdx > experience.beginMonthIdx && endMonthIdx < experience.endMonthIdx)))
                                {
                                    throw new Exception();
                                }
                            }
                            break;
                        }
                }


                relatedExp.beginMonthIdx = beginMonthIdx;
                relatedExp.endMonthIdx = endMonthIdx;
                if (relatedExp.relatedGroup != null)
                {
                    // 修改
                    switch (expType)
                    {
                        case 0:
                            {
                                LiveGroup liveGroup = (LiveGroup)(relatedExp.relatedGroup);
                                liveGroup.relatedExp.Remove(relatedExp);

                                bool isExist = LiveGroup.liveGroups.TryGetValue(tbEditExpValue.Text, out LiveGroup newLiveGroup);
                                if (!isExist)
                                {
                                    newLiveGroup = new LiveGroup();
                                    newLiveGroup.name = tbEditExpValue.Text;
                                    LiveGroup.liveGroups.Add(tbEditExpValue.Text, newLiveGroup);
                                }
                                newLiveGroup.relatedExp.Add(relatedExp);
                                relatedExp.relatedGroup = newLiveGroup;

                                if (liveGroup.relatedExp.Count == 0)
                                {
                                    LiveGroup.liveGroups.Remove(liveGroup.name);
                                }
                                relatedExp.owner.SortExp(relatedExp.owner.liveExp);
                                break;
                            }
                        case 1:
                            {
                                EduGroup eduGroup = (EduGroup)(relatedExp.relatedGroup);
                                eduGroup.relatedExp.Remove(relatedExp);

                                bool isExist = EduGroup.eduGroups.TryGetValue(tbEditExpValue.Text, out EduGroup newEduGroup);
                                if (!isExist)
                                {
                                    newEduGroup = new EduGroup();
                                    newEduGroup.name = tbEditExpValue.Text;
                                    EduGroup.eduGroups.Add(tbEditExpValue.Text, newEduGroup);
                                }
                                newEduGroup.relatedExp.Add(relatedExp);
                                relatedExp.relatedGroup = newEduGroup;

                                if (eduGroup.relatedExp.Count == 0)
                                {
                                    EduGroup.eduGroups.Remove(eduGroup.name);
                                }
                                relatedExp.owner.SortExp(relatedExp.owner.eduExp);
                                break;
                            }
                        case 2:
                            {
                                WorkGroup workGroup = (WorkGroup)(relatedExp.relatedGroup);
                                workGroup.relatedExp.Remove(relatedExp);

                                bool isExist = WorkGroup.workGroups.TryGetValue(tbEditExpValue.Text, out WorkGroup newWorkGroup);
                                if (!isExist)
                                {
                                    newWorkGroup = new WorkGroup();
                                    newWorkGroup.name = tbEditExpValue.Text;
                                    WorkGroup.workGroups.Add(tbEditExpValue.Text, newWorkGroup);
                                }
                                newWorkGroup.relatedExp.Add(relatedExp);
                                relatedExp.relatedGroup = newWorkGroup;

                                if (workGroup.relatedExp.Count == 0)
                                {
                                    WorkGroup.workGroups.Remove(workGroup.name);
                                }
                                relatedExp.owner.SortExp(relatedExp.owner.workExp);
                                break;
                            }
                    }
                }
                else
                {
                    // 新建
                    switch (expType)
                    {
                        case 0:
                            {
                                bool searchSuccess = LiveGroup.liveGroups.TryGetValue(tbEditExpValue.Text, out LiveGroup liveGroup);
                                if (!searchSuccess)
                                {
                                    liveGroup = new LiveGroup();
                                    liveGroup.name = tbEditExpValue.Text;

                                    LiveGroup.liveGroups.Add(tbEditExpValue.Text, liveGroup);
                                }

                                liveGroup.relatedExp.Add(relatedExp);
                                relatedExp.relatedGroup = liveGroup;

                                int i = 0;
                                for (; i < relatedExp.owner.liveExp.Count; ++i)
                                {
                                    if (relatedExp.owner.liveExp[i].beginMonthIdx > relatedExp.beginMonthIdx)
                                    {
                                        relatedExp.owner.liveExp.Insert(i, relatedExp);
                                        break;
                                    }
                                }
                                if (i == relatedExp.owner.liveExp.Count)
                                {
                                    relatedExp.owner.liveExp.Add(relatedExp);
                                }
                                break;
                            }

                        case 1:
                            {
                                bool searchSuccess = EduGroup.eduGroups.TryGetValue(tbEditExpValue.Text, out EduGroup eduGroup);
                                if (!searchSuccess)
                                {
                                    eduGroup = new EduGroup();
                                    eduGroup.name = tbEditExpValue.Text;

                                    EduGroup.eduGroups.Add(tbEditExpValue.Text, eduGroup);
                                }

                                eduGroup.relatedExp.Add(relatedExp);
                                relatedExp.relatedGroup = eduGroup;
                                int i = 0;
                                for (; i < relatedExp.owner.eduExp.Count; ++i)
                                {
                                    if (relatedExp.owner.eduExp[i].beginMonthIdx > relatedExp.beginMonthIdx)
                                    {
                                        relatedExp.owner.eduExp.Insert(i, relatedExp);
                                        break;
                                    }
                                }
                                if (i == relatedExp.owner.eduExp.Count)
                                {
                                    relatedExp.owner.eduExp.Add(relatedExp);
                                }
                                break;
                            }

                        case 2:
                            {
                                bool searchSuccess = WorkGroup.workGroups.TryGetValue(tbEditExpValue.Text, out WorkGroup workGroup);
                                if (!searchSuccess)
                                {
                                    workGroup = new WorkGroup();
                                    workGroup.name = tbEditExpValue.Text;

                                    WorkGroup.workGroups.Add(tbEditExpValue.Text, workGroup);
                                }

                                workGroup.relatedExp.Add(relatedExp);
                                relatedExp.relatedGroup = workGroup;
                                int i = 0;
                                for (; i < relatedExp.owner.workExp.Count; ++i)
                                {
                                    if (relatedExp.owner.workExp[i].beginMonthIdx > relatedExp.beginMonthIdx)
                                    {
                                        relatedExp.owner.workExp.Insert(i, relatedExp);
                                        break;
                                    }
                                }
                                if (i == relatedExp.owner.workExp.Count)
                                {
                                    relatedExp.owner.workExp.Add(relatedExp);
                                }
                                break;
                            }
                    }
                }
                this.CloseWindow();
            }
            catch (Exception)
            {
                AlertDialogWindow alertDialogWindow = new AlertDialogWindow("输入有误，请进行检查。");
                alertDialogWindow.ShowAlertDialog();
            }
        }

        public bool? ShowEditDialog()
        {
            this.Owner = Application.Current.Windows
                            .Cast<Window>()
                            .FirstOrDefault(window => window is MainWindow) as MainWindow;
            this.ShowInTaskbar = false;
            return this.ShowDialog();
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
