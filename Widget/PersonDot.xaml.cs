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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Relationship.Class;

namespace Relationship.Widget
{
    /// <summary>
    /// PersonDot.xaml 的交互逻辑
    /// </summary>
    public partial class PersonDot : Button
    {
        public static List<PersonDot> allPersonDots = new List<PersonDot>();
        public static double gravityRate;
        public static double repulsiveRate;
        public static Random random = new Random();

        public Person relatedPerson;
        public Point speed = new Point(0, 0);
        public List<PersonDot> noLinkPersonDot = new List<PersonDot>();
        public List<RelationLine> links = new List<RelationLine>();
        
        public PersonDot(Person person, PersonDot parent)
        {
            InitializeComponent();

            relatedPerson = person;
            person.relatedDot = this;
            this.ToolTip = person.id.ToString() + " " + person.name + " " + person.age.ToString();

            if (parent == null)
            {
                Canvas.SetLeft(this, 400);
                Canvas.SetTop(this, 300);
            }
            else
            {
                double parentLeft = Canvas.GetLeft(parent);
                double parentTop = Canvas.GetTop(parent);

                Canvas.SetLeft(this, parentLeft + random.Next(-200, 200));
                Canvas.SetTop(this, parentTop + random.Next(-200, 200));
            }

            this.Background = new SolidColorBrush(LabColorSpace.LabToRGB(33, LabColorSpace.LabA, LabColorSpace.LabB));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.InitPanel(1, relatedPerson.id);
            MainWindow.mainWindow.SwitchPanel(1, null);
        }

        public Point MoveDelta()
        {
            Point point = new Point(0, 0);

            double dotLeft = Canvas.GetLeft(this);
            double dotTop = Canvas.GetTop(this);

            for (int i = 0; i < links.Count; ++i)
            {
                double deltaLength = links[i].Width - 100;
                double angle = links[i].rotateTransform.Angle * Math.PI / 180;
                if (links[i].personDot0 != this)
                {
                    angle += Math.PI;
                }

                point.X += gravityRate * deltaLength * Math.Cos(angle);
                point.Y += gravityRate * deltaLength * Math.Sin(angle);
            }

            for (int i = 0; i < noLinkPersonDot.Count; ++i)
            {
                double noLinkLeft = Canvas.GetLeft(noLinkPersonDot[i]);
                double noLinkTop = Canvas.GetTop(noLinkPersonDot[i]);
                double length = Math.Sqrt(Math.Pow(noLinkLeft - dotLeft, 2) + Math.Pow(noLinkTop - dotTop, 2));

                double force;
                if (length < 15)
                {
                    force = PersonDot.repulsiveRate / 225;
                }
                else
                {
                    force = repulsiveRate / Math.Pow(length, 2);
                }
                point.X -= force * (noLinkLeft - dotLeft) / length;
                point.Y -= force * (noLinkTop - dotTop) / length;
            }

            double targetLength = Math.Sqrt(Math.Pow(point.X, 2) + Math.Pow(point.Y, 2));
            if (targetLength > 5)
            {
                point.X *= 5 / targetLength;
                point.Y *= 5 / targetLength;
            }
            return point;
        }

        public void SetNoLinks()
        {
            HashSet<PersonDot> linkedSet = new HashSet<PersonDot>(links.Count);
            linkedSet.Add(this);
            for (int i = 0; i < links.Count; ++i)
            {
                if (this == links[i].personDot0)
                {
                    linkedSet.Add(links[i].personDot1);
                }
                else
                {
                    linkedSet.Add(links[i].personDot0);
                }
            }

            for (int i = 0; i < PersonDot.allPersonDots.Count; ++i)
            {
                if (!linkedSet.Contains(PersonDot.allPersonDots[i]))
                {
                    noLinkPersonDot.Add(PersonDot.allPersonDots[i]);
                }
            }
        }

        public static void ExecEpoch(double damping)
        {
            for (int i = 0; i < allPersonDots.Count; ++i)
            {
                Point deltaForce = allPersonDots[i].MoveDelta();

                allPersonDots[i].speed.X += deltaForce.X * damping;
                allPersonDots[i].speed.Y += deltaForce.Y * damping;

                Canvas.SetLeft(allPersonDots[i], Canvas.GetLeft(allPersonDots[i]) + deltaForce.X * damping);
                Canvas.SetTop(allPersonDots[i], Canvas.GetTop(allPersonDots[i]) + deltaForce.Y * damping);
                
            }

            for (int i = 0; i < RelationLine.allLinks.Count; ++i)
            {
                RelationLine.allLinks[i].SetPosition();
            }
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            ExponentialEase exponentialEase = new ExponentialEase()
            {
                EasingMode = EasingMode.EaseOut
            };
            ColorAnimation colorAnimation = new ColorAnimation(LabColorSpace.LabToRGB(63, LabColorSpace.LabA, LabColorSpace.LabB), new Duration(TimeSpan.FromSeconds(0.3)));
            colorAnimation.EasingFunction = exponentialEase;
            Storyboard.SetTarget(colorAnimation, this);
            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath("(Button.Background).(SolidColorBrush.Color)"));

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(colorAnimation);
            storyboard.Begin();
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            ExponentialEase exponentialEase = new ExponentialEase()
            {
                EasingMode = EasingMode.EaseOut
            };
            ColorAnimation colorAnimation = new ColorAnimation(LabColorSpace.LabToRGB(33, LabColorSpace.LabA, LabColorSpace.LabB), new Duration(TimeSpan.FromSeconds(0.3)));
            colorAnimation.EasingFunction = exponentialEase;
            Storyboard.SetTarget(colorAnimation, this);
            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath("(Button.Background).(SolidColorBrush.Color)"));

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(colorAnimation);
            storyboard.Begin();
        }
    }
}
