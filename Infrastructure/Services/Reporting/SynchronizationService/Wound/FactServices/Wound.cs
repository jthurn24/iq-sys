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
using RedArrow.Framework.Extensions.Formatting;
using IQI.Intuition.Reporting.Containers;

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.Wound.FactServices
{
    public class Wound: AbstractFactService
    {
        public Wound(
            IDimensionBuilderRepository db,
            IDimensionRepository d,
            ICubeBuilderRepository cb,
            IStatelessDataContext dc,
            IFactBuilderRepository fb,
            ILog log,
            IDocumentStore ds)
            : base(db, d, cb, dc, fb, log, ds)
        {

        }


        public void Run(
            IList<Domain.Models.WoundReport> dReports,
            Domain.Models.Facility dFacility,
            DataDimensions dimensions)
        {
            foreach (var dReport in dReports)
            {

                _Log.Info(string.Format("Syncing Wound: {0}  facility: {1}", dReport.Guid, dFacility.Name));

                Dimensions.Day firstNotedOnDay = null;
                Dimensions.Month firstNotedOnMonth = null;
                Dimensions.Quarter firstNotedOnQuarter = null;

                Dimensions.Day closedOnDay = null;
                Dimensions.Month closedOnMonth = null;
                Dimensions.Quarter closedOnQuarter = null;

                if (dReport.FirstNotedOn.HasValue)
                {
                    firstNotedOnDay = _DimensionBuilderRepository.GetOrCreateDay(dReport.FirstNotedOn.Value);
                    firstNotedOnMonth = _DimensionBuilderRepository.GetOrCreateMonth(dReport.FirstNotedOn.Value.Month, dReport.FirstNotedOn.Value.Year);
                    firstNotedOnQuarter = firstNotedOnMonth.Quarter;
                }

                if (dReport.ResolvedOn.HasValue)
                {
                    closedOnDay = _DimensionBuilderRepository.GetOrCreateDay(dReport.ResolvedOn.Value);
                    closedOnMonth = _DimensionBuilderRepository.GetOrCreateMonth(dReport.ResolvedOn.Value.Month, dReport.ResolvedOn.Value.Year);
                    closedOnQuarter = closedOnMonth.Quarter;
                }

                var classification = _DimensionBuilderRepository.GetOrCreateWoundClassification(
                    System.Enum.GetName(typeof(IQI.Intuition.Domain.Enumerations.WoundClassification), dReport.Classification).SplitPascalCase());


                var dType = _DataContext.Fetch<WoundType>(dReport.WoundType.Id);

                var type = _DimensionBuilderRepository.GetOrCreateWoundType(dType.Name);

                var dSite = _DataContext.Fetch<WoundSite>(dReport.Site.Id);
                
                var site = _DimensionBuilderRepository.GetOrCreateWoundSite(
                    dSite.Name, 
                    dSite.TopLeftX.Value,
                    dSite.TopLeftY.Value, 
                    dSite.BottomRightX.Value, 
                    dSite.BottomRightY.Value);

                var dPatient = _DataContext.Fetch<Patient>(dReport.Patient.Id);
                var dAccount = _DataContext.Fetch<Domain.Models.Account>(dFacility.Account.Id);


                var account = _DimensionBuilderRepository.GetOrCreateAccount(dAccount.Guid, dAccount.Name);
                var facility = _DimensionBuilderRepository.GetOrCreateFacility(dFacility.Guid);

                /* Sync report record */

                var record = _FactBuilderRespository.GetOrCreateWoundReport(dReport.Guid);

                TrackDimensionChanges(dimensions, record);

                record.Classification = classification;
                record.FirstNotedOnDate = dReport.FirstNotedOn;
                record.FirstNotedOnMonth = firstNotedOnMonth;
                record.FirstNotedOnQuarter = firstNotedOnQuarter;
                record.ClosedOnDate = dReport.ResolvedOn;
                record.ClosedOnMonth = closedOnMonth;
                record.ClosedOnQuarter = closedOnQuarter;
                record.Account = account;
                record.Facility = facility;
                record.Deleted = dReport.Deleted;
                record.Site = site;
                record.WoundType = type;

                Save<Facts.WoundReport>(record);

                TrackDimensionChanges(dimensions, record);

                /* Sync assessment records */

                if (record.Assessments == null)
                {
                    record.Assessments = new List<Facts.WoundReport.Assessment>();
                }

                foreach (var oldAssessment in record.Assessments)
                {
                    TrackDimensionChanges(dimensions,oldAssessment);
                }

                record.Assessments.Clear();

                var dAssessments = _DataContext.CreateQuery<WoundAssessment>()
                    .FilterBy(x => x.Report.Id == dReport.Id)
                    .FilterBy(x => x.AssessmentDate != null)
                    .FetchAll()
                    .OrderBy(x => x.AssessmentDate)
                    .ToList();

                foreach (var dAssessment in dAssessments)
                {
                    var index = dAssessments.IndexOf(dAssessment);
                    var nextIndex = index + 1;

                    WoundAssessment next = nextIndex < dAssessments.Count() ? dAssessments[nextIndex] : null;

                    SyncAssessment(dimensions,
                        record,
                        dAssessment,
                        facility,
                        next);
                }

                Save<Facts.WoundReport>(record);

            }

        }


        private void SyncAssessment(DataDimensions dimensions,
            Facts.WoundReport report,
            WoundAssessment dAssessment,
            Dimensions.Facility facility,
            WoundAssessment nextAssessment
            )
        {
            var dRoom = _DataContext.Fetch<Room>(dAssessment.Room.Id);
            var dWing = _DataContext.Fetch<Wing>(dRoom.Wing.Id);
            var dFloor = _DataContext.Fetch<Floor>(dWing.Floor.Id);

            var room = _DimensionBuilderRepository.GetOrCreateRoom(dRoom.Guid);
            var wing = _DimensionBuilderRepository.GetOrCreateWing(dWing.Guid);
            var floor = _DimensionBuilderRepository.GetOrCreateFloor(dFloor.Guid);


            var day = _DimensionBuilderRepository.GetOrCreateDay(dAssessment.AssessmentDate.Value);
            var month = _DimensionBuilderRepository.GetOrCreateMonth(dAssessment.AssessmentDate.Value.Month, dAssessment.AssessmentDate.Value.Year);
            var quarter = month.Quarter;

            var dStage = _DataContext.Fetch<WoundStage>(dAssessment.Stage.Id);
            var stage = _DimensionBuilderRepository.GetOrCreateWoundStage(dStage.Name, dStage.RatingValue);

 
            Dimensions.FloorMap floorMap;

            if (facility.HasSingleFloorMap.HasValue && facility.HasSingleFloorMap.Value == true)
            {
                floorMap = _DimensionBuilderRepository.GetOrCreateFloorMap(facility);
            }
            else
            {
                floorMap = _DimensionBuilderRepository.GetOrCreateFloorMap(wing);
            }

            var floorMapRoom = _DimensionBuilderRepository.GetOrCreateFloorMapRoom(room, floorMap);


            var record = new Facts.WoundReport.Assessment();
            record.AssessmentDate = dAssessment.AssessmentDate;
            record.FloorMap = floorMap;
            record.FloorMapRoom = floorMapRoom;
            record.Room = room;
            record.Wing = wing;
            record.Floor = floor;
            record.Month = month;
            record.Quarter = quarter;
            record.Day = day;
            record.Stage = stage;
            record.CoverageEndDate = nextAssessment != null ? nextAssessment.AssessmentDate : report.ClosedOnDate;

            report.Assessments.Add(record);

            TrackDimensionChanges(dimensions, record);

        }

        private void TrackDimensionChanges(DataDimensions dimensions, Facts.WoundReport report)
        {

            if (report.Classification != null)
            {
                if (dimensions.WoundClassifications.Count(x => x.Name == report.Classification.Name) < 1)
                {
                    dimensions.WoundClassifications.Add(report.Classification);
                }
            }


            if (report.Site != null)
            {
                if (dimensions.WoundSites.Count(x => x.Name == report.Site.Name) < 1)
                {
                    dimensions.WoundSites.Add(report.Site);
                }
            }

            if (report.WoundType != null)
            {
                if (dimensions.WoundTypes.Count(x => x.Name == report.WoundType.Name) < 1)
                {
                    dimensions.WoundTypes.Add(report.WoundType);
                }
            }


            if (report.FirstNotedOnDate.HasValue)
            {
                DateTime startDate = report.FirstNotedOnDate.Value;
                DateTime endDate = DateTime.Today;

                if (report.ClosedOnDate.HasValue)
                {
                    endDate = report.ClosedOnDate.Value;
                }

                if (dimensions.StartDate.HasValue == false || dimensions.StartDate.Value > startDate)
                {
                    dimensions.StartDate = startDate;
                }

                if (dimensions.EndDate.HasValue == false || dimensions.EndDate.Value < endDate)
                {
                    dimensions.EndDate = endDate;
                }

            }         
        }


        private void TrackDimensionChanges(DataDimensions dimensions, Facts.WoundReport.Assessment assessment)
        {

            if (assessment.AssessmentDate.HasValue)
            {
                if (dimensions.StartDate.HasValue == false || dimensions.StartDate.Value > assessment.AssessmentDate.Value)
                {
                    dimensions.StartDate = assessment.AssessmentDate.Value;
                }

                if (dimensions.EndDate.HasValue == false || dimensions.EndDate.Value < assessment.AssessmentDate.Value)
                {
                    dimensions.EndDate = assessment.AssessmentDate.Value;
                }
            }

            if (assessment.Wing != null)
            {
                if (dimensions.Wings.Count(x => x.Id == assessment.Wing.Id) < 1)
                {
                    dimensions.Wings.Add(assessment.Wing);
                }
            }

            if (assessment.Stage != null)
            {
                if (dimensions.WoundStages.Count(x => x.Id == assessment.Stage.Id) < 1)
                {
                    dimensions.WoundStages.Add(assessment.Stage);
                }
            }
            
        }


    }
}
