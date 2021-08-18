using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace Relationship
{
    class NonLinearEasingFunction : EasingFunctionBase
    {
        private readonly int sampleCnt;
        public NonLinearEasingFunction(int sampleCnt)
            : base()
        {
            this.sampleCnt = sampleCnt;
        }
        protected override Freezable CreateInstanceCore()
        {
            return new NonLinearEasingFunction(sampleCnt);
        }

        protected override double EaseInCore(double normalizedTime)
        {
            double indexX = normalizedTime * sampleCnt;
            /*
            double normalRate = 20 / 1053.0;
            double value = -0.2 * Math.Pow(indexX, 5) + 0.75 * Math.Pow(indexX, 4) - 3 * Math.Pow(indexX, 3) + 13.5 * Math.Pow(indexX, 2);
            value *= normalRate;
            return value;
            */
            // return valueTable[(int)indexX];
            return ((-indexX - 1) * Math.Pow(Math.E, -indexX)) + 1;
        }
    }
}
