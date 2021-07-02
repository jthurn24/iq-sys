using System;
using System.Collections.Generic;

using IQI.Intuition.Web.Models.Patient;

namespace IQI.Intuition.Web.Models.Vaccine
{
    public class VaccineForm
    {
        public VaccineForm()
        {
            Patient = new PatientInfo();
            ClientData = new VaccineFormClientData();
        }

        public int? VaccineEntryId { get; set; }
        public Guid Guid { get; set; }
        public PatientInfo Patient { get; set; }

        public int? VaccineTypeId { get; set; }
        public int? VaccineTradeNameId { get; set; }
        public int? VaccineUnitOfMeasureId { get; set; }
        public int? VaccineRefusalReasonId { get; set; }
        public int? VaccineRouteId { get; set; }
        public int? VaccineAdministrativeSiteId { get; set; }
        public int? VaccineManufacturerId { get; set; }
        public int? EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public decimal? AdministeredAmount { get; set; }
        public bool? UnknownAdmisteredAmount { get; set; }
        public DateTime? AdministeredOn { get; set; }
        public string SubstanceLotNumber { get; set; }
        public DateTime? SubstanceExpirationDate { get; set; }

        public bool IsUpdateMode { get; set; }

        public int? Floor { get; set; }

        public int? Wing { get; set; }

        public int? Room { get; set; }
        
        public VaccineFormClientData ClientData { get; set; }

        public object ClientViewModel
        {
            get
            {
                return new
                {
                    Floor = Floor,
                    Wing = Wing,
                    Room = Room,
                    Floors = ClientData.Floors,
                    Wings = ClientData.Wings,
                    Rooms = ClientData.Rooms
                };
            }
        }    }
}
