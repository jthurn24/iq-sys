using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Reporting.Models;

namespace IQI.Intuition.Exi.DataSources.QICast.FloorMap.WoundProvider
{
    public class Stage2 : BaseWoundStage
    {

        public Stage2(IDimensionRepository d,
            ICubeRepository c) : base(d,c)
        {}

        protected override IEnumerable<Reporting.Models.Cubes.FloorMapRoomWoundStage.EntityEntry> Filter(IEnumerable<Reporting.Models.Cubes.FloorMapRoomWoundStage.EntityEntry> src)
        {
            return src.Where(x => x.WoundStage.Name == "Stage II");
        }

        public override string GetTitle()
        {
            return "Stage II";
        }

        public override string GetDescription()
        {
            return string.Concat("Active Stage 2 pressure ulcers rooms: ", GetRoomList());
        }

        public override int GetListWeight()
        {
            return 2;
        }
    }
}
