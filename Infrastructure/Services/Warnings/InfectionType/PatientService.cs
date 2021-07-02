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
    public class PatientService : BaseService
    {
        public override void Run(
            WarningRule rule, 
            Facility facility, 
            ILog log,
            IStatelessDataContext dataContext)
        {
            foreach (var patient in dataContext.CreateQuery<Patient>()
                .FilterBy(x => x.Room.Wing.Floor.Facility.Id == facility.Id)
                .FilterBy(x => x.Deleted == null || x.Deleted == false)
                .FetchAll())
            {
                EvaluatePatient(rule,facility,patient, dataContext, log);
            }
        }

        public override string DescribeRuleType(WarningRule rule, IDataContext dataContext)
        {
            return "Infection Type: Patient";
        }

        public override IDictionary<string, string> DescribeArguments(WarningRule rule, IDataContext dataContext)
        {
            var args = new Dictionary<string, string>();

            args["Days"] = rule.ParsedArguments["Days"];
            args["Threshold" ] = rule.ParsedArguments["Threshold"];

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

        protected void EvaluatePatient(
            WarningRule rule, 
            Facility facility, 
            Patient patient,
            IStatelessDataContext dataContext,
            ILog log)
        {
            int days = Convert.ToInt32(rule.ParsedArguments["Days"]);
            int threshold = Convert.ToInt32(rule.ParsedArguments["Threshold"]);

            var classifications = new List<int>();
            var infectionTypes = new List<int>();

            var descriptionTokens = new Dictionary<string, string>();


            foreach (var value in rule.ParsedArguments["Classifications"].Split("|".ToCharArray()))
            {
                classifications.Add(Convert.ToInt32(value));
            }

            foreach (var value in rule.ParsedArguments["InfectionTypes"].Split("|".ToCharArray()))
            {
                infectionTypes.Add(Convert.ToInt32(value));
            }

            var infections = dataContext.CreateQuery<InfectionVerification>()
                .FilterBy(x => x.Patient.Id == patient.Id)
                .FilterBy(x => classifications.Contains((int)x.Classification))
                .FilterBy(x => infectionTypes.Contains(x.InfectionSite.Type.Id))
                .FilterBy(x => x.Deleted == null || x.Deleted == false)
                .FetchAll().OrderBy(x => x.FirstNotedOn);


            foreach (var infection in infections)
            {
                if (infection.FirstNotedOn.HasValue)
                {
                    var scanStartDate = infection.FirstNotedOn.Value.AddDays(0 - days);
                    var scanEndDate = infection.FirstNotedOn.Value;


                    var applicableInfections = infections.Where(x => x.FirstNotedOn >= scanStartDate && x.FirstNotedOn <= scanEndDate);

                    if (applicableInfections.Count() >= threshold)
                    {
                        if (!WarningExists(dataContext,rule,null, patient.Id, infection.FirstNotedOn.Value))
                        {
                            var itemTokens = new List<Dictionary<string, string>>();

                            foreach (var applicableInfection in applicableInfections)
                            {
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

                            AddWarning(dataContext, rule, null, patient.Id, infection.FirstNotedOn.Value, descriptionTokens, itemTokens, log);
                        }
                    }
                }
            }


        }
    }
}
