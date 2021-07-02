using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Reporting.Models.Dimensions;
using IQI.Intuition.Reporting.Models.Facts;
using IQI.Intuition.Reporting.Repositories;
using RedArrow.Framework.Utilities;
using SnyderIS.sCore.Persistence;

namespace IQI.Intuition.Infrastructure.Persistence.Repositories.Reporting
{
    public class FactBuilderRepository : ReportingRepository, IFactBuilderRepository
    {
        public FactBuilderRepository(IDocumentStore store)
            : base(store)
        { }

        public InfectionVerification GetOrCreateInfectionVerification(Guid guid)
        {
            var result = GetQueryable<InfectionVerification>()
                           .Where(src => src.Id == guid)
                           .FirstOrDefault();

            if (result == null)
            {
                result = new InfectionVerification()
                {
                    Id = guid
                };

                _Store.Save<InfectionVerification>(result);
            }

            return result;
        }


        public IncidentReport GetOrCreateIncidentReport(Guid guid)
        {
            var result = GetQueryable<IncidentReport>()
                           .Where(src => src.Id == guid)
                           .FirstOrDefault();

            if (result == null)
            {
                result = new IncidentReport()
                {
                    Id = guid,
                    IncidentTypeGroups = new List<IncidentTypeGroup>(),
                    IncidentTypes = new List<IncidentType>(),
                    IncidentInjuries =  new List<IncidentInjury>()
                };

                _Store.Save<IncidentReport>(result);
            }

            return result;
        }

        public PsychotropicAdministration GetOrCreatePsychotropicAdministration(Guid guid)
        {
            var result = GetQueryable<PsychotropicAdministration>()
               .Where(src => src.Id == guid)
               .FirstOrDefault();

            if (result == null)
            {
                result = new PsychotropicAdministration()
                {
                    Id = guid,
                    AdministrationMonths = new List<PsychotropicAdministrationMonth>()
                };

                _Store.Save<PsychotropicAdministration>(result);
            }

            return result;
        }

        public WoundReport GetOrCreateWoundReport(Guid guid)
        {
            var result = GetQueryable<WoundReport>()
                           .Where(src => src.Id == guid)
                           .FirstOrDefault();

            if (result == null)
            {
                result = new WoundReport()
                {
                    Id = guid,
                    Assessments = new List<WoundReport.Assessment>()
                };

                _Store.Save<WoundReport>(result);
            }

            return result;
        }

        public Complaint GetOrCreateComplaint(Guid guid)
        {
            var result = GetQueryable<Complaint>()
                           .Where(src => src.Id == guid)
                           .FirstOrDefault();

            if (result == null)
            {
                result = new Complaint()
                {
                    Id = guid
                };

                _Store.Save<Complaint>(result);
            }

            return result;
        }


        public Catheter GetOrCreateCatheter(Guid guid)
        {
            var result = GetQueryable<Catheter>()
                           .Where(src => src.Id == guid)
                           .FirstOrDefault();

            if (result == null)
            {
                result = new Catheter()
                {
                    Id = guid
                };

                _Store.Save<Catheter>(result);
            }

            return result;
        }

    }
}
