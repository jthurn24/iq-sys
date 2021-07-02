using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using RedArrow.Framework.Extensions.Common;

namespace IQI.Intuition.Reporting.Graphics
{
    public class PieChart
    {
        private const int DEFAULT_WIDTH = 450;
        private const int DEFAULT_HEIGHT = 200;

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
        
        private List<Item> _items;

        public PieChart()
        {
            _items = new List<Item>();
        }

        public static MemoryStream GenerateImage(string data, int? width, int? height)
        {

            Chart chart;
            Series series;
            ChartArea area;

            chart = new Chart();
            chart.ImageType = ChartImageType.Jpeg;
            chart.Compression = 0;
            chart.BackColor = Color.White;
            chart.Width = Unit.Pixel(width.HasValue ? width.Value : DEFAULT_WIDTH );
            chart.Height = Unit.Pixel(height.HasValue ? height.Value : DEFAULT_HEIGHT);
            chart.AntiAliasing = AntiAliasingStyles.All;


            Legend legend = new Legend();
            legend.LegendStyle = LegendStyle.Column;
            legend.Docking = Docking.Right;
            legend.Alignment = StringAlignment.Center;

            chart.Legends.Add(legend);

            if (data.IsNullOrEmpty())
            {
                var mss = new MemoryStream();
                chart.SaveImage(mss);
                mss.Seek(0, SeekOrigin.Begin);
                return mss;
            }

            /* Decode Data */

            var items = new List<Item>();

            foreach (string segment in data.Split(":".ToCharArray()))
            {

                var item = new Item()
                {
                    Value = Convert.ToDouble(segment.Split("|".ToCharArray())[0]),
                    Label = segment.Split("|".ToCharArray())[1],
                    Marker = segment.Split("|".ToCharArray())[2],
                    Color = Color.FromArgb(Convert.ToInt32(segment.Split("|".ToCharArray())[3]))
                };

                items.Add(item);
            }


            series = new Series("Series1");
            series.ChartArea = "ca1";
            series.ChartType = SeriesChartType.Pie;
            series.CustomProperties = "PieDrawingStyle=SoftEdge";
            series["PieLabelStyle"] = "None";
            chart.Series.Add(series);

            area = new ChartArea("ca1");
            area.BackColor = Color.Transparent;
            chart.ChartAreas.Add(area);


            foreach (var item in items)
            {

                
                series.Points.Add(new DataPoint
                {
                    AxisLabel = item.Marker,
                    LegendText = string.Concat(item.Label),
                    YValues = new double[] { item.Value },
                    Color = item.Color,
                    BorderColor = Color.White,
                    BorderDashStyle = ChartDashStyle.Solid,
                    BorderWidth = 2,
                });

            }

            var ms = new MemoryStream();
            chart.SaveImage(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;

        }

        public static Color GetDefaultColor(int index)
        {
            if (index >= _colorSet.Length)
            {
                index = _colorSet.Length -1;
            }

            return _colorSet[index];
        }

        public void AddItem(Item item)
        {
            _items.Add(item);
        }

        public bool HasData()
        {
            if (_items.Count < 1)
            {
                return false;
            }

            if (_items.Sum(x => x.Value) == 0)
            {
                return false;
            }

            return true;
        }

        public string SerializeForRender()
        {
            var builder = new StringBuilder();

            bool first = true;
            foreach (var item in _items)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    builder.Append(":");
                }

                builder.Append(item.Value);
                builder.Append("|");
                builder.Append(item.Label);
                builder.Append("|");
                builder.Append(item.Marker);
                builder.Append("|");
                builder.Append(item.Color.ToArgb());
            }

            return builder.ToString();
        }

        public class Item
        {
            public double Value { get; set; }
            public string Label { get; set; }
            public string Marker { get; set; }
            public Color Color { get; set; }
        }

    }
}
