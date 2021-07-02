using System;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Persistence.NHibernate;
using RedArrow.Framework.Logging;
using SnyderIS.sCore.Persistence;
using System.Collections.Generic;
using RedArrow.Framework.Utilities;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;
using System.IO;
using IQI.Intuition.Reporting.Models.Dimensions;

namespace IQI.Intuition.Infrastructure.Services.Migration
{
    public class ConvertAverages
    {

        private IStatelessDataContext _DataContext;
        private IDocumentStore _Store;


        public ConvertAverages(IStatelessDataContext dataContext,
            IDocumentStore store)
        {
            _DataContext = dataContext;
            _Store = store;
        }


        public void Run(string[] args)
        {
            ImportAverages();
        }

        public void ImportAverages()
        {
            var lookup = new Dictionary<int, Guid>();

            var command = _DataContext.CreateCommand();
            command.CommandText = "SELECT ID,[Name] FROM [Dimensions_AverageType]";
            command.CommandType = System.Data.CommandType.Text;

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                int id = Convert.ToInt32(reader[0]);
                string name = reader[1].ToString();

                var match = _Store.GetQueryable<AverageType>()
                    .Where(x => x.Name == name)
                    .FirstOrDefault();

                if (match == null)
                {
                    match = new AverageType();
                    match.Name = name;
                    _Store.Insert(match);
                }

                lookup[id] = match.Id;
            }

            reader.Close();


            command = _DataContext.CreateCommand();
            command.CommandText = "SELECT [AverageTypeId],Dimensions_Facility.Guid FROM [Dimensions_FacilityAverageType] LEFT JOIN Dimensions_Facility on Dimensions_Facility.ID = [Dimensions_FacilityAverageType].FacilityId ";
            command.CommandType = System.Data.CommandType.Text;

            reader = command.ExecuteReader();

            while (reader.Read())
            {
                int id = Convert.ToInt32(reader[0]);
                Guid fid = Guid.Parse(reader[1].ToString());
                Guid aid = lookup[id];

                var average = _Store.GetQueryable<AverageType>()
                    .Where(x => x.Id == aid).First();

                var facility = _Store.GetQueryable<IQI.Intuition.Reporting.Models.Dimensions.Facility>()
                    .Where(x => x.Id == fid).First();

                var e = new FacilityAverageType();
                e.AverageType = average;
                e.Facility = facility;
                _Store.Insert(e);
            }

            reader.Close();
            
        }

 
    }
}
