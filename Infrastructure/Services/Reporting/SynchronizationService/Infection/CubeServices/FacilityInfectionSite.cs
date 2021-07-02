using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Domain.Models;
using Dimensions = IQI.Intuition.Reporting.Models.Dimensions;
using Cubes = IQI.Intuition.Reporting.Models.Cubes;
using Facts = IQI.Intuition.Reporting.Models.Facts;
using IQI.Intuition.Reporting.Repositories;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Logging;
using SnyderIS.sCore.Persistence;
using IQI.Intuition.Reporting.Containers;

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.Infection.CubeServices
{
    public class FacilityInfectionSite : AbstractService
    {

        protected override void Run(DataDimensions changes)
        {
            _Log.Info(string.Format("Syncing cube FacilityInfectionSite starting: {0} facility: {1}", changes.StartDate, changes.Facility.Name));

            var cube = GetQueryable<Cubes.FacilityInfectionSite>()
            .Where(x => x.Facility.Id == changes.Facility.Id)
            .FirstOrDefault();

            var facts = GetQueryable<Facts.InfectionVerification>()
                .Where(x => x.Facility.Id == changes.Facility.Id)
                .ToList();

            if (cube == null)
            {
                cube = new Cubes.FacilityInfectionSite();
                cube.Entries = new List<Cubes.FacilityInfectionSite.Entry>();
            }

            cube.Facility = changes.Facility;

            if (facts.Count() > 0)
            {
                foreach (var site in facts.Select(x => x.InfectionSite).Distinct())
                {
                    if (cube.Entries.Where(x => x.InfectionSite.Name == site.Name).Count() < 1)
                    {
                        var c = new Cubes.FacilityInfectionSite.Entry();
                        c.InfectionSite = site;
                        c.InfectionType = site.InfectionType;
                        cube.Entries.Add(c);
                    }
                }
            }

            Save(cube);

        }



    }
}
