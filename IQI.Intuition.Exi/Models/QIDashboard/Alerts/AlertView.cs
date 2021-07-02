using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.DataSource;
using SnyderIS.sCore.Exi.Implementation.DataSource;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using StructureMap;
using StructureMap.Query;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Infrastructure.Services;

namespace IQI.Intuition.Exi.Models.QIDashboard.Alerts
{
    public class AlertView 
    {

        public int Id { get; set; }

        public string Title { get; set; }

        public string TriggeredOn { get; set; }

        public string Target { get; set; }

        public string PatientName { get; set; }

        public string FacilityName { get; set; }

        public string DescriptionText { get; set; }

        public bool Recent { get; set; }

    }
}
