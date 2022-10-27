using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;

namespace UTravellerModel.UTraveller.Converter
{
    public class HexColorConverter
    {
        private static Regex hexColorMatchRegex = 
            new Regex("^#?(?<a>[a-z0-9][a-z0-9])?(?<r>[a-z0-9][a-z0-9])(?<g>[a-z0-9][a-z0-9])(?<b>[a-z0-9][a-z0-9])$", 
                RegexOptions.IgnoreCase | RegexOptions.Compiled);
        
        public static Color GetColorFromHex(string hexColorString)
        {
            var convertedColor = Colors.Green;

            if (hexColorString != null)
            {

                var match = hexColorMatchRegex.Match(hexColorString);

                if (match.Success)
                {
                    byte a = 255, r = 0, b = 0, g = 0;
                    if (match.Groups["a"].Success)
                    {
                        a = System.Convert.ToByte(match.Groups["a"].Value, 16);
                    }
                    r = System.Convert.ToByte(match.Groups["r"].Value, 16);
                    b = System.Convert.ToByte(match.Groups["b"].Value, 16);
                    g = System.Convert.ToByte(match.Groups["g"].Value, 16);
                    convertedColor = Color.FromArgb(a, r, g, b);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Can't convert string \"{0}\" to argb or rgb color.", hexColorString));
                }
            }
            return convertedColor;
        }
    }
}
