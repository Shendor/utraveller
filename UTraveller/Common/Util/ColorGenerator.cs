using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace UTraveller.Common.Util
{
    public class ColorGenerator
    {
        private static readonly Color[] COLORS = new Color[]{
            Color.FromArgb(255, 0, 132, 218),
            Color.FromArgb(255, 240, 203, 0),
            Color.FromArgb(255, 72, 119, 227),
            Color.FromArgb(255, 108, 0, 218),
            Color.FromArgb(255, 202, 0, 0),
            Color.FromArgb(255, 162, 208, 0),
            Color.FromArgb(255, 88, 219, 0),
            Color.FromArgb(255, 0, 205, 198),
            Color.FromArgb(255, 88, 219, 0),
            Color.FromArgb(255, 224, 0, 200),
            Color.FromArgb(255, 156, 59, 218),
            Color.FromArgb(255, 255, 60, 0),
            Color.FromArgb(255, 30, 118, 55),
            Color.FromArgb(255, 177, 80, 28)
        };


        public static Color GetRandomColor(int index = 0)
        {
            if (index >= COLORS.Length)
            {
                index = 0;
            }

            return COLORS[index];
        }
    }
}
