using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Exi.Models.QICast
{
    public class Statistic
    {
        public enum BadgeType
        {
            Normal,
            Up,
            Down
        }

        public Domain.Enumerations.KnownProductType ProductType { get; set; }
        public BadgeType Badge { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
    }
}
