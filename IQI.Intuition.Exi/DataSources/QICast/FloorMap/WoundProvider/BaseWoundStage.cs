using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Reporting.Models;
using RedArrow.Framework.Extensions.Common;

namespace IQI.Intuition.Exi.DataSources.QICast.FloorMap.WoundProvider
{
    public abstract class BaseWoundStage: Interfaces.ILayerProvider
    {
        protected IEnumerable<Reporting.Models.Cubes.FloorMapRoomWoundStage.EntityEntry> CubeRecords { get; set;}

        protected IDimensionRepository DimensionRepository { get; set;}
        protected ICubeRepository CubeRepository { get; set;}

        public BaseWoundStage(IDimensionRepository dimensionRepository,
            ICubeRepository cubeRepository)
        {
            DimensionRepository = dimensionRepository;
            CubeRepository = cubeRepository;
        }

        public Domain.Enumerations.KnownProductType ProductType
        {
            get { return Domain.Enumerations.KnownProductType.WoundTracking; }
        }

        public void Initialize(Reporting.Models.Dimensions.FloorMap map)
        {
            var records = CubeRepository
            .GetFloorMapRoomWoundStageByDateRange(map.Id, DateTime.Today, DateTime.Today.AddDays(1));
            CubeRecords = Filter(records);
        }

        protected abstract IEnumerable<Reporting.Models.Cubes.FloorMapRoomWoundStage.EntityEntry> Filter(IEnumerable<Reporting.Models.Cubes.FloorMapRoomWoundStage.EntityEntry> src);

        public Reporting.Graphics.SmartFloorMap.Layer GetLayer()
        {
            var layer = new Reporting.Graphics.SmartFloorMap.Layer();
            layer.LayerId = RedArrow.Framework.Utilities.GuidHelper.NewGuid();
            layer.Icons = new List<Reporting.Graphics.SmartFloorMap.Icon>();

            foreach (var i in CubeRecords)
            {
                var icon = layer.Icons.Where(x => x.Coordinates == i.FloorMapRoom.Coordinates).FirstOrDefault();

                if (icon == null)
                {
                    icon = new Reporting.Graphics.SmartFloorMap.Icon();
                    icon.Coordinates = i.FloorMapRoom.Coordinates;
                    layer.Icons.Add(icon);
                }

                icon.Count++;

            }

            return layer;
        }

        public abstract string GetTitle();
        public abstract string GetDescription();

        protected string GetRoomList()
        {
            return CubeRecords.Select(x => x.FloorMapRoom.Room.Name).Distinct().ToDelimitedString(',');
        }

        public virtual int GetListWeight()
        {
            return 1;
        }
    }
}
