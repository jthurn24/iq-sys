using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Graphics;
using System.Web.Mvc;
using IQI.Intuition.Reporting.Models.Cubes;
using IQI.Intuition.Reporting.Models.Dimensions;
using System.Drawing;
using RedArrow.Framework.Extensions.Formatting;
using IQI.Intuition.Reporting.Tables;
using IQI.Intuition.Reporting.Containers;

namespace IQI.Intuition.Web.Models.Reporting.Incident.Facility
{
    public class QuarterlyIncidentControlView
    {

        public Guid Quarter { get; set; }

        public IEnumerable<SelectListItem> QuarterOptions { get; set; }

        public Month Month1 { get; set; }
        public Month Month2 { get; set; }
        public Month Month3 { get; set; }

        public SeriesColumnChart IncidentTypeChart { get; private set; }
        public QuarterlyStatTable<IncidentTypeGroup> IncidentTypeView { get; private set; }

        public SeriesColumnChart IncidentInjuryChart { get; private set; }
        public QuarterlyStatTable<IncidentInjury> IncidentInjuryView { get; private set; }

        public SeriesColumnChart IncidentInjuryLevelChart { get; private set; }
        public QuarterlyStatTable<IncidentInjuryLevel> IncidentInjuryLevelView { get; private set; }

        public SeriesColumnChart IncidentLocationChart { get; private set; }
        public QuarterlyStatTable<IncidentLocation> IncidentLocationView { get; private set; }

        public SeriesColumnChart IncidentDayOfWeekChart { get; private set; }
        public QuarterlyStatTable<int> IncidentDayOfWeekView { get; private set; }

        public SeriesColumnChart IncidentHourOfDayChart { get; private set; }
        public QuarterlyStatTable<int> IncidentHourOfDayView { get; private set; }

        public QuarterlyStatTable<Floor> IncidentFloorView { get; private set; }

        public QuarterlyStatTable<Wing> IncidentWingView { get; private set; }

        public CensusTable CensusView { get; private set; }

        private Dictionary<int, string> HourGroups;

        public void SetData(QuarterMonths quarter,
            IEnumerable<FacilityMonthCensus> census,
            IEnumerable<FacilityMonthIncidentTypeGroup.Entry> typeTotals,
            IEnumerable<FacilityMonthIncidentInjury.Entry> injuryTotals,
            IEnumerable<FacilityMonthIncidentLocation.Entry> locationTotals,
            IEnumerable<FacilityMonthIncidentInjuryLevel.Entry> injuryLevelTotals,
            IEnumerable<FacilityMonthIncidentDayOfWeek.Entry> dayOfWeekTotals,
            IEnumerable<FacilityMonthIncidentHourOfDay.Entry> hourOfDayTotals,
            IEnumerable<FloorMonthIncidentTypeGroup.Entry> floorTotals,
            IEnumerable<WingMonthIncidentTypeGroup.Entry> wingTotals)
        {

            /* Setup Basic Info */

            HourGroups = new Dictionary<int, string>();
            HourGroups.Add(0, "2400-0200");
            HourGroups.Add(1, "2400-0200");
            HourGroups.Add(2, "0200-0400");
            HourGroups.Add(3, "0200-0400");
            HourGroups.Add(4, "0400-0600");
            HourGroups.Add(5, "0400-0600");
            HourGroups.Add(6, "0600-0800");
            HourGroups.Add(7, "0600-0800");
            HourGroups.Add(8, "0800-1000");
            HourGroups.Add(9, "0800-1000");
            HourGroups.Add(10, "1000-1200");
            HourGroups.Add(11, "1000-1200");
            HourGroups.Add(12, "1200-1400");
            HourGroups.Add(13, "1200-1400");
            HourGroups.Add(14, "1400-1600");
            HourGroups.Add(15, "1400-1600");
            HourGroups.Add(16, "1600-1800");
            HourGroups.Add(17, "1600-1800");
            HourGroups.Add(18, "1800-2000");
            HourGroups.Add(19, "1800-2000");
            HourGroups.Add(20, "2000-2200");
            HourGroups.Add(21, "2000-2200");
            HourGroups.Add(22, "2200-2400");
            HourGroups.Add(23, "2200-2400");


            this.Month1 = quarter.Month1;
            this.Month2 = quarter.Month2;
            this.Month3 = quarter.Month3;


            /* Map Cenus Info */
            CensusView = new CensusTable();

            CensusView.Month1 = new CensusStat()
            {
                AverageCensus = census.Where(m => m.Month.Id == this.Month1.Id).Select(m => m.Average).FirstOrDefault(),
                TimePeriod = census.Where(m => m.Month.Id == this.Month1.Id).Select(m => m.TotalDays).FirstOrDefault(),
                PatientDays = census.Where(m => m.Month.Id == this.Month1.Id).Select(m => m.TotalPatientDays).FirstOrDefault()
            };


            CensusView.Month2 = new CensusStat()
            {
                AverageCensus = census.Where(m => m.Month.Id == this.Month2.Id).Select(m => m.Average).FirstOrDefault(),
                TimePeriod = census.Where(m => m.Month.Id == this.Month2.Id).Select(m => m.TotalDays).FirstOrDefault(),
                PatientDays = census.Where(m => m.Month.Id == this.Month2.Id).Select(m => m.TotalPatientDays).FirstOrDefault()
            };

            CensusView.Month3 = new CensusStat()
            {
                AverageCensus = census.Where(m => m.Month.Id == this.Month3.Id).Select(m => m.Average).FirstOrDefault(),
                TimePeriod = census.Where(m => m.Month.Id == this.Month3.Id).Select(m => m.TotalDays).FirstOrDefault(),
                PatientDays = census.Where(m => m.Month.Id == this.Month3.Id).Select(m => m.TotalPatientDays).FirstOrDefault()
            };


            IncidentTypeView = new QuarterlyStatTable<IncidentTypeGroup>();
            IncidentTypeView.CategoryDescription = "Type";
            IncidentTypeView.CountDescription = "Inc";
            IncidentTypeView.Month1 = this.Month1;
            IncidentTypeView.Month2 = this.Month2;
            IncidentTypeView.Month3 = this.Month3;
            IncidentTypeChart = new SeriesColumnChart();

            QuarterlyStatTable<IncidentTypeGroup>.LoadTable(
                IncidentTypeView,
                typeTotals,
                x => x.Total,
                x => x.Month,
                x => x.IncidentTypeGroup.Name,
                x => x.Change,
                x => x.Rate);


            IncidentInjuryView = new QuarterlyStatTable<IncidentInjury>();
            IncidentInjuryView.CategoryDescription = "Injury";
            IncidentInjuryView.CountDescription = "Inc";
            IncidentInjuryView.Month1 = this.Month1;
            IncidentInjuryView.Month2 = this.Month2;
            IncidentInjuryView.Month3 = this.Month3;
            IncidentInjuryChart = new SeriesColumnChart();


            QuarterlyStatTable<IncidentInjury>.LoadTable(
                IncidentInjuryView,
                injuryTotals,
                x => x.Total,
                x => x.Month,
                x => x.IncidentInjury.Name,
                x => x.Change,
                x => x.Rate);


            IncidentLocationView = new QuarterlyStatTable<IncidentLocation>();
            IncidentLocationView.CategoryDescription = "Location";
            IncidentLocationView.CountDescription = "Inc";
            IncidentLocationView.Month1 = this.Month1;
            IncidentLocationView.Month2 = this.Month2;
            IncidentLocationView.Month3 = this.Month3;
            IncidentLocationChart = new SeriesColumnChart();


            QuarterlyStatTable<IncidentLocation>.LoadTable(
                IncidentLocationView,
                locationTotals,
                x => x.Total,
                x => x.Month,
                x => x.IncidentLocation.Name,
                x => x.Change,
                x => x.Rate);


            IncidentInjuryLevelView = new QuarterlyStatTable<IncidentInjuryLevel>();
            IncidentInjuryLevelView.CategoryDescription = "Injury Severity";
            IncidentInjuryLevelView.CountDescription = "Inc";
            IncidentInjuryLevelView.Month1 = this.Month1;
            IncidentInjuryLevelView.Month2 = this.Month2;
            IncidentInjuryLevelView.Month3 = this.Month3;
            IncidentInjuryLevelChart = new SeriesColumnChart();

            QuarterlyStatTable<IncidentInjuryLevel>.LoadTable(
                IncidentInjuryLevelView,
                injuryLevelTotals,
                x => x.Total,
                x => x.Month,
                x => x.IncidentInjuryLevel.Name.SplitPascalCase(),
                x => x.Change,
                x => x.Rate);

            IncidentDayOfWeekView = new QuarterlyStatTable<int>();
            IncidentDayOfWeekView.CategoryDescription = "Day";
            IncidentDayOfWeekView.CountDescription = "Inc";
            IncidentDayOfWeekView.Month1 = this.Month1;
            IncidentDayOfWeekView.Month2 = this.Month2;
            IncidentDayOfWeekView.Month3 = this.Month3;
            IncidentDayOfWeekChart = new SeriesColumnChart();


            QuarterlyStatTable<int>.LoadTable(
                IncidentDayOfWeekView,
                dayOfWeekTotals,
                x => x.Total,
                x => x.Month,
                x => ((DayOfWeek)x.DayOfWeek).ToString(),
                x => x.Change,
                x => x.Rate);


            IncidentHourOfDayView = new QuarterlyStatTable<int>();
            IncidentHourOfDayView.CategoryDescription = "Hour";
            IncidentHourOfDayView.CountDescription = "Inc";
            IncidentHourOfDayView.Month1 = this.Month1;
            IncidentHourOfDayView.Month2 = this.Month2;
            IncidentHourOfDayView.Month3 = this.Month3;
            IncidentHourOfDayChart = new SeriesColumnChart();

            QuarterlyStatTable<int>.LoadTable(
                IncidentHourOfDayView,
                hourOfDayTotals,
                x => x.Total,
                x => x.Month,
                x => HourGroups[x.HourOfDay.Value],
                x => x.Change,
                x => x.Rate);


            IncidentFloorView = new QuarterlyStatTable<Floor>();
            IncidentFloorView.CategoryDescription = "Floor";
            IncidentFloorView.CountDescription = "Inc";
            IncidentFloorView.Month1 = this.Month1;
            IncidentFloorView.Month2 = this.Month2;
            IncidentFloorView.Month3 = this.Month3;

            QuarterlyStatTable<Floor>.LoadTable(
                IncidentFloorView,
                floorTotals,
                x => x.Total,
                x => x.Month,
                x => x.Floor.Name,
                x => x.Change,
                x => x.Rate);


            IncidentWingView = new QuarterlyStatTable<Wing>();
            IncidentWingView.CategoryDescription = "Wing";
            IncidentWingView.CountDescription = "Inc";
            IncidentWingView.Month1 = this.Month1;
            IncidentWingView.Month2 = this.Month2;
            IncidentWingView.Month3 = this.Month3;

            QuarterlyStatTable<Wing>.LoadTable(
                IncidentWingView,
                wingTotals,
                x => x.Total,
                x => x.Month,
                x => x.Wing.Name,
                x => x.Change,
                x => x.Rate);
        }


        /* Census */

        public class CensusTable
        {
            public CensusStat Month1 { get; set; }
            public CensusStat Month2 { get; set; }
            public CensusStat Month3 { get; set; }
        }

        public class CensusStat
        {
            public decimal AverageCensus { get; set; }
            public int TimePeriod { get; set; }
            public int PatientDays { get; set; }
        }



 
    }
}
