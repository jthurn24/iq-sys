using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.DataSource;
using SnyderIS.sCore.Exi.Implementation.DataSource;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Exi.Models.QICast;
using StructureMap;
using StructureMap.Query;
using System.Web.Mvc;
using RedArrow.Framework.Extensions.Formatting;

namespace IQI.Intuition.Exi.DataSources.QICast.Statistics
{
    public class DataSource : IDataSource<Statistic>
    {

        private static List<KeyValuePair<string,string>> _Providers;

        private IContainer _Container;
        public const string STAT_PROVIDER_TYPE_NAME = "STAT_PROVIDER_TYPE_NAME";
        public const string FACILITY_GUID_KEY = "FACILITY_GUID_KEY";

        public DataSource(IContainer container)
        {
            _Container = container;
        }

        public IDataSourceResult<Statistic> GetResult(IDictionary<string, string> criteria)
        {
            var result = new DataSourceResult<Statistic>();

            var sourceTypeName = criteria[STAT_PROVIDER_TYPE_NAME];

            var provider = _Container.GetInstance(Type.GetType(sourceTypeName));

            var stat = ((Interfaces.IStatProvider)provider).GetResult(new Guid(criteria[FACILITY_GUID_KEY]));

            result.Metrics = new List<Statistic>(new[] { stat });

            return result;
        }

        public string DefaultTitle(IDictionary<string, string> criteria)
        {
            return string.Concat("Statistic");
        }

        IDataSourceResult IDataSource.GetResult(IDictionary<string, string> criteria)
        {
            return GetResult(criteria);
        }

        public static List<KeyValuePair<string,string>> GetAvailableProviders()
        {
            if (_Providers == null)
            {
                _Providers = new List<KeyValuePair<string,string>>();

                var type = typeof(Interfaces.IStatProvider);
                var types = System.Reflection.Assembly.GetAssembly(typeof(DataSource)).GetTypes()
                    .Where(p => type.IsAssignableFrom(p));


                foreach (var r in types)
                {
                    if (r.IsInterface == false && r.IsAbstract == false)
                    {
                        _Providers.Add(
                            new KeyValuePair<string, string>(
                                r.FullName,
                                string.Concat(r.Namespace.Split('.').Last().Replace("Provider", ""), "-", 
                                r.Name.SplitPascalCase()))
                                );
                    }
                }
            }

            return _Providers;
        }
    }
}
