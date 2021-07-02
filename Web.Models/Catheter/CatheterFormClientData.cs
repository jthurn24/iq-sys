using System;
using System.Collections;

namespace IQI.Intuition.Web.Models.Catheter
{
    public class CatheterFormClientData
    {
        public IEnumerable Floors { get; set; }

        public IEnumerable Wings { get; set; }

        public IEnumerable Rooms { get; set; }
    }
}
