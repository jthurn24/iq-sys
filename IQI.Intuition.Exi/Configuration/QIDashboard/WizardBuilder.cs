using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Implementation.Widget.FluentWizard;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Reporting.Repositories;


namespace IQI.Intuition.Exi.Configuration.QIDashboard
{
    public class WizardBuilder : SnyderIS.sCore.Exi.Implementation.Widget.FluentWizard.WizardBuilder
    {
        public WizardBuilder(IActionContext context,
            IDimensionRepository dimensionRepository)
        {
            WarningConfig.Build(this);
            InfectionConfig.Build(this);
            IncidentConfig.Build(this, dimensionRepository);
            WoundConfig.Build(this);
            AuditConfig.Build(this);
            FloorMapConfig.Build(this, dimensionRepository, context);
        }
    }
}
