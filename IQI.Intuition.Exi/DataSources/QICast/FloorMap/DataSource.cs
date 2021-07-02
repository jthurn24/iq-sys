using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.DataSource;
using SnyderIS.sCore.Exi.Implementation.DataSource;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using StructureMap;
using StructureMap.Query;
using IQI.Intuition.Reporting.Repositories;

namespace IQI.Intuition.Exi.DataSources.QICast.FloorMap
{
    public class DataSource : IDataSource<Models.QICast.FloorMapView>
    {
        private ISystemRepository _SystemRepository;
        private IContainer _Container;
        private IDimensionRepository _DimensionRepository;

        public const string FLOORMAP_GUID_KEY = "FLOORMAP_GUID_KEY";

        public DataSource(ISystemRepository systemRepository,
            IContainer container,
            IDimensionRepository dimensionRepository)
        {
            _SystemRepository = systemRepository;
            _Container = container;
            _DimensionRepository = dimensionRepository;

        }

        public IDataSourceResult<Models.QICast.FloorMapView> GetResult(IDictionary<string, string> criteria)
        {
            var resultWrapper = new SnyderIS.sCore.Exi.Implementation.DataSource.DataSourceResult<Models.QICast.FloorMapView>();
            var result = new Models.QICast.FloorMapView();

            result.FloorMap = new Reporting.Graphics.SmartFloorMap(new Guid(criteria[FLOORMAP_GUID_KEY]));
            result.FloorMap.DarkMode = true;

            var floorMap = _DimensionRepository.GetFloorMap(new Guid(criteria[FLOORMAP_GUID_KEY]));

            var groups = new List<Models.QICast.FloorMapView.Group>();
            result.Groups = groups;

            var type = typeof(Interfaces.ILayerProvider);
            var types = System.Reflection.Assembly.GetAssembly(typeof(DataSource)).GetTypes()
                .Where(p => type.IsAssignableFrom(p));

            foreach (var r in types)
            {
                if (r.IsAbstract == false)
                {
                    ProcessProvider(groups,
                        r,
                        floorMap,
                        result.FloorMap);

                }
            }

            resultWrapper.Metrics = new List<Models.QICast.FloorMapView>() { result };
            return resultWrapper;
        }

        public void ProcessProvider(List<Models.QICast.FloorMapView.Group> groups,
            Type provider, 
            Reporting.Models.Dimensions.FloorMap map,
            Reporting.Graphics.SmartFloorMap smartMap
          )
        {
            var providerInstance = (Interfaces.ILayerProvider)_Container.GetInstance(provider);
            providerInstance.Initialize(map);

            var group = groups.Where(x => x.ProductType == providerInstance.ProductType).FirstOrDefault();

            if (group == null)
            {
                group = new Models.QICast.FloorMapView.Group();
                group.Layers = new List<Models.QICast.FloorMapView.Layer>();
                group.ProductType = providerInstance.ProductType;
                groups.Add(group);
            }

            var layer = providerInstance.GetLayer();
            smartMap.Layers.Add(layer);

            group.Layers.Add(new Models.QICast.FloorMapView.Layer()
            {
                 Description = providerInstance.GetDescription(),
                 Title = providerInstance.GetTitle(),
                 LayerId = layer.LayerId,
                 Count = layer.Icons.Count() 
            });
        }


        public string DefaultTitle(IDictionary<string, string> criteria)
        {
            return string.Concat("Floor Map");
        }

        IDataSourceResult IDataSource.GetResult(IDictionary<string, string> criteria)
        {
            return GetResult(criteria);
        }
    }
}
