using System;
using System.Collections;
using System.Collections.Generic;

namespace IQI.Intuition.Web.Models.Patient
{
    public class PatientForm
    {
        public PatientForm()
        {
            ClientData = new PatientFormClientData();
        }

        public Guid? Guid { get; set; }

        public string FirstName { get; set; }

        public string MiddleInitial { get; set; }

        public string LastName { get; set; }

        public string BirthDate { get; set; }

        public string MDName { get; set; }

        public PatientInfoStatus? CurrentStatus { get; set; }

        public PatientInfoStatus? NewStatus { get; set; }

        public DateTime? StatusChangedAt { get; set; }

        public int? Floor { get; set; }

        public int? Wing { get; set; }

        public int? Room { get; set; }

        public int? CurrentRoom { get; set; }

        public DateTime? RoomChangeAt { get; set; }

        public bool IsUpdateMode { get; set; }

        public IEnumerable<int> SelectedPatientFlags { get; set; }

        public PatientFormClientData ClientData { get; set; }

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
                    CurrentStatus = CurrentStatus.ToString(),
                    NewStatus = NewStatus.ToString(),
                    CurrentRoom = CurrentRoom
                };
            }
        }
    }
}
