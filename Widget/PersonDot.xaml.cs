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
    /// PersonDot.xaml 的交互逻辑
    /// </summary>
    public partial class PersonDot : Button
    {
        public static List<PersonDot> allPersonDots = new List<PersonDot>();
        public static double gravityRate;
        public static double repulsiveRate;
        public static Random random = new Random();

        public Person relatedPerson;
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

                Canvas.SetLeft(this, parentLeft + random.Next(-100, 100));
                Canvas.SetTop(this, parentTop + random.Next(-100, 100));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.InitPanel(1, relatedPerson.id);
            MainWindow.mainWindow.SwitchPanel(1, null);
        }

        public Point MoveDelta()
        {
            Point point = new Point(0, 0);

            for (int i = 0; i < links.Count; ++i)
            {
                double deltaLength = links[i].Width - 100;
                double angle = links[i].rotateTransform.Angle * Math.PI / 180;

                point.X -= gravityRate * deltaLength * Math.Cos(angle); 
                point.Y -= gravityRate * deltaLength * Math.Sin(angle); 
            }

            double dotLeft = Canvas.GetLeft(this);
            double dotTop = Canvas.GetTop(this);
            for (int i = 0; i < noLinkPersonDot.Count; ++i)
            {
                double noLinkLeft = Canvas.GetLeft(noLinkPersonDot[i]);
                double noLinkTop = Canvas.GetTop(noLinkPersonDot[i]);
                double length = Math.Sqrt(Math.Pow(noLinkLeft - dotLeft, 2) + Math.Pow(noLinkTop - dotTop, 2));

                double force;
                if (length < 15)
                {
                    force = 100;
                }
                else
                {
                    force = repulsiveRate / Math.Pow(length, 2);
                }
                point.X += force * (dotLeft - noLinkLeft) / length;
                point.Y += force * (dotTop - noLinkTop) / length;
            }
            return point;
        }

        public static void ExecEpoch()
        {
            for (int i = 0; i < allPersonDots.Count; ++i)
            {
                Point deltaForce = allPersonDots[i].MoveDelta();
                Canvas.SetLeft(allPersonDots[i], Canvas.GetLeft(allPersonDots[i]) + deltaForce.X);
                Canvas.SetTop(allPersonDots[i], Canvas.GetTop(allPersonDots[i]) + deltaForce.Y);
            }

            for (int i = 0; i < RelationLine.allLinks.Count; ++i)
            {
                RelationLine.allLinks[i].SetPosition();
            }
        }
    }
}
