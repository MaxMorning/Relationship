using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Relationship
{
    class LabColorSpace
    {
        public static double LabA;
        public static double LabB;

        public static Color LabToRGB(double l, double a, double b)
        {

            double var_Y = (l + 16.0) / 116.0;
            double var_X = a / 500.0 + var_Y;
            double var_Z = var_Y - b / 200.0;

            if (Math.Pow(var_Y, 3) > 0.008856) var_Y = Math.Pow(var_Y, 3);
            else var_Y = (var_Y - 16.0 / 116.0) / 7.787;
            if (Math.Pow(var_X, 3) > 0.008856) var_X = Math.Pow(var_X, 3);
            else var_X = (var_X - 16.0/ 116.0) / 7.787;
            if (Math.Pow(var_Z, 3) > 0.008856) var_Z = Math.Pow(var_Z, 3);
            else var_Z = (var_Z - 16.0/ 116.0) / 7.787;

            double X = 95.047 * var_X;    //ref_X =  95.047     Observer= 2°, Illuminant= D65
            double Y = 100.000 * var_Y;   //ref_Y = 100.000
            double Z = 108.883 * var_Z;    //ref_Z = 108.883


            var_X = X / 100.0;       //X from 0 to  95.047      (Observer = 2°, Illuminant = D65)
            var_Y = Y / 100.0;       //Y from 0 to 100.000
            var_Z = Z / 100.0;      //Z from 0 to 108.883

            double var_R = var_X * 3.2406 + var_Y * -1.5372 + var_Z * -0.4986;
            double var_G = var_X * -0.9689 + var_Y * 1.8758 + var_Z * 0.0415;
            double var_B = var_X * 0.0557 + var_Y * -0.2040 + var_Z * 1.0570;

            var_R = var_R > 0.0031308 ? 1.055 * Math.Pow(var_R, (1 / 2.4)) - 0.055 : 12.92 * var_R;
            var_G = var_G > 0.0031308 ? 1.055 * Math.Pow(var_G, (1 / 2.4)) - 0.055 : 12.92 * var_G;
            var_B = var_B > 0.0031308 ? 1.055 * Math.Pow(var_B, (1 / 2.4)) - 0.055 : 12.92 * var_B;

            double R = var_R * 255.0;
            double G = var_G * 255.0;
            double B = var_B * 255.0;

            byte colorR, colorG, colorB;
            if (R > 255)
            {
                colorR = 255;
            }
            else if (R < 0)
            {
                colorR = 0;
            }
            else
            {
                colorR = (byte)R;
            }

            if (G > 255)
            {
                colorG = 255;
            }
            else if (G < 0)
            {
                colorG = 0;
            }
            else
            {
                colorG = (byte)G;
            }

            if (B > 255)
            {
                colorB = 255;
            }
            else if (B < 0)
            {
                colorB = 0;
            }
            else
            {
                colorB = (byte)B;
            }
            Color color = new Color();
            color.A = 255;
            color.R = colorR;
            color.G = colorG;
            color.B = colorB;

            return color;
        }
    }
}
