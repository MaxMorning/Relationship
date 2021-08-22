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
using System.Threading;
using System.Diagnostics;

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

        public static int THREAD_NUM = 8;
        public static MainWindow mainWindow;

        public static string filePath = null;
        public static int showCommonFriendsMax = 128;
        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

            mainWindow = this;
            Database.LoadProperties();
            
            bool initLoad = Database.ParseFile("database.txt");
            if (initLoad)
            {
                filePath = "database.txt";
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

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard storyboard = (Storyboard)this.Resources["showAnim"];
            storyboard.Begin();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            BeforeCloseWindow beforeCloseWindow = new BeforeCloseWindow();
            beforeCloseWindow.ShowSaveDialog();

            if (beforeCloseWindow.shouldClose)
            {
                Storyboard storyboard = (Storyboard)this.Resources["exitAnim"];
                storyboard.Completed += (sArg, eArg) =>
                {
                    System.Windows.Application.Current.Shutdown();
                };
                storyboard.Begin();
            }
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
                        LeaveStartPanel();
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
                        LeaveStartPanel();
                        FreshFriendList();
                        FreshGroupList();
                        break;
                    }

                case 3:
                    {
                        LeaveStartPanel();
                        break;
                    }

                case 4:
                    {
                        LeaveStartPanel();
                        spPossibleFriend.Children.Clear();
                        spRelationRelationship.Children.Clear();
                        break;
                    }

                case 5:
                    {
                        LeaveStartPanel();
                        groupPanelGroup = null;
                        gridGroupDetail.Opacity = 0;
                        btGroupSave.Opacity = 0;
                        break;
                    }
            }
        }
        
        private void LeaveStartPanel()
        {
            tbStartIdxName.Text = "";
            spStartSearchResult.Children.Clear();
        }

        // 左侧切换栏实现
        private Button[] switchButtons = new Button[6];
        public int currentPanelIdx = 0;
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

            InitPanel(clickIdx, role.id);
            SwitchPanel(clickIdx, null);
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
            canvasVisualizeDrawCanvas.Children.Clear();
            canvasLeftBias = 0;
            canvasTopBias = 0;

            Random random = new Random();
            LabColorSpace.LabA = random.NextDouble() * 88 - 128;
            LabColorSpace.LabB = random.NextDouble() * 100 - 50;

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

            if (Person.persons.Count >= 2048)
            {
                // Parallel Search
                Thread[] threads = new Thread[THREAD_NUM - 1];
                List<Person>[] threadSearchResult = new List<Person>[THREAD_NUM];
                int idxTask = Person.persons.Count / THREAD_NUM;
                for (int i = 0; i < THREAD_NUM - 1; ++i)
                {
                    threadSearchResult[i + 1] = new List<Person>(idxTask);
                    int newI = i;
                    threads[i] = new Thread(() =>
                    {
                        for (int idx = newI * idxTask; idx < (newI + 1) * idxTask; ++idx)
                        {
                            if (FuzzySearch(Person.persons[idx].name, key))
                            {
                                threadSearchResult[newI + 1].Add(Person.persons[idx]);
                            }
                        }
                    });
                }
                for (int i = 0; i < THREAD_NUM - 1; ++i)
                {
                    threads[i].Start();
                }

                // main thread
                threadSearchResult[0] = new List<Person>(idxTask);
                for (int idx = (THREAD_NUM - 1) * idxTask; idx < Person.persons.Count; ++idx)
                {
                    if (FuzzySearch(Person.persons[idx].name, key))
                    {
                        threadSearchResult[0].Add(Person.persons[idx]);
                    }
                }

                for (int i = 0; i < THREAD_NUM - 1; ++i)
                {
                    threads[i].Join();
                }


                for (int i = 1; i < THREAD_NUM; ++i)
                {
                    int newI = i;
                    for (int idx = 0; idx < threadSearchResult[i].Count; ++idx)
                    {
                        int newIdx = idx;
                        RoutedEventHandler routedEventHandler = (sArg, eArg) =>
                        {
                            SetRole(threadSearchResult[newI][newIdx]);
                            InitPanel(1, threadSearchResult[newI][newIdx].id);
                            SwitchPanel(1, null);
                        };
                        spStartSearchResult.Children.Add(new UserInfoDetailWithName(threadSearchResult[newI][newIdx], "选择", routedEventHandler));
                    }
                }

                for (int idx = 0; idx < threadSearchResult[0].Count; ++idx)
                {
                    int newIdx = idx;
                    RoutedEventHandler routedEventHandler = (sArg, eArg) =>
                    {
                        SetRole(threadSearchResult[0][newIdx]);
                        InitPanel(1, threadSearchResult[0][newIdx].id);
                        SwitchPanel(1, null);
                    };
                    spStartSearchResult.Children.Add(new UserInfoDetailWithName(threadSearchResult[0][newIdx], "选择", routedEventHandler));
                }
            }
            else
            {
                foreach (Person person in Person.persons)
                {
                    if (FuzzySearch(person.name, key))
                    {
                        RoutedEventHandler routedEventHandler = (sArg, eArg) =>
                        {
                            SetRole(person);
                            InitPanel(1, person.id);
                            SwitchPanel(1, null);
                        };
                        spStartSearchResult.Children.Add(new UserInfoDetailWithName(person, "选择", routedEventHandler));
                    }
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
                    filePath = dialog.FileName;
                    lbStartLoadStatus.Content = "已加载: " + dialog.FileName;
                    spStartSearchResult.Children.Clear();
                    canvasVisualizeDrawCanvas.Children.Clear();
                }
                else
                {
                    lbStartLoadStatus.Content = "载入错误: " + dialog.FileName;
                    filePath = null;

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

        // Group Panel
        private SocialGroup groupPanelGroup = null;
        private void btGroupSearch_Click(object sender, RoutedEventArgs e)
        {
            int index;
            try
            {
                index = int.Parse(tbGroupIdx.Text);
            }
            catch (FormatException)
            {
                index = -1;
            }

            spGroupMemberList.Children.Clear();

            if (index < 0 || index >= SocialGroup.socialGroups.Count)
            {
                AlertDialogWindow alertDialogWindow = new AlertDialogWindow("您输入的编号非法，请检查。");
                alertDialogWindow.ShowAlertDialog();
            }
            else
            {
                groupPanelGroup = SocialGroup.socialGroups[index];
                SetGroupPanel();
            }
        }

        private void SetGroupPanel()
        {
            gridGroupDetail.Opacity = 1;
            btGroupSave.Opacity = 1;

            tbGroupGroupName.Text = groupPanelGroup.name;
            tbGroupMemberIdx.Text = "";

            FreshGroupMemberList(false);
        }

        private void FreshGroupMemberList(bool sort)
        {
            spGroupMemberList.Children.Clear();

            if (sort)
            {
                groupPanelGroup.members.Sort(Person.Compare);
            }
            foreach (Person person in groupPanelGroup.members)
            {
                if (person.enable)
                {
                    RoutedEventHandler routedEventHandler = (sArg, eArg) =>
                    {
                        person.QuitSocialGroup(groupPanelGroup);
                        FreshGroupMemberList(false);
                    };
                    spGroupMemberList.Children.Add(new UserInfoDetailWithName(person, "移除", routedEventHandler));
                }
            }
        }

        private void tbGroupAddMember_Click(object sender, RoutedEventArgs e)
        {
            int index;
            try
            {
                index = int.Parse(tbGroupMemberIdx.Text);
            }
            catch (FormatException)
            {
                index = -1;
            }

            if (index < 0 || index >= Person.persons.Count)
            {
                AlertDialogWindow alertDialogWindow = new AlertDialogWindow("您输入的编号非法，请检查。");
                alertDialogWindow.ShowAlertDialog();
            }
            else
            {
                Person addMember = Person.persons[index];
                addMember.JoinSocialGroup(groupPanelGroup);
                FreshGroupMemberList(true);
            }
        }

        private void btGroupSave_Click(object sender, RoutedEventArgs e)
        {
            if (tbGroupGroupName.Text.Length > 0)
            {
                groupPanelGroup.name = tbGroupGroupName.Text;
            }
            else
            {
                AlertDialogWindow alertDialogWindow = new AlertDialogWindow("群组名称不能为空，请检查。");
                alertDialogWindow.ShowAlertDialog();
            }
        }

        private void btGroupCreate_Click(object sender, RoutedEventArgs e)
        {
            if (tbGroupIdx.Text.Length > 0)
            {
                SocialGroup newSocialGroup = new SocialGroup(tbGroupIdx.Text);
                newSocialGroup.id = SocialGroup.socialGroups.Count;
                SocialGroup.socialGroups.Add(newSocialGroup);
                groupPanelGroup = newSocialGroup;

                tbGroupGroupName.Text = newSocialGroup.name;
                tbGroupIdx.Text = newSocialGroup.id.ToString();

                btGroupSave.Opacity = 1;
                gridGroupDetail.Opacity = 1;

                FreshGroupMemberList(false);
            }
            else
            {
                AlertDialogWindow alertDialogWindow = new AlertDialogWindow("群组名称不能为空，请检查。");
                alertDialogWindow.ShowAlertDialog();
            }
        }

        // Relationship Panel
        private void btRelationChooseTarget_Click(object sender, RoutedEventArgs e)
        {
            SearchPersonWindow searchPersonWindow = new SearchPersonWindow();
            searchPersonWindow.ShowSearchDialog();

            if (searchPersonWindow.selectPerson != null)
            {
                List<Person> relationChain = role.GetRelationship(searchPersonWindow.selectPerson, out List<string> relations);

                if (relationChain != null)
                {
                    spRelationRelationship.Children.Clear();
                    spRelationRelationship.Children.Add(new FriendRecordGrid(role, -1));

                    for (int i = 1; i < relationChain.Count; ++i)
                    {
                        spRelationRelationship.Children.Add(Person.GetRelationLabel(relations[i]));
                        spRelationRelationship.Children.Add(new FriendRecordGrid(relationChain[i], -1));
                    }
                }
                else
                {
                    AlertDialogWindow alertDialogWindow = new AlertDialogWindow("未找到从" + role.name + "到" + searchPersonWindow.selectPerson.name + "的关系。");
                    alertDialogWindow.ShowAlertDialog();
                }
            }
        }

        public void btRelationPossibleFriend_Click(object sender, RoutedEventArgs e)
        {
            List<Person> possibleFriend = role.PossibleFriend(out List<int> commonFriendCount);

            SortPossibleFriend(possibleFriend, commonFriendCount);
            spPossibleFriend.Children.Clear();

            int showItemCount = commonFriendCount.Count;
            if (MainWindow.showCommonFriendsMax >= 0 && MainWindow.showCommonFriendsMax < showItemCount)
            {
                showItemCount = MainWindow.showCommonFriendsMax;
            }
            for (int i = 0; i < showItemCount; ++i)
            {
                spPossibleFriend.Children.Add(new FriendRecordGrid(possibleFriend[i], commonFriendCount[i]));
            }
        }

        // todo parallel
        private void SortPossibleFriend(List<Person> possibleFriend, List<int> commonFriendCount)
        {
            if (possibleFriend.Count >= 32768)
            {
                QuickSortPack sortPack = new QuickSortPack()
                {
                    possibleFriend = possibleFriend,
                    commonFriendCount = commonFriendCount,
                    left = 0,
                    right = possibleFriend.Count - 1
                };
                QuickSortPackFunc(sortPack);
            }
            else
            {
                QuickSort(possibleFriend, commonFriendCount, 0, possibleFriend.Count - 1);
            }
        }

        private class QuickSortPack
        {
            public List<Person> possibleFriend;
            public List<int> commonFriendCount;
            public int left;
            public int right;

            public static ParallelOptions parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = THREAD_NUM
            };
        }

        private void QuickSortPackFunc(object state)
        {
            QuickSortPack quickSortPack = (QuickSortPack)state;

            if (quickSortPack.right - quickSortPack.left < 1024)
            {
                QuickSort(quickSortPack.possibleFriend, quickSortPack.commonFriendCount, quickSortPack.left, quickSortPack.right);
                return;
            }


            if (quickSortPack.left < quickSortPack.right)
            {
                int pivot = Division(quickSortPack.possibleFriend, quickSortPack.commonFriendCount, quickSortPack.left, quickSortPack.right);

                QuickSortPack leftPack = new QuickSortPack()
                {
                    possibleFriend = quickSortPack.possibleFriend,
                    commonFriendCount = quickSortPack.commonFriendCount,
                    left = quickSortPack.left,
                    right = pivot - 1
                };

                QuickSortPack rightPack = new QuickSortPack()
                {
                    possibleFriend = quickSortPack.possibleFriend,
                    commonFriendCount = quickSortPack.commonFriendCount,
                    left = pivot + 1,
                    right = quickSortPack.right
                };

                Parallel.Invoke(QuickSortPack.parallelOptions, 
                    () =>
                    {
                        QuickSortPackFunc(leftPack);
                    },
                    () =>
                    {
                        QuickSortPackFunc(rightPack);
                    });
            }
        }

        private void QuickSort(List<Person> possibleFriend, List<int> commonFriendCount, int left, int right)
        {
            if (left < right)
            {
                int pivot = Division(possibleFriend, commonFriendCount, left, right);

                QuickSort(possibleFriend, commonFriendCount, left, pivot - 1);
                QuickSort(possibleFriend, commonFriendCount, pivot + 1, right);
            }
        }

        private int Division(List<Person> possibleFriend, List<int> commonFriendCount, int left, int right)
        {
            while (left < right)
            {
                int num = commonFriendCount[left];
                Person person = possibleFriend[left];
                if (num < commonFriendCount[left + 1])
                {
                    commonFriendCount[left] = commonFriendCount[left + 1];
                    commonFriendCount[left + 1] = num;

                    possibleFriend[left] = possibleFriend[left + 1];
                    possibleFriend[left + 1] = person;

                    ++left;
                }
                else
                {
                    int temp = commonFriendCount[right];
                    commonFriendCount[right] = commonFriendCount[left + 1];
                    commonFriendCount[left + 1] = temp;

                    Person tempPerson = possibleFriend[right];
                    possibleFriend[right] = possibleFriend[left + 1];
                    possibleFriend[left + 1] = tempPerson;

                    --right;
                }
            }

            return left;
        }

        // Visualize Panel
        private double canvasLeftBias = 0;
        private double canvasTopBias = 0;
        private void btVisualizeStart_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).MinWidth < 0.5)
            {
                return;
            }
            try
            {
                int totalLayer = int.Parse(tbVisualizeLayer.Text);
                if (totalLayer <= 1)
                {
                    AlertDialogWindow alertDialogWindow = new AlertDialogWindow("展示层数不能小于2。");
                    alertDialogWindow.ShowAlertDialog();
                    return;
                }
                PersonDot.gravityRate = double.Parse(tbVisualizeGravity.Text);
                PersonDot.repulsiveRate = double.Parse(tbVisualizeRepulsive.Text);
                int iterTime = int.Parse(tbVisualizeIterTime.Text);
                if (iterTime < 1)
                {
                    AlertDialogWindow alertDialogWindow = new AlertDialogWindow("迭代次数不能小于1。");
                    alertDialogWindow.ShowAlertDialog();
                    return;
                }

                ((Button)sender).MinWidth = 0;
                btVisualizeContIter.MinWidth = 0;
                // set lists
                PersonDot.allPersonDots.Clear();
                canvasVisualizeDrawCanvas.Children.Clear();
                Dictionary<int, RelationLine> uniqueRelations = new Dictionary<int, RelationLine>();
                HashSet<Person> uniquePerson = new HashSet<Person>();

                List<Person>[] personInLayers = new List<Person>[totalLayer + 1];
                for (int i = 0; i < totalLayer + 1; ++i)
                {
                    personInLayers[i] = new List<Person>();
                }

                // clear temp
                for (int i = 0; i < Person.persons.Count; ++i)
                {
                    Person.persons[i].tempSchoolmates = null;
                    Person.persons[i].tempGroupmates = null;
                    Person.persons[i].tempColleagues = null;
                    Person.persons[i].tempCitizens = null;
                }

                personInLayers[0].Add(role);
                PersonDot roleDot = new PersonDot(role, null);
                PersonDot.allPersonDots.Add(roleDot);
                uniquePerson.Add(role);
                for (int i = 0; i < totalLayer; ++i)
                {
                    for (int idx = 0; idx < personInLayers[i].Count; ++idx)
                    {
                        // check friends
                        for (int j = 0; j < personInLayers[i][idx].friends.Count; ++j)
                        {
                            Person person = personInLayers[i][idx].friends[j];
                            if (person.enable)
                            {
                                if (!uniquePerson.Contains(person))
                                {
                                    PersonDot personDot = new PersonDot(person, personInLayers[i][idx].relatedDot);
                                    PersonDot.allPersonDots.Add(personDot);
                                    personInLayers[i + 1].Add(person);
                                    uniquePerson.Add(person);
                                }

                                int key = person.id > personInLayers[i][idx].id ? personInLayers[i][idx].id * Person.persons.Count + person.id : person.id * Person.persons.Count + personInLayers[i][idx].id;
                                bool searchSuccess = uniqueRelations.TryGetValue(key, out RelationLine relationLine);

                                if (!searchSuccess || relationLine.relationRate < Person.friendRate)
                                {
                                    uniqueRelations[key] = new RelationLine(person.relatedDot, personInLayers[i][idx].relatedDot, Person.friendRate, "好友");
                                }
                            }
                        }

                        // check schoolmates
                        HashSet<Person> relatedSchoolmates = personInLayers[i][idx].GetRelatedSchoolmates();
                        foreach (Person person in relatedSchoolmates)
                        {
                            if (person.enable)
                            {
                                if (!uniquePerson.Contains(person))
                                {
                                    PersonDot personDot = new PersonDot(person, personInLayers[i][idx].relatedDot);
                                    PersonDot.allPersonDots.Add(personDot);
                                    personInLayers[i + 1].Add(person);
                                    uniquePerson.Add(person);
                                }

                                int key = person.id > personInLayers[i][idx].id ? personInLayers[i][idx].id * Person.persons.Count + person.id : person.id * Person.persons.Count + personInLayers[i][idx].id;
                                bool searchSuccess = uniqueRelations.TryGetValue(key, out RelationLine relationLine);

                                if (!searchSuccess || relationLine.relationRate < Person.schoolmateRate)
                                {
                                    uniqueRelations[key] = new RelationLine(person.relatedDot, personInLayers[i][idx].relatedDot, Person.schoolmateRate, "校友");
                                }
                            }
                        }

                        // check groupmates
                        HashSet<Person> relatedGroupmates = personInLayers[i][idx].GetRelatedGroupmates();
                        foreach (Person person in relatedGroupmates)
                        {
                            if (person.enable)
                            {
                                if (!uniquePerson.Contains(person))
                                {
                                    PersonDot personDot = new PersonDot(person, personInLayers[i][idx].relatedDot);
                                    PersonDot.allPersonDots.Add(personDot);
                                    personInLayers[i + 1].Add(person);
                                    uniquePerson.Add(person);
                                }

                                int key = person.id > personInLayers[i][idx].id ? personInLayers[i][idx].id * Person.persons.Count + person.id : person.id * Person.persons.Count + personInLayers[i][idx].id;
                                bool searchSuccess = uniqueRelations.TryGetValue(key, out RelationLine relationLine);

                                if (!searchSuccess || relationLine.relationRate < Person.groupmateRate)
                                {
                                    uniqueRelations[key] = new RelationLine(person.relatedDot, personInLayers[i][idx].relatedDot, Person.groupmateRate, "群友");
                                }
                            }
                        }

                        // check colleagues
                        HashSet<Person> relatedColleagues = personInLayers[i][idx].GetRelatedColleagues();
                        foreach (Person person in relatedColleagues)
                        {
                            if (person.enable)
                            {
                                if (!uniquePerson.Contains(person))
                                {
                                    PersonDot personDot = new PersonDot(person, personInLayers[i][idx].relatedDot);
                                    PersonDot.allPersonDots.Add(personDot);
                                    personInLayers[i + 1].Add(person);
                                    uniquePerson.Add(person);
                                }

                                int key = person.id > personInLayers[i][idx].id ? personInLayers[i][idx].id * Person.persons.Count + person.id : person.id * Person.persons.Count + personInLayers[i][idx].id;
                                bool searchSuccess = uniqueRelations.TryGetValue(key, out RelationLine relationLine);

                                if (!searchSuccess || relationLine.relationRate < Person.colleagueRate)
                                {
                                    uniqueRelations[key] = new RelationLine(person.relatedDot, personInLayers[i][idx].relatedDot, Person.colleagueRate, "同事");
                                }
                            }
                        }

                        // check citizens
                        HashSet<Person> relatedCitizens = personInLayers[i][idx].GetRelatedCitizens();
                        foreach (Person person in relatedCitizens)
                        {
                            if (person.enable)
                            {
                                if (!uniquePerson.Contains(person))
                                {
                                    PersonDot personDot = new PersonDot(person, personInLayers[i][idx].relatedDot);
                                    PersonDot.allPersonDots.Add(personDot);
                                    personInLayers[i + 1].Add(person);
                                    uniquePerson.Add(person);
                                }

                                int key = person.id > personInLayers[i][idx].id ? personInLayers[i][idx].id * Person.persons.Count + person.id : person.id * Person.persons.Count + personInLayers[i][idx].id;
                                bool searchSuccess = uniqueRelations.TryGetValue(key, out RelationLine relationLine);

                                if (!searchSuccess || relationLine.relationRate < Person.citizenRate)
                                {
                                    uniqueRelations[key] = new RelationLine(person.relatedDot, personInLayers[i][idx].relatedDot, Person.citizenRate, "同乡");
                                }
                            }
                        }
                    }
                }

                // last layer process
                for (int idx = 0; idx < personInLayers[totalLayer].Count; ++idx)
                {
                    // check friends
                    for (int j = 0; j < personInLayers[totalLayer][idx].friends.Count; ++j)
                    {
                        Person person = personInLayers[totalLayer][idx].friends[j];
                        if (person.enable && uniquePerson.Contains(person))
                        {
                            int key = person.id > personInLayers[totalLayer][idx].id ? personInLayers[totalLayer][idx].id * Person.persons.Count + person.id : person.id * Person.persons.Count + personInLayers[totalLayer][idx].id;
                            bool searchSuccess = uniqueRelations.TryGetValue(key, out RelationLine relationLine);

                            if (!searchSuccess || relationLine.relationRate < Person.friendRate)
                            {
                                uniqueRelations[key] = new RelationLine(person.relatedDot, personInLayers[totalLayer][idx].relatedDot, Person.friendRate, "好友");
                            }
                        }
                    }

                    // check schoolmates
                    HashSet<Person> relatedSchoolmates = personInLayers[totalLayer][idx].GetRelatedSchoolmates();
                    foreach (Person person in relatedSchoolmates)
                    {
                        if (person.enable && uniquePerson.Contains(person))
                        {
                            int key = person.id > personInLayers[totalLayer][idx].id ? personInLayers[totalLayer][idx].id * Person.persons.Count + person.id : person.id * Person.persons.Count + personInLayers[totalLayer][idx].id;
                            bool searchSuccess = uniqueRelations.TryGetValue(key, out RelationLine relationLine);

                            if (!searchSuccess || relationLine.relationRate < Person.schoolmateRate)
                            {
                                uniqueRelations[key] = new RelationLine(person.relatedDot, personInLayers[totalLayer][idx].relatedDot, Person.schoolmateRate, "校友");
                            }
                        }
                    }

                    // check schoolmates
                    HashSet<Person> relatedGroupmates = personInLayers[totalLayer][idx].GetRelatedGroupmates();
                    foreach (Person person in relatedGroupmates)
                    {
                        if (person.enable && uniquePerson.Contains(person))
                        {
                            int key = person.id > personInLayers[totalLayer][idx].id ? personInLayers[totalLayer][idx].id * Person.persons.Count + person.id : person.id * Person.persons.Count + personInLayers[totalLayer][idx].id;
                            bool searchSuccess = uniqueRelations.TryGetValue(key, out RelationLine relationLine);

                            if (!searchSuccess || relationLine.relationRate < Person.groupmateRate)
                            {
                                uniqueRelations[key] = new RelationLine(person.relatedDot, personInLayers[totalLayer][idx].relatedDot, Person.groupmateRate, "群友");
                            }
                        }
                    }

                    // check colleagues
                    HashSet<Person> relatedColleagues = personInLayers[totalLayer][idx].GetRelatedColleagues();
                    foreach (Person person in relatedColleagues)
                    {
                        if (person.enable && uniquePerson.Contains(person))
                        {
                            int key = person.id > personInLayers[totalLayer][idx].id ? personInLayers[totalLayer][idx].id * Person.persons.Count + person.id : person.id * Person.persons.Count + personInLayers[totalLayer][idx].id;
                            bool searchSuccess = uniqueRelations.TryGetValue(key, out RelationLine relationLine);

                            if (!searchSuccess || relationLine.relationRate < Person.colleagueRate)
                            {
                                uniqueRelations[key] = new RelationLine(person.relatedDot, personInLayers[totalLayer][idx].relatedDot, Person.colleagueRate, "同事");
                            }
                        }
                    }

                    // check citizens
                    HashSet<Person> relatedCitizens = personInLayers[totalLayer][idx].GetRelatedCitizens();
                    foreach (Person person in relatedCitizens)
                    {
                        if (person.enable && uniquePerson.Contains(person))
                        {
                            int key = person.id > personInLayers[totalLayer][idx].id ? personInLayers[totalLayer][idx].id * Person.persons.Count + person.id : person.id * Person.persons.Count + personInLayers[totalLayer][idx].id;
                            bool searchSuccess = uniqueRelations.TryGetValue(key, out RelationLine relationLine);

                            if (!searchSuccess || relationLine.relationRate < Person.citizenRate)
                            {
                                uniqueRelations[key] = new RelationLine(person.relatedDot, personInLayers[totalLayer][idx].relatedDot, Person.citizenRate, "同乡");
                            }
                        }
                    }
                }

                // write to allLinks
                RelationLine.allLinks.Clear();
                foreach (RelationLine relationLine in uniqueRelations.Values)
                {
                    relationLine.personDot0.links.Add(relationLine);
                    relationLine.personDot1.links.Add(relationLine);
                    RelationLine.allLinks.Add(relationLine);
                    canvasVisualizeDrawCanvas.Children.Add(relationLine);
                }

                Person.allPersonDots.Clear();
                for (int i = 0; i < PersonDot.allPersonDots.Count; ++i)
                {
                    canvasVisualizeDrawCanvas.Children.Add(PersonDot.allPersonDots[i]);
                    Person.allPersonDots.Add(PersonDot.allPersonDots[i].relatedPerson);
                    PersonDot.allPersonDots[i].relatedPerson.links.Clear();

                    // set nolinks
                    PersonDot.allPersonDots[i].SetNoLinks();
                }

                foreach (RelationLine relationLine in uniqueRelations.Values)
                {
                    relationLine.personDot0.relatedPerson.links.Add(relationLine.personDot1.relatedPerson);
                    relationLine.personDot1.relatedPerson.links.Add(relationLine.personDot0.relatedPerson);
                }

                // start iter
                for (int i = 0; i < iterTime; ++i)
                {
                    Person.ExecEpoch();
                }
                Person.ApplyPosition();

                canvasLeftBias = 400 - Canvas.GetLeft(roleDot);
                canvasTopBias = 300 - Canvas.GetTop(roleDot);
                Canvas.SetLeft(vbVisualizeCanvasViewbox, canvasLeftBias);
                Canvas.SetTop(vbVisualizeCanvasViewbox, canvasTopBias);

                ((Button)sender).MinWidth = 1;
                btVisualizeContIter.MinWidth = 1;
            }
            catch (Exception)
            {
                AlertDialogWindow alertDialogWindow = new AlertDialogWindow("存在非法输入，请检查。");
                alertDialogWindow.ShowAlertDialog();
            }
        }

        private double prevHoriChange = 0;
        private double prevVerChange = 0;
        private double currentScaleRate = 1;
        private void thumbVisualize_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            double currentLeft = Canvas.GetLeft(vbVisualizeCanvasViewbox);
            double currentTop = Canvas.GetTop(vbVisualizeCanvasViewbox);

            double newCanvasLeft = currentLeft + e.HorizontalChange - prevHoriChange;
            Canvas.SetLeft(vbVisualizeCanvasViewbox, newCanvasLeft);

            double newCanvasTop = currentTop + e.VerticalChange - prevVerChange;
            Canvas.SetTop(vbVisualizeCanvasViewbox, newCanvasTop);

            prevHoriChange = e.HorizontalChange;
            prevVerChange = e.VerticalChange;
        }

        private void thumbVisualize_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            prevHoriChange = 0;
            prevVerChange = 0;
        }

        public void thumbVisualize_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta == 0)
            {
                return;
            }

            double destination = e.Delta / Math.Abs(e.Delta);

            if (currentScaleRate < 0.01 && destination < 0)
            {
                return;
            }

            if (currentScaleRate > 8 && destination > 0)
            {
                return;
            }

            double prevScaleRate = currentScaleRate;
            if (destination > 0)
            {
                currentScaleRate *= 1.11111;
            }
            else
            {
                currentScaleRate *= 0.9;
            }
            //currentScaleRate += destination * 0.1;
            Point targetZoomFocus = e.GetPosition(vbVisualizeCanvasViewbox);

            scaletransVisualizeDrawCanvas.ScaleX = currentScaleRate;
            scaletransVisualizeDrawCanvas.ScaleY = currentScaleRate;


            double deltaX = targetZoomFocus.X * (currentScaleRate - prevScaleRate);
            double deltaY = targetZoomFocus.Y * (currentScaleRate - prevScaleRate);

            double newCanvasLeft = Canvas.GetLeft(vbVisualizeCanvasViewbox) - deltaX;
            double newCanvasTop = Canvas.GetTop(vbVisualizeCanvasViewbox) - deltaY;
            Canvas.SetLeft(vbVisualizeCanvasViewbox, newCanvasLeft);
            Canvas.SetTop(vbVisualizeCanvasViewbox, newCanvasTop);
        }

        private void thumbVisualize_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Storyboard storyboard = new Storyboard();
            double durationTime = Math.Sqrt(Math.Pow(Canvas.GetLeft(vbVisualizeCanvasViewbox), 2) + Math.Pow(Canvas.GetTop(vbVisualizeCanvasViewbox), 2)) / 1000;
            DoubleAnimation leftAnim = new DoubleAnimation(canvasLeftBias, new Duration(TimeSpan.FromSeconds(durationTime)));
            leftAnim.EasingFunction = nonLinearEasingFunction;
            Storyboard.SetTarget(leftAnim, vbVisualizeCanvasViewbox);
            Storyboard.SetTargetProperty(leftAnim, new PropertyPath("(Canvas.Left)"));
            storyboard.Children.Add(leftAnim);

            DoubleAnimation topAnim = new DoubleAnimation(canvasTopBias, new Duration(TimeSpan.FromSeconds(durationTime)));
            topAnim.EasingFunction = nonLinearEasingFunction;
            Storyboard.SetTarget(topAnim, vbVisualizeCanvasViewbox);
            Storyboard.SetTargetProperty(topAnim, new PropertyPath("(Canvas.Top)"));
            storyboard.Children.Add(topAnim);

            DoubleAnimation scaleXAnim = new DoubleAnimation(1, new Duration(TimeSpan.FromSeconds(durationTime)));
            scaleXAnim.EasingFunction = nonLinearEasingFunction;
            Storyboard.SetTarget(scaleXAnim, vbVisualizeCanvasViewbox);
            Storyboard.SetTargetProperty(scaleXAnim, new PropertyPath("RenderTransform.ScaleX"));
            storyboard.Children.Add(scaleXAnim);

            DoubleAnimation scaleYAnim = new DoubleAnimation(1, new Duration(TimeSpan.FromSeconds(durationTime)));
            scaleYAnim.EasingFunction = nonLinearEasingFunction;
            Storyboard.SetTarget(scaleYAnim, vbVisualizeCanvasViewbox);
            Storyboard.SetTargetProperty(scaleYAnim, new PropertyPath("RenderTransform.ScaleY"));
            storyboard.Children.Add(scaleYAnim);

            storyboard.Completed += (sArg, eArg) =>
            {
                storyboard.Remove(vbVisualizeCanvasViewbox);
                Canvas.SetLeft(vbVisualizeCanvasViewbox, canvasLeftBias);
                Canvas.SetTop(vbVisualizeCanvasViewbox, canvasTopBias);
                scaletransVisualizeDrawCanvas.ScaleX = 1;
                scaletransVisualizeDrawCanvas.ScaleY = 1;
                currentScaleRate = 1;
            };
            storyboard.Begin(vbVisualizeCanvasViewbox, true);
        }

        private void btVisualizeContIter_Click(object sender, RoutedEventArgs e)
        {
            if (btVisualizeContIter.MinWidth < 0.5)
            {
                return;
            }

            try
            {
                PersonDot.gravityRate = double.Parse(tbVisualizeGravity.Text);
                PersonDot.repulsiveRate = double.Parse(tbVisualizeRepulsive.Text);
                int iterTime = int.Parse(tbVisualizeIterTime.Text);
                if (iterTime < 1)
                {
                    AlertDialogWindow alertDialogWindow = new AlertDialogWindow("迭代次数不能小于1。");
                    alertDialogWindow.ShowAlertDialog();
                    return;
                }

                btVisualizeStart.MinWidth = 0;
                btVisualizeContIter.MinWidth = 0;
                for (int i = 0; i < iterTime; ++i)
                {
                    Person.ExecEpoch();
                }
                Person.ApplyPosition();

                canvasLeftBias = 400 - Canvas.GetLeft(PersonDot.allPersonDots[0]);
                canvasTopBias = 300 - Canvas.GetTop(PersonDot.allPersonDots[0]);
                Canvas.SetLeft(vbVisualizeCanvasViewbox, canvasLeftBias);
                Canvas.SetTop(vbVisualizeCanvasViewbox, canvasTopBias);

                btVisualizeStart.MinWidth = 1;
                btVisualizeContIter.MinWidth = 1;
            }
            catch (Exception)
            {
                AlertDialogWindow alertDialogWindow = new AlertDialogWindow("存在非法输入，请检查。");
                alertDialogWindow.ShowAlertDialog();
            }
        }
    }
}
