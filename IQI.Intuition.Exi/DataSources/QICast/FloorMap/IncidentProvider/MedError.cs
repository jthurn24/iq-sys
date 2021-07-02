using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Reporting.Models;

namespace IQI.Intuition.Exi.DataSources.QICast.FloorMap.IncidentProvider
{
    public class MedError : BaseIncidentType
    {
        public MedError(IDimensionRepository d,
            ICubeRepository c) : base(d,c)
        {}

        protected override IEnumerable<Reporting.Models.Cubes.FloorMapRoomIncidentType.EntityEntry> Filter(IEnumerable<Reporting.Models.Cubes.FloorMapRoomIncidentType.EntityEntry> src)
        {
            return src.Where(x => x.IncidentTypeGroups.Select(xx => xx.Name).Contains("Med Error"));
        }

        public override string GetTitle()
        {
            return "Med Error";
        }

        public override string GetDescription()
        {
            return string.Concat("Med Error related incidents in last 14 days rooms: ", GetRoomList());
        }
    }
}
