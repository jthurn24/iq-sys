using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Infrastructure.Services.BusinessLogic.FacilityTimeLine.EventSource
{
    public abstract class BaseState
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool Is<T>()
        {
            return (this.GetType() == typeof(T));
        }
    }
}
