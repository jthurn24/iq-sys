using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using SnyderIS.sCore.Persistence;

namespace IQI.Intuition.Reporting.Graphics
{
    public class SmartFloorMap
    {
        private List<Layer> _Layers;
        private Guid _FloorMapBackgroundGuid;
        private bool _DarkMode;
        private decimal _ResizeMultipler = 1;
        private bool _AutoRotate;


        public SmartFloorMap(Guid backgroundGuid)
        {
            _FloorMapBackgroundGuid = backgroundGuid;
        }

        public bool DarkMode
        {
            get
            {
                return _DarkMode;
            }
            set
            {
                _DarkMode = value;
            }
        }

        public decimal ResizeMultipler
        {
            get
            {
                return _ResizeMultipler;
            }
            set
            {
                _ResizeMultipler = value;
            }
        }

        public bool AutoRotate
        {
            get
            {
                return _AutoRotate;
            }
            set
            {
                _AutoRotate = value;
            }
        }

        public List<Layer> Layers
        {
            get
            {
                if (_Layers == null)
                {
                    _Layers = new List<Layer>();
                }

                return _Layers;
            }
        }


        public string SerializeForRender()
        {
            var builder = new StringBuilder();

            builder.Append(_FloorMapBackgroundGuid);

            builder.Append(",");

            builder.Append(DarkMode);

            builder.Append(",");

            builder.Append(ResizeMultipler);

            return builder.ToString();
        }

        public static MemoryStream GenerateImage(string data, IDocumentStore store)
        {

            string guid = data.Split(',')[0];
            bool darkMode = bool.Parse(data.Split(',')[1]);

            decimal resizeMultiplier = decimal.Parse(data.Split(',')[2]);

           
            var imageDocument = store.GetQueryable<Models.Dimensions.FloorMapImage>()
                .Where(x => x.FloorMap.Id == Guid.Parse(guid))
                .First();

            MemoryStream ms = new MemoryStream(imageDocument.Image);
            var background= Image.FromStream(ms);

            var img = new Bitmap(background.Width, background.Height);

            var graphics = System.Drawing.Graphics.FromImage(img);
            graphics.DrawImage(background, 0, 0, background.Width, background.Height);


            if (darkMode)
            {
                Color pixelColor;
                byte A, R, G, B;

                for (int y = 0; y < img.Height; y++)
                {

                    for (int x = 0; x < img.Width; x++)
                    {
                        pixelColor = img.GetPixel(x, y);

                        if (pixelColor.R > 240 && pixelColor.G > 240 && pixelColor.B > 240)
                        {
                            pixelColor = Color.Black;
                        }
                        else
                        {
                            pixelColor = Color.Gray;
                        }




                        img.SetPixel(x, y, pixelColor);

                    }
                }

                img.MakeTransparent(Color.Black);
            }

            if (resizeMultiplier < 1)
            {
                img = new Bitmap(img,
                    (int)((decimal)img.Width * resizeMultiplier),
                    (int)((decimal)img.Height * resizeMultiplier));
            }


            var mms = new System.IO.MemoryStream();
            img.Save(mms, ImageFormat.Png);
            mms.Seek(0, SeekOrigin.Begin);
            return mms;
        }

        public class Layer
        {
            public IList<Icon> Icons { get; set; }
            public Guid LayerId { get; set; }

            public Layer()
            {
                Icons = new List<Icon>();
            }

        }

        public class Icon
        {
            public string Coordinates { get; set; }
            public int Count { get; set; }

            public int X
            {
                get
                {
                    return Convert.ToInt32(this.Coordinates.Split(',')[0]);
                }
            }

            public int Y
            {
                get
                {
                    return Convert.ToInt32(this.Coordinates.Split(',')[1]);
                }
            }
        }


    }
}
