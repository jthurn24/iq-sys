using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Reporting.Models;

namespace IQI.Intuition.Exi.DataSources.QICast.FloorMap.InfectionProvider
{
    public class Respiratory : BaseInfectionType
    {

        public Respiratory(IDimensionRepository d,
            ICubeRepository c) : base(d,c)
        {}

        protected override IEnumerable<Reporting.Models.Cubes.FloorMapRoomInfectionType.EntityEntry> Filter(IEnumerable<Reporting.Models.Cubes.FloorMapRoomInfectionType.EntityEntry> src)
        {
            return src.Where(x => x.InfectionType.ShortName == "Respiratory");
        }

        public override string GetTitle()
        {
            return "Respiratory";
        }

        public override string GetDescription()
        {
            return string.Concat("Active confirmed respiratory infections rooms: ", GetRoomList());
        }
    }
}
