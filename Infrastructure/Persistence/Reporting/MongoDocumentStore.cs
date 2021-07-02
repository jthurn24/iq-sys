using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;
using SnyderIS.sCore.Persistence;

namespace IQI.Intuition.Infrastructure.Persistence.Reporting
{
    public class MongoDocumentStore : IDocumentStore
    {
        private MongoClient _Client { get;  set; }
        private MongoDatabase _Database { get;  set; }
        private MongoServer _Server { get;  set; }

        public MongoDocumentStore()
        {
            _Client = new MongoClient(System.Configuration.ConfigurationSettings.AppSettings["MongoConnection"]);
            _Server = _Client.GetServer();
            _Database = _Server.GetDatabase(System.Configuration.ConfigurationSettings.AppSettings["MongoDB"]);
            
        }

        protected MongoCollection<T> GetCollection<T>()
        {
            return _Database
                .GetCollection<T>(typeof(T).FullName.Replace("IQI.Intuition.Reporting.Models.", string.Empty));
        }

        public IQueryable<T> GetQueryable<T>()
        {
            return GetCollection<T>().AsQueryable();
        }

        public void Insert<T>(T obj)
        {
            GetCollection<T>().Insert(obj);
        }

        public void Save<T>(T obj)
        {
            GetCollection<T>().Save(obj);
        }

        public void Delete<T>(T obj, Guid id)
        {
            GetCollection<T>().Remove(Query.EQ("_id", id));
        }
    }
}
