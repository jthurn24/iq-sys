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
using IQI.Intuition.Reporting.Containers;

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.Complaint.FactServices
{
    public class Complaint : AbstractFactService
    {


        public Complaint(
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
            IList<Domain.Models.Complaint> dComplaints,
            Domain.Models.Facility dFacility,
            DataDimensions dimensions)
        {
            foreach (var dComplaint in dComplaints)
            {

                _Log.Info(string.Format("Syncing complaint: {0}  facility: {1}", dComplaint.Guid, dFacility.Name));

                Dimensions.Day occurredOnDay = null;
                Dimensions.Month occurredOnMonth = null;
                Dimensions.Quarter occurredOnQuarter = null;


                occurredOnDay = _DimensionBuilderRepository.GetOrCreateDay(dComplaint.DateOccurred);
                occurredOnMonth = _DimensionBuilderRepository.GetOrCreateMonth(dComplaint.DateOccurred.Month, dComplaint.DateOccurred.Year);

                occurredOnQuarter = GetQueryable<Dimensions.Quarter>()
                    .Where(x => occurredOnMonth.Quarter.QuarterOfYear == x.QuarterOfYear
                        && occurredOnMonth.Quarter.Year == x.Year)
                        .First();

                var dWing = _DataContext.Fetch<Wing>(dComplaint.Wing.Id);
                var dFloor = _DataContext.Fetch<Floor>(dWing.Floor.Id);
                var dComplaintType = _DataContext.Fetch<ComplaintType>(dComplaint.ComplaintType.Id);
                var dAccount = _DataContext.Fetch<Domain.Models.Account>(dFacility.Account.Id);
                

                var wing = _DimensionBuilderRepository.GetOrCreateWing(dWing.Guid);
                var floor = _DimensionBuilderRepository.GetOrCreateFloor(dFloor.Guid);
                var account = _DimensionBuilderRepository.GetOrCreateAccount(dAccount.Guid, dAccount.Name);
                var facility = _DimensionBuilderRepository.GetOrCreateFacility(dFacility.Guid);
                var complaintType = _DimensionBuilderRepository.GetOrCreateComplaintType(dComplaintType.Name);

                var record = _FactBuilderRespository.GetOrCreateComplaint(dComplaint.Guid);

                TrackDimensionChanges(dimensions, record);

                record.Wing = wing;
                record.Floor = floor;
                record.Account = account;
                record.Facility = facility;
                record.Deleted = dComplaint.Deleted;
                record.ComplaintType = complaintType;
                record.OccurredDate = dComplaint.DateOccurred;

                record.Month = _DimensionBuilderRepository.GetOrCreateMonth(dComplaint.DateOccurred.Month, dComplaint.DateOccurred.Year);
                
                record.Quarter = GetQueryable<Dimensions.Quarter>()
                    .Where(x => record.Month.Quarter.QuarterOfYear == x.QuarterOfYear 
                        && record.Month.Quarter.Year == x.Year )
                        .First();

                TrackDimensionChanges(dimensions, record);

                Save<Facts.Complaint>(record);

            }
        }

        private void TrackDimensionChanges(DataDimensions dimensions,
            Facts.Complaint complaint
            )
        {
            if (complaint.ComplaintType != null)
            {
                if (dimensions.ComplaintTypes.Count(x => x.Name == complaint.ComplaintType.Name) < 1)
                {
                    dimensions.ComplaintTypes.Add(complaint.ComplaintType);
                }
            }


            if (complaint.Floor != null)
            {
                if (dimensions.Floors.Count(x => x.Id == complaint.Floor.Id) < 1)
                {
                    dimensions.Floors.Add(complaint.Floor);
                }
            }

            if (complaint.Wing != null)
            {
                if (dimensions.Wings.Count(x => x.Id == complaint.Wing.Id) < 1)
                {
                    dimensions.Wings.Add(complaint.Wing);
                }
            }


            if (complaint.OccurredDate.HasValue)
            {
                DateTime startDate = complaint.OccurredDate.Value;
                DateTime endDate = DateTime.Today;


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
    }
}
