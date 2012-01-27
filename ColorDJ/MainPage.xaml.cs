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

        // Constructor
        public MainPage()
        {
            InitializeComponent();
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
            if (instructions.Opacity != 0)
                instructions.Opacity = 0;

            StylusPoint p = e.StylusDevice.GetStylusPoints(rect).Last();
            double width = System.Windows.Application.Current.Host.Content.ActualWidth;
            double height = System.Windows.Application.Current.Host.Content.ActualHeight;

            byte u = (byte)(255 * p.X / width);
            byte v = (byte)(255 * p.Y / height);
            byte y = 160;
            byte[] rgb = YUVToRGB(y, u, v);
            rect.Fill = new SolidColorBrush(Color.FromArgb(0xFF, rgb[0], rgb[1], rgb[2]));
        }
    }
}