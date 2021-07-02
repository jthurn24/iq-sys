using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.IO;
using System.Web.Mvc;


namespace IQI.Intuition.Web.Models.Reporting.Infection.Facility
{
    public class OrganismYearly
    {
        public int? Year { get; set; }
        public int? WingId { get; set; }
        public IEnumerable<SelectListItem> WingOptions { get; set; }

        public IList<MonthEntry> MonthEntries { get; set; }
        public IList<OrganismEntry> OrganismTotals { get; set; }

        public void SetData(IEnumerable<Domain.Models.InfectionLabResultPathogen> resultPathogens)
        {
            MonthEntries = new List<MonthEntry>();
            OrganismTotals = new List<OrganismEntry>();

            int shadeCount = 1;
            bool shade = false;

            for(int i = 1; i < 13; i++)
            {
                var entry = new MonthEntry();
                entry.Name = new DateTime(2010, i, 1).ToString("MMM", System.Globalization.CultureInfo.InvariantCulture);
                entry.MonthlyOrganisms = new List<OrganismEntry>();
                MonthEntries.Add(entry);

                if (shadeCount > 1)
                {
                    shadeCount = 1;
                    shade = shade ? false : true;
                }

                entry.Shaded = shade;
                shadeCount++;

                var applicablePathogens = resultPathogens.Where(x =>
                    x.InfectionLabResult.CompletedOn.HasValue
                    && x.InfectionLabResult.CompletedOn.Value.Month == i);


                foreach (var ap in applicablePathogens)
                {
                    var orgTotal = OrganismTotals.Where(x => x.Name == ap.Pathogen.Name).FirstOrDefault();

                    if (orgTotal == null)
                    {
                        orgTotal = new OrganismEntry();
                        orgTotal.Name = ap.Pathogen.Name;
                        OrganismTotals.Add(orgTotal);
                    }

                    orgTotal.Total++;

                    var monthTotal = entry.MonthlyOrganisms.Where(x => x.Name == ap.Pathogen.Name).FirstOrDefault();

                    if (monthTotal == null)
                    {
                        monthTotal = new OrganismEntry();
                        monthTotal.Name = ap.Pathogen.Name;
                        entry.MonthlyOrganisms.Add(monthTotal);
                    }

                    monthTotal.Total++;
                    entry.Total++;

                }


            }
        }

        public string GetTotal(MonthEntry me, OrganismEntry oe)
        {
            var total = me.MonthlyOrganisms.Where(x => x.Name == oe.Name).FirstOrDefault();

            if(total != null)
            {
                return total.Total.ToString();
            }

            return string.Empty;
        }

        public class MonthEntry
        {
            public string Name { get; set; }
            public int Total { get; set;}
            public IList<OrganismEntry> MonthlyOrganisms { get; set; }
            public bool Shaded { get; set; }

        }

        public class OrganismEntry
        {
            public string Name { get; set;}
            public int Total { get; set;}
        }

    }
}
