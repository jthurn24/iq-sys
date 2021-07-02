using System;
using System.Collections.Generic;
using RedArrow.Framework.Extensions.Common;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Web.Models.Patient;
using System.Web.Mvc;

namespace IQI.Intuition.Web.Models.Catheter
{
    public class CatheterForm
    {
        public CatheterForm()
        {
            Patient = new PatientInfo();
            ClientData = new CatheterFormClientData();
        }

        public int? CatheterEntryId { get; set; }
        public Guid? Guid { get; set; }
        public PatientInfo Patient { get; set; }
        public string Diagnosis { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? DiscontinuedDate { get; set; }
        public int? Type { get; set; }
        public int? Material { get; set; }
        public int? Reason { get; set; }
        public bool? PatientEducated { get; set; }
        public bool? FamilyEducated { get; set; }

        public bool IsUpdateMode { get; set; }


        public int? Floor { get; set; }

        public int? Wing { get; set; }

        public int? Room { get; set; }
        
        public CatheterFormClientData ClientData { get; set; }

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
                    Rooms = ClientData.Rooms,
                };
            }
        }
      
    }
}
