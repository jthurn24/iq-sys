using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RedArrow.Framework.Extensions.Common;

namespace IQI.Intuition.Reporting.Models.Dimensions
{
    public class WoundSite : BaseReportingEntity
    {
        public virtual string Name { get; set; }
        public virtual string Coordinates { get; set; }


        public virtual bool HasCoordinates(int tlX, int tlY, int brX, int brY)
        {
            foreach (var rect in GetRectangles())
            {
                if (rect.TopLeftX == tlX &&
                    rect.TopLeftY == tlY &&
                    rect.BottomRightX == brX &&
                    rect.BottomRightY == brY)
                {
                    return true;
                }
            }
            
            return false;
        }

        public virtual void AddCoordinates(int tlX, int tlY, int brX, int brY)
        {
            var current = GetRectangles().ToList();

            current.Add(new Graphics.Helpers.Rectangle()
            {
                 TopLeftX = tlX,
                 TopLeftY = tlY,
                 BottomRightX = brX,
                 BottomRightY = brY
            });

            var tokenBuilder = new StringBuilder();

            foreach (var e in current)
            {
                if (tokenBuilder.ToString() != string.Empty)
                {
                    tokenBuilder.Append("|");
                }

                tokenBuilder.AppendFormat("{0},{1},{2},{3}",
                    e.TopLeftY,
                    e.TopLeftX,
                    e.BottomRightY,
                    e.BottomRightX);
            }

            this.Coordinates = tokenBuilder.ToString();
        }

        public virtual IEnumerable<Reporting.Graphics.Helpers.Rectangle> GetRectangles()
        {
            var result = new List<Reporting.Graphics.Helpers.Rectangle>();

            if (this.Coordinates.IsNotNullOrEmpty())
            {
                var lines = this.Coordinates.Split('|');

                foreach (var line in lines)
                {
                    var peices = line.Split(',');

                    var r = new Reporting.Graphics.Helpers.Rectangle();
                    r.TopLeftY = Convert.ToInt32(peices[0]);
                    r.TopLeftX = Convert.ToInt32(peices[1]);
                    r.BottomRightY = Convert.ToInt32(peices[2]);
                    r.BottomRightX = Convert.ToInt32(peices[3]);

                    result.Add(r);
                }

            }

            return result;
        }

    }
}
