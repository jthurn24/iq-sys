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
using IQI.Intuition.Reporting.Models.Dimensions;
using IQI.Intuition.Domain.Utilities;

namespace IQI.Intuition.Infrastructure.Services.Migration
{
    public class TestReports
    {
        //private IStatelessDataContext _DataContext;
        //private IDocumentStore _Store;
        //private string _Prefix;

        //public TestReports(IStatelessDataContext dataContext,
        //    IDocumentStore store)
        //{
        //    _DataContext = dataContext;
        //    _Store = store;
        //}

        //public void Run(string[] args)
        //{
        //    var quarters = _Store.GetQueryable<Quarter>()
        //        .OrderByDescending(x => x.Year).ThenByDescending(x => x.QuarterOfYear)
        //        .Take(4);

        //    var account = _DataContext.CreateQuery<Domain.Models.Account>().FetchFirst();


        //    var request = new Domain.Models.ExportRequest(account, ExportRequestFormat.Pdf);
        //    request.Status = ExportRequestStatus.New;
        //    request.EmailTo = "mark@iqisystems.com";
        //    _DataContext.Insert(request);

        //    var paths = new List<Domain.Models.ExportRequestPath>();

        //    foreach (var q in quarters)
        //    {
        //        Add(paths, q, args[1]);
        //    }

        //    foreach (var p in paths)
        //    {
        //        p.ExportRequest = request;
        //        _DataContext.Insert(p);
        //    }

        //    request.Status = ExportRequestStatus.Pending;
        //    _DataContext.Update(request);
           
        //}

        //private void Add(IList<Domain.Models.ExportRequestPath> paths, Quarter q, string prefix)
        //{
        //    var user = _DataContext.CreateQuery<AccountUser>().FetchAll()
        //        .Where(x => x.Login.ToLower() == "test").First();

        //    var facility = _Store.GetQueryable<IQI.Intuition.Reporting.Models.Dimensions.Facility>()
        //        .Where(x => x.Name == "Facility #1").First();


        //    _Prefix = prefix;

        //    Add(paths, "/Reporting/Facility/QuarterlyInfectionControlReport?Quarter={0}", user, true, q);
        //    Add(paths, "/Reporting/Facility/QuarterlyInfectionTrendReport?Quarter={0}", user, true, q);
        //    Add(paths, "/Reporting/Facility/QuarterlyInfectionByTypeReport?Quarter={0}", user, true, q);

        //    foreach (var t in _Store.GetQueryable<IQI.Intuition.Reporting.Models.Cubes.FacilityInfectionSite>().ToList()
        //        .Where(x => x.Facility.Id == facility.Id)
        //        .SelectMany(x => x.Entries).Select(x => x.InfectionType).Distinct())
        //    {
        //        Add(paths,
        //        string.Concat("/Reporting/Facility/QuarterlyInfectionBySiteReport?Quarter={0}&InfectionType=", t.Id)
        //        , user, true, q);
        //    }

        //    Add(paths, "/Reporting/Facility/QuarterlyInfectionByFloorReport?Quarter={0}", user, true, q);
        //    Add(paths, "/Reporting/Facility/QuarterlyInfectionByWingReport?Quarter={0}", user, true, q);

        //    Add(paths, "/Reporting/Facility/QuarterlySCAUTIReport?Quarter={0}", user, true, q);
        //    Add(paths, "/Reporting/Facility/QuarterlyComplaintControlReport?Quarter={0}", user, true, q);
        //    Add(paths, "/Reporting/Facility/QuarterlyComplaintTrendReport?Quarter={0}", user, true, q);
        //    Add(paths, "/Reporting/Facility/QuarterlyComplaintByWingReport?Quarter={0}", user, true, q);
        //    Add(paths, "/Reporting/Facility/QuarterlyComplaintByFloorReport?Quarter={0}", user, true, q);
        //    Add(paths, "/Reporting/Facility/QuarterlyIncidentControlReport?Quarter={0}", user, true, q);
            
        //    foreach(var t in _Store.GetQueryable<IncidentTypeGroup>().ToList())
        //    {
        //        Add(paths, 
        //            string.Concat("/Reporting/Facility/QuarterlyIncidentByTypeReport?Quarter={0}&IncidentTypeGroup=",t.Id)
        //            , user, true, q);

        //        Add(paths,
        //            string.Concat("/Reporting/Facility/QuarterlyIncidentByInjuryReport?Quarter={0}&IncidentTypeGroup=", t.Id)
        //            , user, true, q);

        //        Add(paths,
        //            string.Concat("/Reporting/Facility/QuarterlyIncidentByInjuryLevelReport?Quarter={0}&IncidentTypeGroup=", t.Id)
        //            , user, true, q);

        //        Add(paths,
        //            string.Concat("/Reporting/Facility/QuarterlyIncidentByLocationReport?Quarter={0}&IncidentTypeGroup=", t.Id)
        //            , user, true, q);

        //        Add(paths,
        //            string.Concat("/Reporting/Facility/QuarterlyIncidentByTypeReport?Quarter={0}&IncidentTypeGroup=", t.Id)
        //            , user, true, q);


        //        Add(paths,
        //            string.Concat("/Reporting/Facility/QuarterlyIncidentByDayOfWeekReport?Quarter={0}&IncidentTypeGroup=", t.Id)
        //            , user, true, q);

        //        Add(paths,
        //            string.Concat("/Reporting/Facility/QuarterlyIncidentByHourOfDayReport?Quarter={0}&IncidentTypeGroup=", t.Id)
        //            , user, true, q);

        //        Add(paths,
        //            string.Concat("/Reporting/Facility/QuarterlyIncidentByFloorReport?Quarter={0}&IncidentTypeGroup=", t.Id)
        //            , user, true, q);
        //    }

 

        //    Add(paths, "/Reporting/Facility/QuarterlyWoundControlReport?Quarter={0}&WoundType=bbd47f38-f2b0-4c79-a2e2-d0c49225b41a", user, true, q);
        //    Add(paths, "/Reporting/Facility/QuarterlyWoundByWingReport?Quarter={0}&WoundType=bbd47f38-f2b0-4c79-a2e2-d0c49225b41a", user, true, q);
        //    Add(paths, "/Reporting/Facility/QuarterlyPsychotropicControlReport?Quarter={0}", user, true, q);

        //}

        //private void Add(IList<Domain.Models.ExportRequestPath> paths, string link, AccountUser user, bool landscape, Quarter q)
        //{

        //    link = string.Format(link, q.Id);
        //    link = string.Concat(_Prefix, link);

        //    var token = new AuthenticationRequestToken(user);
        //    string argChar = "?";

        //    if (link.Contains("?"))
        //    {
        //        argChar = "&";
        //    }

        //    string finalPath = string.Concat(link, argChar, AuthenticationRequestToken.AUTHENTICATION_TOKEN_REQUEST_VAR, "=", token.ToToken(), "&export=true");
        //    paths.Add(new ExportRequestPath() { Path = finalPath, Landscape = true });
        //}

    }
}
