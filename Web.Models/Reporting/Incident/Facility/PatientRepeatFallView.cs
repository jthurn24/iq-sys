using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;
using System.Web.Mvc;
using IQI.Intuition.Infrastructure.Services.BusinessLogic.Incident;
using RedArrow.Framework.Extensions.Formatting;
using IQI.Intuition.Reporting.Graphics;

namespace IQI.Intuition.Web.Models.Reporting.Incident.Facility
{
    public class PatientRepeatFallView
    {
        public IList<Entry> Entries { get; set; }
        public IEnumerable<SelectListItem> WingOptions { get; set; }
        public int? Wing { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public PieChart RepeatChart { get; set;}

        public void SetData(IEnumerable<IncidentReport> reports)
        {
            Entries = new List<Entry>();

            var patients = reports.Select(x => x.Patient).Distinct();
            var calculator = new RepeatFallCalculator();


            int repeatCount = 0;
            int singleCount = 0;

            foreach (var p in patients)
            {

                var patientIncidents = reports.Where(x => x.Patient == p);
                var lastAdmitDate = p.GetLastAdmissionDate();
                var lastNonAdmitDate = p.GetLastNonAdmissionDate();

                var adjustedStartDate = lastAdmitDate.HasValue && lastAdmitDate.Value > this.StartDate.Value ? lastAdmitDate : this.StartDate;

                DateTime? endDate = this.EndDate.HasValue ? this.EndDate.Value : DateTime.Today;

                if (p.CurrentStatus != Domain.Enumerations.PatientStatus.Admitted &&
                    (lastNonAdmitDate.HasValue && lastNonAdmitDate.Value < endDate ))
                {
                    endDate = lastNonAdmitDate;
                }


                if (patientIncidents.Count() > 1)
                {
                    repeatCount++;
                }
                else
                {
                    singleCount++;
                }

                Entries.Add(new Entry()
                {
                    Total = patientIncidents.Count(),
                    PatientName = p.FullName,
                    AveragePerMonth = calculator.AveragePerMonth(adjustedStartDate.Value, null, patientIncidents),
                    DateRange = string.Format("{0} - {1}", adjustedStartDate.Value.FormatAsShortDate(), endDate.Value.FormatAsShortDate())
                });
    
            }

            Entries = Entries.OrderByDescending(x => x.Total).ToList();

            RepeatChart = new PieChart();

            double repeatPerc = (double)repeatCount == 0 ? 0 : ((double)repeatCount / ((double)repeatCount + (double)singleCount)) * (double)100;
            RepeatChart.AddItem(new PieChart.Item() 
            {
                 Color = PieChart.GetDefaultColor(0),
                 Label= "Repeat Falls",
                 Value = repeatPerc,
                 Marker = String.Format("% {0}",Math.Round(repeatPerc,2))
            });

            double singlePerc = (double)singleCount == 0 ? 0 : ((double)singleCount / ((double)repeatCount + (double)singleCount)) * (double)100;
            RepeatChart.AddItem(new PieChart.Item()
            {
                Color = PieChart.GetDefaultColor(1),
                Label = "Single Falls",
                Value = singlePerc,
                Marker = String.Format("% {0}", Math.Round(singlePerc, 2))
            });
        }


        public class Entry
        {
            public string PatientName { get; set; }
            public int Total { get; set; }
            public decimal AveragePerMonth { get; set; }
            public string DateRange { get; set; }
        }
    }
}
