using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.CompilerServices;

namespace IQI.Intuition.Reporting.Graphics
{
    public class VerticalTextLabel
    {
        public static Dictionary<string, Image> CachedLabels = new Dictionary<string, Image>();

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static MemoryStream RenderLabel(string label)
        {
            Image img;

            if (CachedLabels.Keys.Contains(label))
            {
                img = CachedLabels[label];
            }
            else
            {
                img = CreateLabel(label);
                CachedLabels[label] = img;
            }

            var ms = new System.IO.MemoryStream();
            img.Save(ms, ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        private static Image CreateLabel(string label)
        {
            var img = new Bitmap(25, 320);
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(img);

            //graphics.Clear(Color.White);

            int fontsize = 10;

            if (label.Length > 44)
            {
                fontsize = 8;
            }

            Font f = new System.Drawing.Font("Microsoft Sans Serif", fontsize, FontStyle.Bold);

            SizeF sz = graphics.VisibleClipBounds.Size;
            //Offset the coordinate system so that point (0, 0) is at the center of the desired area.
            graphics.TranslateTransform(sz.Width / 2, sz.Height / 2);
            //Rotate the Graphics object.
            graphics.RotateTransform(-90);
            sz = graphics.MeasureString(label, f);
            //Offset the Drawstring method so that the center of the string matches the center.
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            graphics.DrawString(label, f, Brushes.Black, -(sz.Width / 2), -(sz.Height / 2));
            //Reset the graphics object Transformations.
            graphics.ResetTransform();

            return img;
        }
    }
}
