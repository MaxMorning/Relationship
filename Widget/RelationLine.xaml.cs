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
    /// RelationLine.xaml 的交互逻辑
    /// </summary>
    public partial class RelationLine : Grid
    {
        public static List<RelationLine> allLinks = new List<RelationLine>();

        public PersonDot personDot0;
        public PersonDot personDot1;
        public double relationRate;
        public RelationLine(PersonDot personDot0, PersonDot personDot1, double rate)
        {
            InitializeComponent();

            if (personDot0.relatedPerson.id > personDot1.relatedPerson.id)
            {
                this.personDot0 = personDot1;
                this.personDot1 = personDot0;
            }
            else
            {
                this.personDot0 = personDot0;
                this.personDot1 = personDot1;
            }
            relationRate = rate;
            SetPosition();
        }

        public void SetPosition()
        {
            double person0Left = Canvas.GetLeft(personDot0);
            double person0Top = Canvas.GetTop(personDot0);
            double person1Left = Canvas.GetLeft(personDot1);
            double person1Top = Canvas.GetTop(personDot1);
            double angle = Math.Atan2(person1Top - person0Top, person1Left - person0Left) * 180 / Math.PI;
            rotateTransform.Angle = angle;

            double length = Math.Sqrt(Math.Pow(person1Top - person0Top, 2) + Math.Pow(person1Left - person0Left, 2));
            this.Width = length;

            Canvas.SetLeft(this, person0Left + 7.5);
            Canvas.SetTop(this, person0Top + 7.5 - 2.5);
            ToolTip = Width;
        }
    }
}
