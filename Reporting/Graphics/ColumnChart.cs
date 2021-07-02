﻿using System;
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
    public class ColumnChart
    {
        private const int DEFAULT_WIDTH = 850;
        private const int DEFAULT_HEIGHT = 250;

        static Color[] _colorSet = new Color[9] { 
               System.Drawing.ColorTranslator.FromHtml("#4572A7"),
                System.Drawing.ColorTranslator.FromHtml("#AA4643"),
                System.Drawing.ColorTranslator.FromHtml("#89A54E"),
                System.Drawing.ColorTranslator.FromHtml("#80699B"),
                System.Drawing.ColorTranslator.FromHtml("#3D96AE"),
                System.Drawing.ColorTranslator.FromHtml("#DB843D"),
                System.Drawing.ColorTranslator.FromHtml("#92A8CD"),
                System.Drawing.ColorTranslator.FromHtml("#A47D7C"),
                System.Drawing.ColorTranslator.FromHtml("#B5CA92")
            };


        private List<Item> _items;

        public int MinimumYaxis { get; set; }

        public ColumnChart()
        {
            _items = new List<Item>();
        }

        public static MemoryStream GenerateImage(string data, string options, int? width, int? height)
        {
            Chart chart;
            ChartArea area;
            int colorCounter = -1;

            Dictionary<string, string> optionList = new Dictionary<string, string>();

            chart = new Chart();
            chart.ImageType = ChartImageType.Jpeg;
            chart.Compression = 0;
            chart.BackColor = Color.White;
            chart.Width = Unit.Pixel(width.HasValue ? width.Value : DEFAULT_WIDTH);
            chart.Height = Unit.Pixel(height.HasValue ? height.Value : DEFAULT_HEIGHT);
            chart.AntiAliasing = AntiAliasingStyles.All;
            chart.TextAntiAliasingQuality = TextAntiAliasingQuality.Normal;

            chart.RenderType = RenderType.ImageTag;

            area = new ChartArea("ca1");
            area.BackColor = Color.Transparent;

            chart.ChartAreas.Add(area);

            if (data.IsNullOrEmpty())
            {
                var mss = new MemoryStream();
                chart.SaveImage(mss);
                mss.Seek(0, SeekOrigin.Begin);
                return mss;
            }

            /* decode options */
            foreach (string option in options.Split(":".ToCharArray()))
            {
                var s = option.Split("|".ToCharArray());
                optionList.Add(s[0], s[1]);
            }

            /* Decode Data */

            var items = new List<Item>();

            foreach (string segment in data.Split(":".ToCharArray()))
            {

                var item = new Item()
                {
                    Value = Convert.ToDouble(segment.Split("|".ToCharArray())[0]),
                    Category = segment.Split("|".ToCharArray())[1],
                    Color = Color.FromArgb(Convert.ToInt32(segment.Split("|".ToCharArray())[2]))
                };

                items.Add(item);
            }

            /* Build X value lookups (we have to do this since a custom label is required for x values that are strings */

            var xValues = new Dictionary<string, double>();
            double xValuesCounter = 0;

            area.AxisX.LabelStyle.IsEndLabelVisible = false;
            area.AxisX.IsLabelAutoFit = true;
            area.AxisX.LabelStyle.Angle = 30;
            area.InnerPlotPosition.Width = 95;
            area.InnerPlotPosition.Height = 65;
            area.InnerPlotPosition.X = 5;


            var minimumYaxis = Convert.ToInt32(optionList["MinimumYaxis"]);
            if (minimumYaxis > 0)
            {
                if (items.Count() < 0)
                {
                    area.AxisY.Maximum = minimumYaxis;
                }
                else
                {
                    if (items.Max(x => x.Value) < 10)
                    {
                        area.AxisY.Maximum = minimumYaxis;
                    }
                }
            }

            foreach (var item in items)
            {
                if (xValues.ContainsKey(item.Category) == false)
                {
                    xValuesCounter++;
                    xValues.Add(item.Category, xValuesCounter);

                    var label = new CustomLabel()
                    {
                        Text = item.Category,
                        FromPosition = xValuesCounter - .99,
                        ToPosition = xValuesCounter + .99,
                    };

                    area.AxisX.CustomLabels.Add(label);
                }
            }

            /* build each series */

            var seriesLookup = new Dictionary<string, Series>();

            foreach (var item in items)
            {
                if (seriesLookup.ContainsKey(string.Empty) == false)
                {
                    colorCounter++;
                    if (colorCounter >= _colorSet.Length)
                    {
                        colorCounter = 0;
                    }

                    var series = new Series(string.Empty);
                    series.ChartArea = area.Name;
                    series.ChartType = SeriesChartType.Column;
                    series["DrawingStyle"] = "Cylinder";
                   

                    chart.Series.Add(series);
                    seriesLookup[string.Empty] = series;
                }
            }


            /* add data */

            foreach (var item in items)
            {
                var series = seriesLookup[string.Empty];

                series.Points.Add(new DataPoint
                {
                    XValue = xValues[item.Category],
                    YValues = new double[] { item.Value },
                    Color = item.Color
                });
            }


            foreach (var item in items)
            {
                if (xValues.ContainsKey(item.Category) == false)
                {
                    xValuesCounter++;
                    xValues.Add(item.Category, xValuesCounter);
                    area.AxisX.CustomLabels.Add(new CustomLabel()
                    {
                        Text = item.Category,
                        FromPosition = xValuesCounter - .01,
                        ToPosition = xValuesCounter + .01
                    });
                }
            }

            /* dump to stream */


            var ms = new MemoryStream();
            chart.SaveImage(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        public static Color GetDefaultColor(int index)
        {
            if (index >= _colorSet.Length)
            {
                return GetDefaultColor(index - _colorSet.Length);
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

        public string SerializeDataForRender()
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
                builder.Append(item.Category);
                builder.Append("|");
                builder.Append(item.Color.ToArgb());
            }

            return builder.ToString();
        }

        public string SerializeOptionsForRender()
        {
            var builder = new StringBuilder();

            builder.Append("MinimumYaxis");
            builder.Append("|");
            builder.Append(this.MinimumYaxis);

            return builder.ToString();
        }

        public class Item
        {
            public double Value { get; set; }
            public string Category { get; set; }
            public Color Color { get; set; }
        }


    }
}