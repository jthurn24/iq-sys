using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.DataSource;
using SnyderIS.sCore.Exi.Implementation.DataSource;

namespace IQI.Intuition.Exi.DataSources
{
    public class NullDataSource : IDataSource<string>
    {
        public IDataSourceResult<string> GetResult(IDictionary<string, string> criteria)
        {
            return new DataSourceResult<string>();
        }

        public string DefaultTitle(IDictionary<string, string> criteria)
        {
            return criteria["Title"];
        }

        IDataSourceResult IDataSource.GetResult(IDictionary<string, string> criteria)
        {
            return GetResult(criteria);
        }
    }
}
