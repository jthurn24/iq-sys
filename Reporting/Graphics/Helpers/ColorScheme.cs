using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace IQI.Intuition.Reporting.Graphics.Helpers
{
    public static class ColorScheme
    {
        static Color[] _colorSet = new Color[12] { 
               System.Drawing.ColorTranslator.FromHtml("#4572A7"),
                System.Drawing.ColorTranslator.FromHtml("#AA4643"),
                System.Drawing.ColorTranslator.FromHtml("#89A54E"),
                System.Drawing.ColorTranslator.FromHtml("#80699B"),
                System.Drawing.ColorTranslator.FromHtml("#3D96AE"),
                System.Drawing.ColorTranslator.FromHtml("#DB843D"),
                System.Drawing.ColorTranslator.FromHtml("#92A8CD"),
                System.Drawing.ColorTranslator.FromHtml("#A47D7C"),
                System.Drawing.ColorTranslator.FromHtml("#B5CA92"),
                System.Drawing.ColorTranslator.FromHtml("#dee446"),
                System.Drawing.ColorTranslator.FromHtml("#e74d2a"),
                System.Drawing.ColorTranslator.FromHtml("#16e4f6")
            };

        public static string GetDefaultHtmlColor(int index)
        {
            var c = GetDefaultColor(index);

            return System.Drawing.ColorTranslator.ToHtml(c);
        }

        public static Color GetDefaultColor(int index)
        {
            if (index >= _colorSet.Length)
            {
                index = _colorSet.Length - 1;
            }

            return _colorSet[index];
        }

    }
}
