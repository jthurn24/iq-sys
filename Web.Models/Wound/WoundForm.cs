using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using IQI.Intuition.Web.Models.Patient;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;

namespace IQI.Intuition.Web.Models.Wound
{
    public class WoundForm
    {

        public WoundForm()
        {
            Patient = new PatientInfo();

        }

        public void LoadSiteQuickList(IWoundRepository woundRepository)
        {
            var siteNames = woundRepository.AllSites.Select(x => x.Name).Distinct();
            this.QuickPickOptions = new List<SiteQuickPick>();

            foreach (string siteName in siteNames)
            {
                var first = woundRepository.AllSites.Where(x => x.Name == siteName).First();
                this.QuickPickOptions.Add(new SiteQuickPick()
                {
                    Name = siteName,
                    X = first.TopLeftX.Value + ((first.BottomRightX.Value - first.TopLeftX.Value) / 2),
                    Y = first.TopLeftY.Value + ((first.BottomRightY.Value - first.TopLeftY.Value) / 2)
                });
            }

            this.QuickPickOptions = this.QuickPickOptions.OrderBy(x => x.Name).ToList();
        }

        
        public int? WoundReportId { get; set; }
        public PatientInfo Patient { get; set; }
        public DateTime? FirstNoted { get; set; }
        public DateTime? ResolvedOn { get; set; }

        public int? Site { get; set; }
        public int? LocationX { get; set; }
        public int? LocationY { get; set; }
        public bool IsUpdateMode { get; set; }
        public bool? IsResolved { get; set; }
        public Domain.Enumerations.WoundClassification Classification { get; set; }
        public int? WoundType { get; set; }
        public string AdditionalSiteDetails { get; set; }


        public IList<SiteQuickPick> QuickPickOptions { get; set; }


        public class SiteQuickPick
        {
            public string Name { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
        }


    }
}
