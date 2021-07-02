using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Models.Dimensions;
using IQI.Intuition.Reporting.Models.Facts;

namespace IQI.Intuition.Reporting.Repositories
{
    public interface IFactBuilderRepository
    {
        InfectionVerification GetOrCreateInfectionVerification(Guid guid);
        IncidentReport GetOrCreateIncidentReport(Guid guid);
        PsychotropicAdministration GetOrCreatePsychotropicAdministration(Guid guid);
        WoundReport GetOrCreateWoundReport(Guid guid);
        Complaint GetOrCreateComplaint(Guid guid);
        Catheter GetOrCreateCatheter(Guid guid);
    }
}
