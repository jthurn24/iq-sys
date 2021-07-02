using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using SnyderIS.sCore.Persistence;
using RedArrow.Framework.Extensions.Common;

namespace IQI.Intuition.Reporting.Graphics
{
    public class FloorMapChart
    {
        private List<Circle> _Circles;
        private Guid _FloorMapBackgroundGuid;
  
        public FloorMapChart(Guid backgroundGuid)
        {
            _FloorMapBackgroundGuid = backgroundGuid;
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

            builder.Append(_FloorMapBackgroundGuid);

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

        public static MemoryStream GenerateImage(string data, IDocumentStore store)
        {
           
 
           
            string guid = data.Substring(0, 36);
            string areaData = data.Substring(36, data.Length - 36);

            var imageDocument = store.GetQueryable<Models.Dimensions.FloorMapImage>()
                .Where(x => x.FloorMap.Id == Guid.Parse(guid))
                .First();

            MemoryStream ms = new MemoryStream(imageDocument.Image);
            var background = Image.FromStream(ms);

            var img = new Bitmap(background.Width, background.Height);
            var graphics = System.Drawing.Graphics.FromImage(img);

            /* draw background */

            graphics.DrawImage(background, 0, 0,background.Width,background.Height);

            /* break up data */

            var circles = new List<Circle>();

            if (areaData != string.Empty)
            {

                foreach (string chunk in areaData.Split(":".ToCharArray()))
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

                    foreach( var circle in circlesToDraw)
                    {
                        var point = new Point();

                        if(circle.Coordinates.IsNullOrEmpty())
                        {
                            point.X = 0;
                            point.Y = 0;
                        }
                        else
                        {
                            var pointX = circle.Coordinates.Split(",".ToCharArray())[0];
                            var pointY = circle.Coordinates.Split(",".ToCharArray())[1];
                            point.X = pointX.IsNullOrEmpty() ? 0 : Convert.ToInt32(pointX);
                            point.Y = pointY.IsNullOrEmpty() ? 0 : Convert.ToInt32(pointY);
                        }

                        Color finalColor = Color.FromArgb(circle.ShadingOpacity, (int)circle.ShadingColor.R, (int)circle.ShadingColor.G, (int)circle.ShadingColor.B);

                        /* draw the shaded portion */
                        var brush = new SolidBrush(finalColor);
                        var shadowBrush = new SolidBrush(Color.FromArgb(100,Color.DarkGray));
                        var pinBrush = new SolidBrush(Color.Black);
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;

                        int circleX = point.X - (circle.Width / 2);
                        int circleY = point.Y - (circle.Width / 2);



                        if(pinDrawn == false)
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

            var mms = new System.IO.MemoryStream();
            img.Save(mms, ImageFormat.Jpeg);
            mms.Seek(0, SeekOrigin.Begin);
            return mms;
        }


        public class Circle
        {
            public string Coordinates {get; set;}
            public int ShadingOpacity { get; set; }
            public Color ShadingColor { get; set; }
            public int Width { get; set; }
            public int Index { get; set; }
            public string DataUrl { get; set; }
        }

    }
}
