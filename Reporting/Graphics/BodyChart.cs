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
    public class BodyChart
    {
        private List<Circle> _Circles;
        private bool _Disabled { get; set; }

        public BodyChart(bool disabled)
        {
            _Disabled = disabled;
        }

        public List<Circle> Circles
        {
            get
            {
                if (_Circles == null)
                {
                    _Circles = new List<Circle>();
                }

                return _Circles;
            }
        }


        public string SerializeForRender()
        {
            var builder = new StringBuilder();

            builder.Append(_Disabled);
            builder.Append(":");

            bool first = true;
            foreach (var circle in Circles)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    builder.Append(":");
                }

                builder.Append(circle.Coordinates);
                builder.Append("|");
                builder.Append(circle.ShadingOpacity);
                builder.Append("|");
                builder.Append(circle.ShadingColor.ToArgb());
                builder.Append("|");
                builder.Append(circle.Width);
            }

            return builder.ToString();
        }

        public static MemoryStream GenerateImage(string data, string imagePath, int? w, int? h)
        {

            IEnumerable<string> circleData = null;
            string settingsData = string.Empty;
            bool disabled = false;

            settingsData = data.Split(":".ToCharArray())[0];
            circleData = data.Split(":".ToCharArray()).Skip(1);
            disabled = Boolean.Parse(settingsData);

            var background = Image.FromFile(imagePath);

            var img = new Bitmap(background.Width, background.Height);
            var graphics = System.Drawing.Graphics.FromImage(img);

            /* draw background */

            graphics.DrawImage(background, 0, 0,background.Width,background.Height);

            /* break up data */

            if (circleData.First() != string.Empty)
            {

                var circles = new List<Circle>();


                foreach (string chunk in circleData)
                {

                    string[] pieces = chunk.Split("|".ToCharArray());
                    string coordinates = pieces[0];
                    int opacity = Convert.ToInt32(pieces[1]);
                    Color srcColor = Color.FromArgb(Convert.ToInt32(pieces[2]));
                    int width = Convert.ToInt32(pieces[3]);

                    var circle = new Circle()
                    {
                        Coordinates = coordinates,
                        ShadingColor = srcColor,
                        ShadingOpacity = opacity,
                        Width = width
                    };

                    circles.Add(circle);

                }

                var coordinateList = circles.Select(x => x.Coordinates).Distinct();

                foreach (var coordinate in coordinateList)
                {
                    var circlesToDraw = circles.Where(x => x.Coordinates == coordinate).OrderByDescending(x => x.Width);

                    bool pinDrawn = false;
                    int circleYOffset = 0;

                    foreach (var circle in circlesToDraw)
                    {
                        var point = new Point();
                        point.X = Convert.ToInt32(circle.Coordinates.Split(",".ToCharArray())[0]);
                        point.Y = Convert.ToInt32(circle.Coordinates.Split(",".ToCharArray())[1]);
                        Color finalColor = Color.FromArgb(circle.ShadingOpacity, (int)circle.ShadingColor.R, (int)circle.ShadingColor.G, (int)circle.ShadingColor.B);

                        /* draw the shaded portion */
                        var brush = new SolidBrush(finalColor);
                        var shadowBrush = new SolidBrush(Color.FromArgb(100, Color.DarkGray));
                        var pinBrush = new SolidBrush(Color.Black);
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;

                        int circleX = point.X - (circle.Width / 2);
                        int circleY = point.Y - (circle.Width / 2);


                        if (pinDrawn == false)
                        {

                            pinDrawn = true;
                            circleYOffset = circle.Width;

                            int pinwidth = circle.Width / 4;

                            /* Draw shadows */

                            graphics.FillPolygon(shadowBrush,
                            new Point[]
                            {
                                new Point(point.X + 5,  (circleY - circleYOffset) + 8 + circle.Width - (int)(circle.Width / 4)),
                                new Point(point.X,point.Y),
                                new Point(point.X + (int)(pinwidth / 2) + 10, (circleY - circleYOffset) + 8 + circle.Width)
                            });

                            graphics.FillEllipse(shadowBrush, circleX + 15, (circleY - circleYOffset) + 10, circle.Width, circle.Width);


                            /* Draw pin */

                            graphics.FillPolygon(pinBrush,
                                new Point[]
                            {
                                new Point(point.X - (int)(pinwidth / 2), circleY),
                                new Point(point.X,point.Y),
                                new Point(point.X + (int)(pinwidth / 2), circleY)
                            });
                        }


                        graphics.FillEllipse(brush, circleX, circleY - circleYOffset, circle.Width, circle.Width);
                        graphics.DrawEllipse(new Pen(Color.Black), circleX, circleY - circleYOffset, circle.Width, circle.Width);
                    }

                }

            }

            /* If disabled, greyscale whole image */

            if (disabled)
            {
                img = MakeGrayscale3(img);
            }

            /* return image stream */

            if (h.HasValue && w.HasValue)
            {

                Bitmap newImage = new Bitmap(w.Value, h.Value);
                using (System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(newImage))
                {
                    gr.SmoothingMode = SmoothingMode.HighQuality;
                    gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    gr.DrawImage(img, new Rectangle(0, 0, w.Value, h.Value));
                }

                img = newImage;
            }


            var ms = new System.IO.MemoryStream();
            img.Save(ms, ImageFormat.Jpeg);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }


        private static Bitmap MakeGrayscale3(Bitmap original)
        {
                //create a blank bitmap the same size as original
                Bitmap newBitmap = new Bitmap(original.Width, original.Height);

                //get a graphics object from the new image
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newBitmap);

                //create the grayscale ColorMatrix
                ColorMatrix colorMatrix = new ColorMatrix(
                   new float[][] 
                  {
                     new float[] {.3f, .3f, .3f, 0, 0},
                     new float[] {.59f, .59f, .59f, 0, 0},
                     new float[] {.11f, .11f, .11f, 0, 0},
                     new float[] {0, 0, 0, 1, 0},
                     new float[] {0, 0, 0, 0, 1}
                  });

                //create some image attributes
                ImageAttributes attributes = new ImageAttributes();

                //set the color matrix attribute
                attributes.SetColorMatrix(colorMatrix);

                //draw the original image on the new image
                //using the grayscale color matrix
                g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                   0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

                //dispose the Graphics object
                g.Dispose();
                return newBitmap;
        }


        public class Circle
        {
            public string Coordinates {get; set;}
            public int ShadingOpacity { get; set; }
            public Color ShadingColor { get; set; }
            public int Width { get; set; }
            public int Index { get; set; }
        }
    }
}
