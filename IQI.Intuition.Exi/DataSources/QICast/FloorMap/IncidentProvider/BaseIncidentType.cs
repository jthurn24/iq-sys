using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Reporting.Models;
using RedArrow.Framework.Extensions.Common;

namespace IQI.Intuition.Exi.DataSources.QICast.FloorMap.IncidentProvider
{
    public abstract class BaseIncidentType : Interfaces.ILayerProvider
    {
        protected IEnumerable<Reporting.Models.Cubes.FloorMapRoomIncidentType.EntityEntry> CubeRecords { get; set;}

        protected IDimensionRepository DimensionRepository { get; set;}
        protected ICubeRepository CubeRepository { get; set;}

        public BaseIncidentType(IDimensionRepository dimensionRepository,
            ICubeRepository cubeRepository)
        {
            DimensionRepository = dimensionRepository;
            CubeRepository = cubeRepository;
        }

        public Domain.Enumerations.KnownProductType ProductType
        {
            get { return Domain.Enumerations.KnownProductType.IncidentTracking; }
        }

        public void Initialize(Reporting.Models.Dimensions.FloorMap map)
        {
            var records = CubeRepository
             .GetFloorMapRoomIncidentTypeByDateRange(map.Id, DateTime.Today.AddDays(-30), DateTime.Today.AddDays(1));
            CubeRecords = Filter(records).ToList();
        }

        protected abstract IEnumerable<Reporting.Models.Cubes.FloorMapRoomIncidentType.EntityEntry> Filter(IEnumerable<Reporting.Models.Cubes.FloorMapRoomIncidentType.EntityEntry> src);

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

        protected string GetRoomList()
        {
            return CubeRecords.Select(x => x.FloorMapRoom.Room.Name).Distinct().ToDelimitedString(", ");
        }

        public abstract string GetTitle();
        public abstract string GetDescription();

        public int GetListWeight()
        {
            return 1;
        }
    }
}
