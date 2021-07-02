using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Infrastructure.Services.BusinessLogic.Psychotropic.FrequencyDefinitions
{
    public class TwicePerDay: BaseDefinitions.MultiplePerDay
    {
        public TwicePerDay()
            : base(2)
        {
        }
    }
}
