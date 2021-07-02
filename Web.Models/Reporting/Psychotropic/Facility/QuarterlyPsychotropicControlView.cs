using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Graphics;
using System.Web.Mvc;
using IQI.Intuition.Reporting.Models.Cubes;
using IQI.Intuition.Reporting.Models.Dimensions;
using System.Drawing;
using IQI.Intuition.Reporting.Containers;

namespace IQI.Intuition.Web.Models.Reporting.Psychotropic.Facility
{
    public class QuarterlyPsychotropicControlView
    {
        public Guid Quarter { get; set; }

        public IEnumerable<SelectListItem> QuarterOptions { get; set; }

        public Month Month1 { get; set; }
        public Month Month2 { get; set; }
        public Month Month3 { get; set; }

        public IList<PsychotropicGroup> Groups { get; set; }

        public SeriesColumnChart ActiveChart { get; private set; }

        public void SetData(QuarterMonths quarter,
            IEnumerable<FacilityMonthPsychotropicDrugType.Entry> totals,
            IEnumerable<PsychotropicDrugType> drugTypes)
        {

            this.Month1 = quarter.Month1;
            this.Month2 = quarter.Month2;
            this.Month3 = quarter.Month3;

            this.ActiveChart = new SeriesColumnChart();

            Groups = new List<PsychotropicGroup>();

            foreach (var drugType in drugTypes)
            {
                var group = new PsychotropicGroup();
                Groups.Add(group);
                group.Name = drugType.Name;

                group.Month1 = new PsychotropicStat();
                group.Month2 = new PsychotropicStat();
                group.Month3 = new PsychotropicStat();

                var month1Stat = totals.Where(x => x.Month.Id == Month1.Id && x.DrugType.Id == drugType.Id).FirstOrDefault();
                var month2Stat = totals.Where(x => x.Month.Id == Month2.Id && x.DrugType.Id == drugType.Id).FirstOrDefault();
                var month3Stat = totals.Where(x => x.Month.Id == Month3.Id && x.DrugType.Id == drugType.Id).FirstOrDefault();

                if (month1Stat != null)
                {
                    group.Month1.ActiveChange = month1Stat.ActiveChange.Value;
                    group.Month1.ActiveCount = month1Stat.ActiveCount.Value;
                    group.Month1.ActiveRate = month1Stat.ActiveRate.Value;
                    group.Month1.DecreaseCount = month1Stat.DecreaseCount.Value;
                    group.Month1.DosageChange = month1Stat.DosageChange.Value;
                    group.Month1.IncreaseCount = month1Stat.IncreaseCount.Value;

                    ActiveChart.AddItem(new Intuition.Reporting.Graphics.SeriesColumnChart.Item()
                    {
                        Category = drugType.Name,
                        Series = this.Month1.Name,
                        Value = month1Stat.ActiveCount.Value,
                    });
                }



                if (month2Stat != null)
                {
                    group.Month2.ActiveChange = month2Stat.ActiveChange.Value;
                    group.Month2.ActiveCount = month2Stat.ActiveCount.Value;
                    group.Month2.ActiveRate = month2Stat.ActiveRate.Value;
                    group.Month2.DecreaseCount = month2Stat.DecreaseCount.Value;
                    group.Month2.DosageChange = month2Stat.DosageChange.Value;
                    group.Month2.IncreaseCount = month2Stat.IncreaseCount.Value;

                    ActiveChart.AddItem(new Intuition.Reporting.Graphics.SeriesColumnChart.Item()
                    {
                        Category = drugType.Name,
                        Series = this.Month2.Name,
                        Value = month2Stat.ActiveCount.Value,
                    });

                }


                if (month3Stat != null)
                {
                    group.Month3.ActiveChange = month3Stat.ActiveChange.Value;
                    group.Month3.ActiveCount = month3Stat.ActiveCount.Value;
                    group.Month3.ActiveRate = month3Stat.ActiveRate.Value;
                    group.Month3.DecreaseCount = month3Stat.DecreaseCount.Value;
                    group.Month3.DosageChange = month3Stat.DosageChange.Value;
                    group.Month3.IncreaseCount = month3Stat.IncreaseCount.Value;

                    ActiveChart.AddItem(new Intuition.Reporting.Graphics.SeriesColumnChart.Item()
                    {
                        Category = drugType.Name,
                        Series = this.Month3.Name,
                        Value = month3Stat.ActiveCount.Value,
                    });
                }



            }

        }

        public class PsychotropicGroup
        {
            public string Name { get; set; }
            public PsychotropicStat Month1 { get; set; }
            public PsychotropicStat Month2 { get; set; }
            public PsychotropicStat Month3 { get; set; }
        }

        public class PsychotropicStat
        {
            public int IncreaseCount { get; set; }
            public int DecreaseCount { get; set; }
            public int ActiveCount { get; set; }
            public decimal ActiveRate { get; set; }
            public int ActiveChange { get; set; }
            public int DosageChange { get; set; }
        }

    }
}
