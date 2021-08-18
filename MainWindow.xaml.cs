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

        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

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

        private int currentID = 0;
        private bool currentPersonEnable = true;
        private Person infoPanelPerson = null;
        private void InitPanel(int panelIdx, int personID)
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


            SwitchPanel(clickIdx, (sArg, eArg) =>
            {
                InitPanel(clickIdx, currentID);
            });

        }

        private double SwitchPanel(int targetPanelIdx, EventHandler eventHandlerAfterAnim)
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

        private Person role = null;
        private void SetRole(Person person)
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
    }
}
