using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Models.Dimensions;
using IQI.Intuition.Reporting.Models.Facts;

namespace IQI.Intuition.Infrastructure.Services.BusinessLogic.Wound
{
    public class ReportingStageCalulator
    {

        public IEnumerable<StageDescription> AnalyzeStages(
            IEnumerable<WoundReport> reports,
            IEnumerable<WoundStage> allStages,
            DateTime startDate,
            DateTime endDate)
        {
            var result = new List<StageDescription>();

            if (reports != null && reports.Count() > 0)
            {
                foreach (var report in reports)
                {

                    if (report.Assessments.Count() > 0)
                    {
                        var entry = AnalyzeStages(report,  allStages, startDate, endDate);

                        if (entry != null)
                        {
                            result.Add(entry);
                        }
                    }

                }
            }

            return result;
        }


        public StageDescription AnalyzeStages(
            WoundReport report,
            IEnumerable<WoundStage> allStages,
            DateTime startDate,
            DateTime endDate)
        {

            if (report.Assessments != null && report.Assessments.Count() < 1)
            {
                return null;
            }

            /* If this report was closed before the start date, return null since we can't determine an applicable stage */

            if (report.ClosedOnDate.HasValue && report.ClosedOnDate <= startDate)
            {
                return null;
            }


            /* If this report started after the specified time frame, return null since an aplicable stage can't be determined */

            var applicableAssessments = report.Assessments.Where(x => x.AssessmentDate >= startDate && x.AssessmentDate <= endDate);

            /* if there are no assessments that fit within the range specified, try to find the most recent assessment before the startDate */

            if (applicableAssessments.Count() < 1)
            {
                var priorAssessments = report.Assessments.Where(x => x.AssessmentDate < startDate);

                if (priorAssessments.Count() < 1)
                {
                    /* No prior assessments exists. This is likely a data issue since we are testing to see if
                     the report is applicable to the specified time frame. We have to return null */

                    return null;
                }

                var applicablePriorAssessment = priorAssessments.OrderBy(x => x.AssessmentDate).Last();

                return new StageDescription()
                {
                    Report = report,
                    MaxStage = applicablePriorAssessment.Stage,
                    MinStage = applicablePriorAssessment.Stage
                };

            }

            /* We have applicable assessments, so do calcs */

            var result = new StageDescription();
            result.Report = report;

            /* We get stages from the master list provided to stay friendly to stateless data environments */
            var applicableStages = allStages.Where(x => applicableAssessments.Select(xx => xx.Stage.Name).Contains(x.Name)).OrderBy(x => x.Rating);

            result.MaxStage = applicableStages.Last();
            result.MinStage = applicableStages.First();

            return result;
        }


        public class StageDescription
        {
            public WoundReport Report { get; set; }
            public WoundStage MaxStage { get; set; }
            public WoundStage MinStage { get; set; }
        }

    }
}
