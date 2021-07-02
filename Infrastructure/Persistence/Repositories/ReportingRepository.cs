using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Persistence;

namespace IQI.Intuition.Infrastructure.Persistence.Repositories
{
    public abstract class ReportingRepository
    {
        protected IDocumentStore _Store;

        public ReportingRepository(IDocumentStore store)
        {
            _Store = store;
        }

        protected IQueryable<T> GetQueryable<T>()
        {
            return _Store.GetQueryable<T>();
        }

        protected void Save<T>(T o)
        {
            _Store.Save<T>(o);
        }
    }
}
