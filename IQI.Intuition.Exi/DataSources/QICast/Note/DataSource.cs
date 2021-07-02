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

namespace IQI.Intuition.Exi.DataSources.QICast.Note
{
    public class DataSource : IDataSource<Models.QICast.Note>
    {

        private IContainer _Container;
        private IUserRepository _UserRepository;
        private IFacilityRepository _FacilityRepository;

        public const string NOTE_GUID_KEY = "NOTE_GUID_KEY";
        public const string FACILITY_GUID_KEY = "FACILITY_GUID_KEY";
        public const string BG_STYLE_KEY = "NOTE_BG_KEY";

        public DataSource(ISystemRepository systemRepository,
            IContainer container,
            IUserRepository userRepository,
            IFacilityRepository facilityRepository)
        {
            _Container = container;
            _UserRepository = userRepository;
            _FacilityRepository = facilityRepository;
        }

        public IDataSourceResult<Models.QICast.Note> GetResult(IDictionary<string, string> criteria)
        {
            var resultWrapper = new SnyderIS.sCore.Exi.Implementation.DataSource.DataSourceResult<Models.QICast.Note>();
            var result = new Models.QICast.Note();

            /* Do not do work if this is preview */
            if (criteria.ContainsKey("slotid"))
            {
                var facilityId = new Guid(criteria[FACILITY_GUID_KEY]);
                var noteId = new Guid(criteria["slotid"]);


                var note = _UserRepository.GetOrCreateNote(noteId, facilityId);

                result.Content = note.Content;
                result.HasImage = note.Image != null;
                result.Id = note.Id;
                result.BackgroundStyle = criteria[BG_STYLE_KEY];
            }

            resultWrapper.Metrics = new List<Models.QICast.Note>() { result };
            return resultWrapper;
        }


        public string DefaultTitle(IDictionary<string, string> criteria)
        {
            return string.Concat("Complaint Graph");
        }

        IDataSourceResult IDataSource.GetResult(IDictionary<string, string> criteria)
        {
            return GetResult(criteria);
        }


    }
}
