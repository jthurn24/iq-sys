using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.Extensions
{
    public static class FormatExtensions
    {
        public static string TwoDecimalPoints(this double src)
        {
            return String.Format("{0:F2}%", src);
        }

        public static string TwoDecimalPoints(this decimal src)
        {
            return String.Format("{0:F2}%", src);
        }
    }
}
