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
using IQI.Intuition.Reporting.Models;

namespace IQI.Intuition.Web.Models.Reporting.Infection.Facility
{
    public class QuarterlyInfectionByWingView
    {
        public Guid Quarter { get; set; }

        public IEnumerable<SelectListItem> QuarterOptions { get; set; }

        public Month Month1 { get; set; }
        public Month Month2 { get; set; }
        public Month Month3 { get; set; }

        public List<Group> Groups { get; set; }

        public PieChart Month1Chart { get; private set; }
        public PieChart Month2Chart { get; private set; }
        public PieChart Month3Chart { get; private set; }
        public PieChart TotalChart { get; private set; }

        public void SetData(QuarterMonths quarter,
            IEnumerable<WingMonthInfectionSite.InfectionSiteEntry> srcdata)
        {
            /* Setup Basic Info */

            var data = new List<FlatWingEntry>();

            foreach(var site in srcdata)
            {
                foreach(var w in site.WingEntries)
                {
                    data.AddRange(w.Entries.Select(
                         x => new FlatWingEntry()
                         {
                             Change = x.Change,
                             Components = x.Components,
                             Id = x.Id,
                             InfectionSite = site.InfectionSite,
                             InfectionType = site.InfectionType,
                             Month = x.Month,
                             Rate = x.Rate,
                             Total = x.Total,
                             ViewAction = x.ViewAction,
                             Wing = w.Wing

                         }
                        ));
                }
            }


            this.Month1 = quarter.Month1;
            this.Month2 = quarter.Month2;
            this.Month3 = quarter.Month3;

            Month1Chart = new PieChart();
            Month2Chart = new PieChart();
            Month3Chart = new PieChart();
            TotalChart = new PieChart();


            var totalData = new Dictionary<Wing, int>();
            var wings = data.Select(x => x.Wing).Distinct();

            IEnumerable<FlatWingEntry> month1Total =
                data.Where(m => m.Month == Month1).ToList();

            var month1Data = new Dictionary<Wing, int>();
            ApplyTotals(month1Total, month1Data, wings);
            ApplyTotals(month1Total, totalData, wings);

            IEnumerable<FlatWingEntry> month2Total =
                data.Where(m => m.Month == Month2).ToList();

            var month2Data = new Dictionary<Wing, int>();
            ApplyTotals(month2Total, month2Data, wings);
            ApplyTotals(month2Total, totalData, wings);

            IEnumerable<FlatWingEntry> month3Total =
                data.Where(m => m.Month == Month3).ToList();

            var month3Data = new Dictionary<Wing, int>();
            ApplyTotals(month3Total, month3Data, wings);
            ApplyTotals(month3Total, totalData, wings);


            FillChart(Month1Chart, month1Data);
            FillChart(Month2Chart, month2Data);
            FillChart(Month3Chart, month3Data);
            FillChart(TotalChart, totalData);

            this.Groups = new List<Group>();

            var types = data.Select(x => x.InfectionType).Distinct();


            foreach (var wing in wings)
            {
                var group = new Group();
                group.Description = string.Concat(wing.Floor.Name, " - ", wing.Name);
                group.Details = new List<Detail>();
                this.Groups.Add(group);

                foreach (var type in types.OrderBy(x => x.SortOrder))
                {
                    var entry = new Detail();
                    entry.IsSubCategory = true;

                    entry.Description = type.Name;

                    entry.Month1Total = new Stat();
                    entry.Month1Total.Total = month1Total.Where(x => x.InfectionType.Id == type.Id && x.Wing.Id == wing.Id).Sum( x => x.Total);
                    entry.Month1Total.Rate = month1Total.Where(x => x.InfectionType.Id == type.Id && x.Wing.Id == wing.Id).Sum(x => x.Rate);
                    entry.Month1Total.Change = month1Total.Where(x => x.InfectionType.Id == type.Id && x.Wing.Id == wing.Id).Sum(x => x.Change);
                    FillAnnotation(month1Total.Where(x => x.InfectionType.Id == type.Id && x.Wing.Id == wing.Id), entry.Month1Total);

                    entry.Month2Total = new Stat();
                    entry.Month2Total.Total = month2Total.Where(x => x.InfectionType.Id == type.Id && x.Wing.Id == wing.Id).Sum(x => x.Total);
                    entry.Month2Total.Rate = month2Total.Where(x => x.InfectionType.Id == type.Id && x.Wing.Id == wing.Id).Sum(x => x.Rate);
                    entry.Month2Total.Change = month2Total.Where(x => x.InfectionType.Id == type.Id && x.Wing.Id == wing.Id).Sum(x => x.Change);
                    FillAnnotation(month2Total.Where(x => x.InfectionType.Id == type.Id && x.Wing.Id == wing.Id), entry.Month2Total);

                    entry.Month3Total = new Stat();
                    entry.Month3Total.Total = month3Total.Where(x => x.InfectionType.Id == type.Id && x.Wing.Id == wing.Id).Sum(x => x.Total);
                    entry.Month3Total.Rate = month3Total.Where(x => x.InfectionType.Id == type.Id && x.Wing.Id == wing.Id).Sum(x => x.Rate);
                    entry.Month3Total.Change = month3Total.Where(x => x.InfectionType.Id == type.Id && x.Wing.Id == wing.Id).Sum(x => x.Change);
                    FillAnnotation(month3Total.Where(x => x.InfectionType.Id == type.Id && x.Wing.Id == wing.Id), entry.Month3Total);
                    
                    if (entry.Month1Total.Total > 0|| entry.Month2Total.Total > 0 || entry.Month3Total.Total > 0
                        || entry.Month1Total.Change > 0 || entry.Month2Total.Change > 0 || entry.Month3Total.Change > 0)
                    {

                        group.Details.Add(entry);

                        var sites = data.Where(x => x.InfectionType.Id == type.Id).Select(x => x.InfectionSite).Distinct();

                        foreach (var site in sites)
                        {
                            var sentry = new Detail();
                            sentry.IsSubCategory = false;
                            sentry.Description = string.Concat("-", site.Name);


                            sentry.Month1Total = new Stat();
                            sentry.Month1Total.Total = month1Total.Where(x => x.InfectionSite.Id == site.Id && x.Wing.Id == wing.Id).Sum(x => x.Total);
                            sentry.Month1Total.Rate = month1Total.Where(x => x.InfectionSite.Id == site.Id && x.Wing.Id == wing.Id).Sum(x => x.Rate);
                            sentry.Month1Total.Change = month1Total.Where(x => x.InfectionSite.Id == site.Id && x.Wing.Id == wing.Id).Sum(x => x.Change);
                            FillAnnotation(month1Total.Where(x => x.InfectionSite.Id == site.Id && x.Wing.Id == wing.Id), sentry.Month1Total);

                            sentry.Month2Total = new Stat();
                            sentry.Month2Total.Total = month2Total.Where(x => x.InfectionSite.Id == site.Id && x.Wing.Id == wing.Id).Sum(x => x.Total);
                            sentry.Month2Total.Rate = month2Total.Where(x => x.InfectionSite.Id == site.Id && x.Wing.Id == wing.Id).Sum(x => x.Rate);
                            sentry.Month2Total.Change = month2Total.Where(x => x.InfectionSite.Id == site.Id && x.Wing.Id == wing.Id).Sum(x => x.Change);
                            FillAnnotation(month2Total.Where(x => x.InfectionSite.Id == site.Id && x.Wing.Id == wing.Id), sentry.Month2Total);

                            sentry.Month3Total = new Stat();
                            sentry.Month3Total.Total = month3Total.Where(x => x.InfectionSite.Id == site.Id && x.Wing.Id == wing.Id).Sum(x => x.Total);
                            sentry.Month3Total.Rate = month3Total.Where(x => x.InfectionSite.Id == site.Id && x.Wing.Id == wing.Id).Sum(x => x.Rate);
                            sentry.Month3Total.Change = month3Total.Where(x => x.InfectionSite.Id == site.Id && x.Wing.Id == wing.Id).Sum(x => x.Change);
                            FillAnnotation(month3Total.Where(x => x.InfectionSite.Id == site.Id && x.Wing.Id == wing.Id), sentry.Month3Total);

                            if (sentry.Month1Total.Total > 0 || sentry.Month2Total.Total > 0 || sentry.Month3Total.Total > 0
                                || sentry.Month1Total.Change > 0 || sentry.Month2Total.Change > 0 || sentry.Month3Total.Change > 0)
                            {

                                group.Details.Add(sentry);
                            }
                        }
                    }

                }
            }

        }

        private void FillAnnotation(IEnumerable<FlatWingEntry> entries, AnnotatedEntry e)
        {
            if (entries.Count() < 1)
            {
                return;
            }

            e.ViewAction = entries.First().ViewAction;
            e.Components = entries.SelectMany(x => x.Components); 
        }

        private void FillChart(PieChart chart, Dictionary<Wing, int> totals)
        {
            var totalCount = totals.Select(m => m.Value).Sum();
            int index = 0;

            foreach (var total in totals)
            {
                double perc = (Convert.ToDouble(total.Value) / Convert.ToDouble(totalCount) * 100);

                chart.AddItem(new PieChart.Item()
                {
                    Label = string.Concat(total.Key.Floor.Name," - ",total.Key.Name),
                    Marker = perc > 0 ? String.Format("{0:F2}%", perc) : string.Empty,
                    Value = total.Value,
                    Color = PieChart.GetDefaultColor(index)
                });

                index++;
            }
        }

        private void ApplyTotals(IEnumerable<FlatWingEntry> data, Dictionary<Wing, int> dest, IEnumerable<Wing> wings)
        {
            foreach (var w in wings)
            {
                if (!dest.ContainsKey(w))
                {
                    dest[w] = 0;
                }
            }

            foreach (var d in data)
            {
                dest[d.Wing] = dest[d.Wing] + d.Total;
            }
        }


        public class Group
        {
            public string Description;
            public IList<Detail> Details { get; set; }
        }

        public class Detail
        {
            public string Description { get; set; }
            public bool IsSubCategory { get; set; }
            public Stat Month1Total { get; set; }
            public Stat Month2Total { get; set; }
            public Stat Month3Total { get; set; }
        }

        public class Stat : AnnotatedEntry
        {
            public int Total { get; set; }
            public decimal Rate { get; set; }
            public decimal Change { get; set; }
        }

        public class FlatWingEntry : AnnotatedEntry
        {
            public virtual int Total { get; set; }
            public virtual Decimal Rate { get; set; }
            public virtual Month Month { get; set; }
            public virtual Decimal Change { get; set; }
            public virtual Wing Wing { get; set; }
            public virtual InfectionType InfectionType { get; set; }
            public virtual InfectionSite InfectionSite { get; set; }
        }
    }
}
