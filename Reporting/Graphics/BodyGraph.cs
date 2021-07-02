using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;

namespace IQI.Intuition.Reporting.Graphics
{
    public class BodyGraph
    {
        private List<Area> _Areas;

        public BodyGraph()
        {
        }

        public List<Area> Areas
        {
            get
            {
                if (_Areas == null)
                {
                    _Areas = new List<Area>();
                }

                return _Areas;
            }
        }


        public string SerializeForRender()
        {
            var builder = new StringBuilder();


            bool first = true;
            foreach (var area in Areas)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    builder.Append(":");
                }



                builder.Append(area.TopLeftX);
                builder.Append("|");
                builder.Append(area.TopLeftY);
                builder.Append("|");
                builder.Append(area.BottomRightX);
                builder.Append("|");
                builder.Append(area.BottomRightY);
                builder.Append("|");
                builder.Append(area.ShadingOpacity);
                builder.Append("|");
                builder.Append(area.ShadingColor.ToArgb());

            }

            return builder.ToString();
        }

        public static MemoryStream GenerateImage(string data, string imagePath)
        {

            IEnumerable<string> areaData = null;

            if (data != null && data != string.Empty)
            {
                areaData = data.Split(":".ToCharArray());
            }

            var background = Image.FromFile(imagePath);

            var img = new Bitmap(background.Width, background.Height);
            var graphics = System.Drawing.Graphics.FromImage(img);

            /* draw background */

            graphics.DrawImage(background, 0, 0,background.Width,background.Height);

            /* break up data */

            if (areaData != null && areaData.First() != string.Empty)
            {

                var areas = new List<Area>();


                foreach (string chunk in areaData)
                {

                    string[] pieces = chunk.Split("|".ToCharArray());
                    int TopLeftX = Convert.ToInt32(pieces[0]);
                    int TopLeftY = Convert.ToInt32(pieces[1]);
                    int BottomRightX = Convert.ToInt32(pieces[2]);
                    int BottomRightY = Convert.ToInt32(pieces[3]);
                    int opacity = Convert.ToInt32(pieces[4]);
                    Color srcColor = Color.FromArgb(Convert.ToInt32(pieces[5]));


                    var area = new Area()
                    {
                        TopLeftX = TopLeftX,
                        TopLeftY = TopLeftY,
                        BottomRightX = BottomRightX,
                        BottomRightY = BottomRightY,
                        ShadingColor = srcColor,
                        ShadingOpacity = opacity
                    };

                    areas.Add(area);

                }



                foreach (var area in areas)
                {
                    for (int y = area.TopLeftY; y < area.BottomRightY; ++y)
                    {
                        for (int x = area.TopLeftX; x < area.BottomRightX; ++x)
                        {
                            if (img.GetPixel(x, y).R < 253 || img.GetPixel(x, y).G < 253 || img.GetPixel(x, y).B < 253)
                            {
                                float perc = (float)area.ShadingOpacity / 100f;
                                var c = Mix(img.GetPixel(x,y),area.ShadingColor,perc);
                                img.SetPixel(x, y, c);
                            }
                        }
                    }
                }

            }




            /* return image stream */

            var ms = new System.IO.MemoryStream();
            img.Save(ms, ImageFormat.Jpeg);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }


        public static Color Mix(Color from, Color to, float percent)
        {
            float amountFrom = 1.0f - percent;

            return Color.FromArgb(
            (byte)(from.A * amountFrom + to.A * percent),
            (byte)(from.R * amountFrom + to.R * percent),
            (byte)(from.G * amountFrom + to.G * percent),
            (byte)(from.B * amountFrom + to.B * percent));
        }

        public class Area
        {
            public int TopLeftX { get; set; }
            public int TopLeftY { get; set; }
            public int BottomRightX { get; set; }
            public int BottomRightY { get; set; }
            public int ShadingOpacity { get; set; }
            public Color ShadingColor { get; set; }
        }
    }
}
