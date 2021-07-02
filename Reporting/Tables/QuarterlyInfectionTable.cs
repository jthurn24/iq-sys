using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Models.Dimensions;
using IQI.Intuition.Reporting.Models;

namespace IQI.Intuition.Reporting.Tables
{
    public class QuarterlyInfectionTable<T>
    {
        public List<QuarterlyInfectionTableGroup<T>> Groups { get; set; }
        public QuarterlyInfectionTableStat<T> Month1Total { get; set; }
        public QuarterlyInfectionTableStat<T> Month2Total { get; set; }
        public QuarterlyInfectionTableStat<T> Month3Total { get; set; }

        public Month Month1 { get; set; }
        public Month Month2 { get; set; }
        public Month Month3 { get; set; }

        public string CategoryDescription { get; set; }
        public string CountDescription { get; set; }

        public static void LoadTable<T, TT>(
            QuarterlyInfectionTable<T> viewTable,
            IEnumerable<TT> totals,
            Func<TT, int> totalFunc,
            Func<TT, Month> monthFunc,
            Func<TT, string> descriptorFunc,
            Func<TT, decimal> changeFunc,
            Func<TT, decimal> rateFunc,
            Func<TT, int> nonNosoFunc
            )
        {

            viewTable.Groups = new List<QuarterlyInfectionTableGroup<T>>();
            viewTable.Month1Total = new QuarterlyInfectionTableStat<T>();
            viewTable.Month2Total = new QuarterlyInfectionTableStat<T>();
            viewTable.Month3Total = new QuarterlyInfectionTableStat<T>();

            foreach (var total in totals)
            {

                /* Add item to table */
                QuarterlyInfectionTableStat<T> stat;
                QuarterlyInfectionTableGroup<T> group;

                if (viewTable.Groups.Where(m => m.Description == descriptorFunc.Invoke(total)).Count() < 1)
                {
                    viewTable.Groups.Add(
                        new QuarterlyInfectionTableGroup<T>()
                        {
                            Description = descriptorFunc.Invoke(total),
                            Month1Total = new QuarterlyInfectionTableStat<T>(),
                            Month2Total = new QuarterlyInfectionTableStat<T>(),
                            Month3Total = new QuarterlyInfectionTableStat<T>()
                        });
                }

                group = viewTable.Groups.Where(m => m.Description == descriptorFunc.Invoke(total)).First();

                if (monthFunc.Invoke(total) == viewTable.Month1)
                {
                    stat = group.Month1Total;
                }
                else if (monthFunc.Invoke(total) == viewTable.Month2)
                {
                    stat = group.Month2Total;
                }
                else
                {
                    stat = group.Month3Total;
                }

                stat.Count += totalFunc.Invoke(total);
                stat.Change += changeFunc.Invoke(total);
                stat.Rate += rateFunc.Invoke(total);
                stat.NonNoso = nonNosoFunc.Invoke(total);

                if (stat.Components == null)
                {
                    stat.Components = new List<Guid>();
                }

                var att = total as Reporting.Models.AnnotatedEntry;

                if (att != null && att.Components != null)
                {
                    var l = new List<Guid>();
                    l.AddRange(att.Components);
                    l.AddRange(stat.Components);
                    stat.Components = l;
                    stat.ViewAction = att.ViewAction;
                }

                /* add to totals */

                QuarterlyInfectionTableStat<T> groupedStat = null;

                if (monthFunc.Invoke(total) == viewTable.Month1)
                {
                    groupedStat = viewTable.Month1Total;
                }
                else if (monthFunc.Invoke(total) == viewTable.Month2)
                {
                    groupedStat = viewTable.Month2Total;
                }
                else
                {
                    groupedStat = viewTable.Month3Total;
                }

                groupedStat.Count += totalFunc.Invoke(total);
                groupedStat.Change += changeFunc.Invoke(total);
                groupedStat.Rate += rateFunc.Invoke(total);
                groupedStat.NonNoso += nonNosoFunc.Invoke(total);
            }
        }
    }

    public class QuarterlyInfectionTableGroup<T>
    {
        public string Description { get; set; }
        public QuarterlyInfectionTableStat<T> Month1Total { get; set; }
        public QuarterlyInfectionTableStat<T> Month2Total { get; set; }
        public QuarterlyInfectionTableStat<T> Month3Total { get; set; }
    }

    public class QuarterlyInfectionTableStat<T> :AnnotatedEntry
    {
        public int Count { get; set; }
        public decimal Change { get; set; }
        public decimal Rate { get; set; }
        public int NonNoso { get; set; }
    }
}
