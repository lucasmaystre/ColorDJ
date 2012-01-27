using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace ColorDJ
{
    public partial class MainPage : PhoneApplicationPage
    {
        private Random rand = new Random();

        static readonly double[][] XYZ_RGB = new double[][] {
            new double[] { 3.240479, -1.537150, -0.498535 },
            new double[] { -0.969256, 1.875992, 0.041556 },
            new double[] { 0.055648, -0.204043, 1.057311 }
        };

        const double T1 = 0.008856;
        const double T2 = 0.206893;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private double[] LabToRGB(double l, double a, double b)
        {
            double[] lab = {l, a, b};
            double[] xyz = new double[3];
            double[] rgb = new double[3];
            
            // Lab -> XYZ
            double y = (l + 16) / 116;
            y = y * y * y;
            xyz[1] = y > T1 ? y : l / 903.3;
            y = y > T1 ? (l + 16) / 116 : 7.787 * y + (16.0 / 116);

            double x = y + (a / 500);
            xyz[0] = x > T2 ? x * x * x : (x - 16.0 / 116) / 7.787;

            double z = y - (b / 200);
            xyz[2] = z > T2 ? z * z * z : (z - 16.0 / 116) / 7.787;

            // Normalize for D65 white point.
            xyz[0] *= 0.950456;
            xyz[2] *= 1.088754;

            // XYZ -> RGB
            for (int i = 0; i < 3; ++i)
                for (int j = 0; j < 3; ++j)
                    rgb[i] += XYZ_RGB[i][j] * xyz[j];

            // Return integer values between 0 and 255
            return new double[] {
                /*(byte)*/Math.Max(0, Math.Min(rgb[0], 1))*255,
                /*(byte)*/Math.Max(0, Math.Min(rgb[1], 1))*255,
                /*(byte)*/Math.Max(0, Math.Min(rgb[2], 1))*255,
            };
            //byte[] rgb = new byte[3];
            //rand.NextBytes(rgb);
            //return rgb;
        }

        private byte[] YUVToRGB(byte y, byte u, byte v) 
        {
            byte[] rgb = new byte[3];
            int r = (int)(y + 1.4075 * (v - 128));
            int g = (int)(y - 0.3455 * (u - 128) - 0.7169 * (v - 128));
            int b = (int)(y + 1.7790 * (u - 128));
            rgb[0] = (byte)Math.Max(0, Math.Min(r, 255));
            rgb[1] = (byte)Math.Max(0, Math.Min(g, 255));
            rgb[2] = (byte)Math.Max(0, Math.Min(b, 255));
            return rgb;
        }

        private void rect_MouseMove(object sender, MouseEventArgs e)
        {
            StylusPoint p = e.StylusDevice.GetStylusPoints(rect).Last();
            double width = System.Windows.Application.Current.Host.Content.ActualWidth;
            double height = System.Windows.Application.Current.Host.Content.ActualHeight;

            //double l = 80;
            //double a = (220 * p.X / width) - 110;
            //double b = (220 * p.Y / height) - 110;
            //double[] rgb = LabToRGB(l, a, b);
            //rect.Fill = new SolidColorBrush(Color.FromArgb(0xFF, (byte)rgb[0], (byte)rgb[1], (byte)rgb[2]));
            byte u = (byte)(255 * p.X / width);
            byte v = (byte)(255 * p.Y / height);
            byte y = 160;
            byte[] rgb = YUVToRGB(y, u, v);
            rect.Fill = new SolidColorBrush(Color.FromArgb(0xFF, rgb[0], rgb[1], rgb[2]));
            //text.Text = "" + p.X / width;
            text.Text = "" + (int)rgb[0] + ":" + (int)rgb[1] + ":" + (int)rgb[2];
            //text2.Text = "" + (int)l + ":" + (int)a + ":" + (int)b;
            text2.Text = "" + y + ":" + u + ":" + v;
        }
    }
}