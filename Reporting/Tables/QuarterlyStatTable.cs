using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Models.Dimensions;

namespace IQI.Intuition.Reporting.Tables
{
    public class QuarterlyStatTable<T>
    {
        public List<QuarterlyStatTableGroup<T>> Groups { get; set; }
        public QuarterlyStatTableStat<T> Month1Total { get; set; }
        public QuarterlyStatTableStat<T> Month2Total { get; set; }
        public QuarterlyStatTableStat<T> Month3Total { get; set; }

        public Month Month1 { get; set; }
        public Month Month2 { get; set; }
        public Month Month3 { get; set; }

        public string CategoryDescription { get; set; }
        public string CountDescription { get; set; }

        public static void LoadTable<T, TT>(
            QuarterlyStatTable<T> viewTable,
            IEnumerable<TT> totals,
            Func<TT, int> totalFunc,
            Func<TT, Month> monthFunc,
            Func<TT, string> descriptorFunc,
            Func<TT, decimal> changeFunc,
            Func<TT, decimal> rateFunc
            )
        {

            viewTable.Groups = new List<QuarterlyStatTableGroup<T>>();
            viewTable.Month1Total = new QuarterlyStatTableStat<T>();
            viewTable.Month2Total = new QuarterlyStatTableStat<T>();
            viewTable.Month3Total = new QuarterlyStatTableStat<T>();

            foreach (var total in totals)
            {

                /* Add item to table */
                QuarterlyStatTableStat<T> stat;
                QuarterlyStatTableGroup<T> group;

                if (viewTable.Groups.Where(m => m.Description == descriptorFunc.Invoke(total)).Count() < 1)
                {
                    viewTable.Groups.Add(
                        new QuarterlyStatTableGroup<T>()
                        {
                            Description = descriptorFunc.Invoke(total),
                            Month1Total = new QuarterlyStatTableStat<T>(),
                            Month2Total = new QuarterlyStatTableStat<T>(),
                            Month3Total = new QuarterlyStatTableStat<T>()
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

                if (stat.Components == null)
                {
                    stat.Components = new List<Guid>();
                }

                var att = total as Reporting.Models.AnnotatedEntry;

                if (att != null && att.Components != null)
                {
                    var l =  new List<Guid>();
                    l.AddRange(att.Components);
                    l.AddRange(stat.Components);
                    stat.Components = l;
                    stat.ViewAction = att.ViewAction;
                }

                /* add to totals */

                QuarterlyStatTableStat<T> groupedStat = null;

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
            }
        }
    }

    public class QuarterlyStatTableGroup<T>
    {
        public string Description { get; set; }
        public QuarterlyStatTableStat<T> Month1Total { get; set; }
        public QuarterlyStatTableStat<T> Month2Total { get; set; }
        public QuarterlyStatTableStat<T> Month3Total { get; set; }
    }

    public class QuarterlyStatTableStat<T> : Reporting.Models.AnnotatedEntry
    {
        public int Count { get; set; }
        public decimal Change { get; set; }
        public decimal Rate { get; set; }
        public object Source { get; set; }
    }
}
