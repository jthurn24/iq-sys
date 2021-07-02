using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Domain.Models;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Logging;
using SnyderIS.sCore.Persistence;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;

namespace IQI.Intuition.Infrastructure.Services.Warnings.InfectionType
{
    public class WingService : BaseService
    {

        public override void Run(
            WarningRule rule,
            Facility facility,
            ILog log,
            IStatelessDataContext dataContext)
        {
            foreach( var floor in dataContext.CreateQuery<Floor>().FilterBy(x => x.Facility.Id == facility.Id).FetchAll())
            {
                foreach (var wing in dataContext.CreateQuery<Wing>().FilterBy(x => x.Floor.Id == floor.Id).FetchAll())
                {
                    EvaluateWing(rule,facility,wing,dataContext,log);
                }
            }
        }
        public override string DescribeRuleType(WarningRule rule, IDataContext dataContext)
        {
            return "Infection Type: Wing";
        }

        public override IDictionary<string, string> DescribeArguments(WarningRule rule, IDataContext dataContext)
        {
            var args = new Dictionary<string, string>();

            args["Days"] = rule.ParsedArguments["Days"];
            args["Threshold"] = rule.ParsedArguments["Threshold"];

            var classificationBuilder = new StringBuilder();
            foreach (var classification in rule.ParsedArguments["Classifications"].Split("|".ToCharArray()))
            {
                classificationBuilder.Append("(");
                classificationBuilder.Append(System.Enum.GetName(typeof(IQI.Intuition.Domain.Models.InfectionClassification), Convert.ToInt32(classification)).SplitPascalCase());
                classificationBuilder.Append(")");
            }
            args["Infection Classification"] = classificationBuilder.ToString();


            var typeBuilder = new StringBuilder();
            foreach (var infectionType in rule.ParsedArguments["InfectionTypes"].Split("|".ToCharArray()))
            {
                typeBuilder.Append("(");
                typeBuilder.Append(dataContext.Fetch<Domain.Models.InfectionType>(Convert.ToInt32(infectionType)).Name);
                typeBuilder.Append(")");
            }
            args["Infection Types"] = typeBuilder.ToString();

            return args;
        }


        protected void EvaluateWing(
            WarningRule rule,
            Facility facility,
            Wing wing,
            IStatelessDataContext dataContext,
            ILog log
            )
        {
            int days = Convert.ToInt32(rule.ParsedArguments["Days"]);
            int threshold = Convert.ToInt32(rule.ParsedArguments["Threshold"]);

            var classifications = new List<int>();
            var infectionTypes = new List<int>();

            var descriptionTokens = new Dictionary<string, string>();
            descriptionTokens["Wing"] = wing.Name;

            foreach (var value in rule.ParsedArguments["Classifications"].Split("|".ToCharArray()))
            {
                classifications.Add(Convert.ToInt32(value));
            }

            foreach (var value in rule.ParsedArguments["InfectionTypes"].Split("|".ToCharArray()))
            {
                infectionTypes.Add(Convert.ToInt32(value));
            }

            var infections = dataContext.CreateQuery<InfectionVerification>()
                .FilterBy(x => x.Patient.Room.Wing.Id == wing.Id)
                .FilterBy(x => classifications.Contains((int)x.Classification))
                .FilterBy(x => infectionTypes.Contains(x.InfectionSite.Type.Id))
                .FilterBy(x => x.Deleted == null || x.Deleted == false)
                .FetchAll().OrderBy(x => x.FirstNotedOn);


            foreach (var infection in infections)
            {
                if(infection.FirstNotedOn.HasValue)
                {
                    var scanStartDate = infection.FirstNotedOn.Value.AddDays( 0 - days);
                    var scanEndDate = infection.FirstNotedOn.Value;

                    var applicableInfections = infections.Where(x => x.FirstNotedOn >= scanStartDate && x.FirstNotedOn <= scanEndDate);

                    if (applicableInfections.Count() >= threshold)
                    {
                        if (!WarningExists(dataContext,rule,facility.Id, null, infection.FirstNotedOn.Value))
                        {
                            var itemTokens = new List<Dictionary<string, string>>();

                            foreach (var applicableInfection in applicableInfections)
                            {
                                var patient = dataContext.Fetch<Patient>(applicableInfection.Patient.Id);
                                var infectionSite = dataContext.Fetch<InfectionSite>(applicableInfection.InfectionSite.Id);
                                var infectionType = dataContext.Fetch<IQI.Intuition.Domain.Models.InfectionType>(infectionSite.Type.Id);

                                var itemToken = new Dictionary<string, string>();
                                itemToken["PatientName"] = patient.FullName;
                                itemToken["PatientGuid"] = patient.Guid.ToString();
                                itemToken["InfectionId"] = applicableInfection.Id.ToString();
                                itemToken["InfectionNotedOn"] = applicableInfection.FirstNotedOn.Value.ToString("MM/dd/yyyy");
                                itemToken["InfectionType"] = infectionType.Name;

                                itemTokens.Add(itemToken);

                            }

                            AddWarning(dataContext, rule, facility.Id, null, infection.FirstNotedOn.Value, descriptionTokens, itemTokens,log);
                        }
                    }
                }
            }
                
                
        }

    }
}
