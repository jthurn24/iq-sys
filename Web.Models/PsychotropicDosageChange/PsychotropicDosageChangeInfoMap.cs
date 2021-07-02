using System;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.PsychotropicDosageChange
{
    public class PsychotropicDosageChangeInfoMap : ReadOnlyModelMap<PsychotropicDosageChangeInfo, Domain.Models.PsychotropicDosageChange>
    {
        public PsychotropicDosageChangeInfoMap()
        {

            ForProperty(model => model.Id)
                .Read(domain => domain.Id.ToString());

            ForProperty(model => model.Dosage)
                .Read(domain => FormatDosage(domain));

            ForProperty(model => model.Frequency)
                .Read(domain => domain.Frequency.Name);

            ForProperty(model => model.Change)
                .Read(domain => CalcChange(domain));

            ForProperty(model => model.StartDate)
                .Read(domain => domain.StartDate.Value.ToString("MM/dd/yy"));
        }

        private string FormatDosage(Domain.Models.PsychotropicDosageChange domain)
        {
            if (domain.GetTotalDosage().HasValue)
            {
                return string.Concat(domain.GetTotalDosage().Value, " ", domain.Administration.DosageForm.Name); 
            }

            return "N/A";
        }

        private string CalcChange(Domain.Models.PsychotropicDosageChange domain)
        {
            var changes = domain.Administration.DosageChanges.OrderBy(x => x.StartDate).ToList();

            var index = changes.IndexOf(domain);

            if (index < 1)
            {
                return "<span><img src=\"/Content/Images/nochange.png\">&nbsp;N/A</span>";
            }

            var priorDosage = changes[index - 1];

            var priorDailyTotal = priorDosage.GetDailyAverageDosage().Value;
            var currentDailyTotal = domain.GetDailyAverageDosage().Value;

            var change = Math.Round(currentDailyTotal - priorDailyTotal,2);
            var details = string.Concat(change, " ", domain.Administration.DosageForm.Name, " per day");

            if (change > 0)
            {
                details = string.Concat("<span><img src=\"/Content/Images/up.png\">&nbsp;", details, "</span>");
            }
            else if (change < 0)
            {
                details = string.Concat("<span><img src=\"/Content/Images/down.png\">&nbsp;", details, "</span>");
            }
            else
            {
                details = string.Concat("<span><img src=\"/Content/Images/nochange.png\">&nbsp;", details, "</span>");
            }

            return details;
        }

    }
}
