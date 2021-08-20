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
using System.Windows.Media.Animation;
using Relationship.Class;
using Relationship.Widget;


namespace Relationship
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static NonLinearEasingFunction nonLinearEasingFunction = new NonLinearEasingFunction(16)
        {
            EasingMode = EasingMode.EaseIn
        };

        public static int THREAD_NUM;
        public static MainWindow mainWindow;
        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

            mainWindow = this;
            Database.LoadProperties();
            
            bool initLoad = Database.ParseFile("database.txt");
            if (initLoad)
            {
                lbStartLoadStatus.Content = "已加载： database.txt";
            }
            else
            {
                lbStartLoadStatus.Content = "未加载";
            }
            // 左侧切换栏
            switchButtons[0] = btSwitchStart;
            switchButtons[1] = btSwitchInfo;
            switchButtons[2] = btSwitchSocial;
            switchButtons[3] = btSwitchVisualize;
            switchButtons[4] = btSwitchRelation;
            switchButtons[5] = btSwitchGroup;

            // 各个面板
            panelGrids[0] = gridBeginPanel;
            panelGrids[1] = gridInfoPanel;
            panelGrids[2] = gridSocialPanel;
            panelGrids[3] = gridVisualizePanel;
            panelGrids[4] = gridRelationPanel;
            panelGrids[5] = gridGroupPanel;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void MinButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private bool currentPersonEnable = true;
        private Person infoPanelPerson = null;
        public void InitPanel(int panelIdx, int personID)
        {
            switch (panelIdx)
            {
                case 1:
                    {
                        Person currentPerson = Person.persons[personID];
                        infoPanelPerson = currentPerson;
                        tbInfoName.Text = currentPerson.name;
                        tbInfoGender.Text = currentPerson.gender;
                        tbInfoAge.Text = currentPerson.age.ToString();
                        currentPersonEnable = currentPerson.enable;
                        lbInfoIndex.Content = "#" + currentPerson.id.ToString();
                        if (currentPerson.enable)
                        {
                            btInfoEnable.Content = "有效";
                        }
                        else
                        {
                            btInfoEnable.Content = "无效";
                        }


                        // Exp area set
                        spInfoLive.Children.Clear();
                        for (int i = 0; i < currentPerson.liveExp.Count; ++i)
                        {
                            spInfoLive.Children.Add(new ExpRecordGrid(currentPerson.liveExp[i], 0));
                        }

                        spInfoEdu.Children.Clear();
                        for (int i = 0; i < currentPerson.eduExp.Count; ++i)
                        {
                            spInfoEdu.Children.Add(new ExpRecordGrid(currentPerson.eduExp[i], 1));
                        }

                        spInfoWork.Children.Clear();
                        for (int i = 0; i < currentPerson.workExp.Count; ++i)
                        {
                            spInfoWork.Children.Add(new ExpRecordGrid(currentPerson.workExp[i], 2));
                        }
                        break;
                    }

                case 2:
                    {
                        FreshFriendList();
                        FreshGroupList();
                        break;
                    }
            }
        }
        
        // 左侧切换栏实现
        private Button[] switchButtons = new Button[6];
        private int currentPanelIdx = 0;
        private Grid[] panelGrids = new Grid[6];
        private void SwitchButton_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).MinWidth < 0.2)
            {
                return;
            }

            int clickIdx = 0;
            for (; clickIdx < 6 && sender != switchButtons[clickIdx]; ++clickIdx) { }

            if (currentPanelIdx == clickIdx)
            {
                return;
            }

            if (clickIdx != 3 && clickIdx != 4)
            {
                InitPanel(clickIdx, role.id);
                SwitchPanel(clickIdx, null);
            }
            else
            {
                SwitchPanel(clickIdx, (sArg, eArg) =>
                {
                    InitPanel(clickIdx, role.id);
                });
            }
        }

        public double SwitchPanel(int targetPanelIdx, EventHandler eventHandlerAfterAnim)
        {
            for (int i = 0; i < 6; ++i)
            {
                switchButtons[i].MinWidth = 1;
            }
            switchButtons[targetPanelIdx].MinWidth = 0;

            Storyboard storyboard = new Storyboard();

            double animTime = 0.7 + 0.3 * Math.Abs(targetPanelIdx - currentPanelIdx);

            for (int i = 0; i < 6; ++i)
            {
                DoubleAnimation doubleAnimation = new DoubleAnimation(680 * (i - targetPanelIdx), new Duration(TimeSpan.FromSeconds(animTime)));
                doubleAnimation.EasingFunction = nonLinearEasingFunction;
                Storyboard.SetTarget(doubleAnimation, panelGrids[i]);
                Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Canvas.Top)"));
                storyboard.Children.Add(doubleAnimation);
            }

            DoubleAnimation maskDoubleAnim = new DoubleAnimation(62.5 + 50 * targetPanelIdx, new Duration(TimeSpan.FromSeconds(animTime)));
            maskDoubleAnim.EasingFunction = nonLinearEasingFunction;
            Storyboard.SetTarget(maskDoubleAnim, pathSwitchMask);
            Storyboard.SetTargetProperty(maskDoubleAnim, new PropertyPath("(Canvas.Top)"));
            storyboard.Children.Add(maskDoubleAnim);

            if (eventHandlerAfterAnim != null)
            {
                storyboard.Completed += eventHandlerAfterAnim;
            }
            
            storyboard.Begin();
            currentPanelIdx = targetPanelIdx;
            return animTime;
        }

        public Person role = null;
        public void SetRole(Person person)
        {
            role = person;
            if (person.name.Length == 2)
            {
                lbSwitchRole.Content = person.name[0] + " " + person.name[1];
            }
            else
            {
                lbSwitchRole.Content = person.name;
            }
        }
        private void btStartConfirmIdx_Click(object sender, RoutedEventArgs e)
        {
            int index;
            try
            {
                index = int.Parse(tbStartIdxName.Text);
            }
            catch (FormatException)
            {
                index = -1;
            }
            
            spStartSearchResult.Children.Clear();

            if (index < 0 || index >= Person.persons.Count)
            {
                // 禁止切换至其它面板
                for (int i = 0; i < 6; ++i)
                {
                    switchButtons[i].MinWidth = 0;
                }

                AlertDialogWindow alertDialogWindow = new AlertDialogWindow("您输入的编号非法，请检查。");
                alertDialogWindow.ShowAlertDialog();
            }
            else
            {
                SetRole(Person.persons[index]);
                InitPanel(1, role.id);
                // 允许切换至其它面板
                for (int i = 1; i < 6; ++i)
                {
                    switchButtons[i].MinWidth = 1;
                }
                SwitchPanel(1, null);
            }
        }

        private void btStartCreateNew_Click(object sender, RoutedEventArgs e)
        {
            if (tbStartIdxName.Text.Length > 0)
            {
                SetRole(new Person(tbStartIdxName.Text));
                InitPanel(1, role.id);
                SwitchPanel(1, null);
            }
            else
            {
                AlertDialogWindow alertDialogWindow = new AlertDialogWindow("姓名不应为空，请检查。");
                alertDialogWindow.ShowAlertDialog();
            }
        }

        private void btInfoEnable_Click(object sender, RoutedEventArgs e)
        {
            currentPersonEnable = !currentPersonEnable;
            if (currentPersonEnable)
            {
                btInfoEnable.Content = "有效";
            }
            else
            {
                btInfoEnable.Content = "无效";
            }
        }

        private void btInfoSave_Click(object sender, RoutedEventArgs e)
        {
            if (tbInfoName.Text.Length > 0 && tbInfoGender.Text.Length > 0 && tbInfoAge.Text.Length > 0)
            {
                try
                {
                    infoPanelPerson.age = int.Parse(tbInfoAge.Text);
                    infoPanelPerson.name = tbInfoName.Text;
                    infoPanelPerson.gender = tbInfoGender.Text;
                    infoPanelPerson.enable = currentPersonEnable;

                    // set lbSwitchRole
                    if (infoPanelPerson == role)
                    {
                        SetRole(role);
                    }
                }
                catch (FormatException)
                {
                    AlertDialogWindow alertDialogWindow = new AlertDialogWindow("存在非法输入，请进行检查。");
                    alertDialogWindow.ShowAlertDialog();
                }
            }
            else
            {
                AlertDialogWindow alertDialogWindow = new AlertDialogWindow("存在非法输入，请进行检查。");
                alertDialogWindow.ShowAlertDialog();
            }

           
        }

        private void btStartSearchName_Click(object sender, RoutedEventArgs e)
        {
            spStartSearchResult.Children.Clear();
            //List<UserInfoWithoutNameGrid> searchResult = new List<UserInfoWithoutNameGrid>();
            string key = tbStartIdxName.Text;

            // todo parallel
            for (int idx = 0; idx < Person.persons.Count; ++idx)
            {
                if (FuzzySearch(Person.persons[idx].name, key))
                {
                    RoutedEventHandler routedEventHandler = (sArg, eArg) =>
                    {
                        SetRole(Person.persons[idx]);
                        InitPanel(1, Person.persons[idx].id);
                        SwitchPanel(1, null);
                    };
                    spStartSearchResult.Children.Add(new UserInfoDetailWithName(Person.persons[idx], "选择", routedEventHandler));
                }
            }
        }

        public static bool FuzzySearch(string name, string key)
        {
            return name.Contains(key);
        }

        private void btStartLoad_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            bool? selectResult = dialog.ShowDialog();
            if (selectResult == true)
            {
                bool loadSuccess = Database.ParseFile(dialog.FileName);
                if (loadSuccess)
                {
                    lbStartLoadStatus.Content = "已加载: " + dialog.FileName;
                }
                else
                {
                    lbStartLoadStatus.Content = "载入错误: " + dialog.FileName;

                    Person.persons.Clear();
                    LiveGroup.liveGroups.Clear();
                    EduGroup.eduGroups.Clear();
                    SocialGroup.socialGroups.Clear();
                    WorkGroup.workGroups.Clear();
                }
            }
        }

        private void btInfoNewLive_Click(object sender, RoutedEventArgs e)
        {
            Experience newExperience = new Experience();
            newExperience.owner = infoPanelPerson;

            EditExpWindow editExpWindow = new EditExpWindow(newExperience, 0);
            editExpWindow.ShowEditDialog();

            infoPanelPerson.RepaintExp(0);
        }

        private void btInfoNewEdu_Click(object sender, RoutedEventArgs e)
        {
            Experience newExperience = new Experience();
            newExperience.owner = infoPanelPerson;

            EditExpWindow editExpWindow = new EditExpWindow(newExperience, 1);
            editExpWindow.ShowEditDialog();

            infoPanelPerson.RepaintExp(1);
        }

        private void btInfoNewWork_Click(object sender, RoutedEventArgs e)
        {
            Experience newExperience = new Experience();
            newExperience.owner = infoPanelPerson;

            EditExpWindow editExpWindow = new EditExpWindow(newExperience, 2);
            editExpWindow.ShowEditDialog();

            infoPanelPerson.RepaintExp(2);
        }

        // Social Panel
        public void FreshFriendList()
        {
            spSocialFriend.Children.Clear();

            role.friends.Sort(Person.Compare);
            for (int idx = 0; idx < role.friends.Count; ++idx)
            {
                if (role.friends[idx].enable)
                {
                    spSocialFriend.Children.Add(new FriendRecordGrid(role.friends[idx], -1));
                }
            }
        }

        public void FreshGroupList()
        {
            spSocialGroup.Children.Clear();

            role.socialGroups.Sort(SocialGroup.Compare);
            for (int idx = 0; idx < role.socialGroups.Count; ++idx)
            {
                spSocialGroup.Children.Add(new GroupRecordGrid(role.socialGroups[idx], true));
            }
        }

        private void btSocialAddFriend_Click(object sender, RoutedEventArgs e)
        {
            SearchPersonWindow searchPersonWindow = new SearchPersonWindow();
            searchPersonWindow.ShowSearchDialog();

            if (searchPersonWindow.selectPerson != null)
            {
                if (role.friends.Contains(searchPersonWindow.selectPerson))
                {
                    role.BreakUpFriend(searchPersonWindow.selectPerson);
                }
                else
                {
                    role.MakeFriend(searchPersonWindow.selectPerson);
                }
                FreshFriendList();
            }
        }

        private void btSocialEditGroup_Click(object sender, RoutedEventArgs e)
        {
            SearchGroupWindow searchGroupWindow = new SearchGroupWindow();
            searchGroupWindow.ShowSearchDialog();

            if (searchGroupWindow.selectSocialGroup != null)
            {
                if (role.socialGroups.Contains(searchGroupWindow.selectSocialGroup))
                {
                    role.QuitSocialGroup(searchGroupWindow.selectSocialGroup);
                }
                else
                {
                    role.JoinSocialGroup(searchGroupWindow.selectSocialGroup);
                }
                FreshGroupList();
            }
        }
    }
}
