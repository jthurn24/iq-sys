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
namespace IQI.Intuition.Infrastructure.Services.Warnings.LabResult
{
    public class WingService : BaseService
    {
        public override string DescribeRuleType(WarningRule rule, IDataContext dataContext)
        {
            return "Lab Result: Wing";
        }

        public override IDictionary<string, string> DescribeArguments(WarningRule rule, IDataContext dataContext)
        {
            var args = new Dictionary<string, string>();

            args["Days"] = rule.ParsedArguments["Days"];
            args["Threshold"] = rule.ParsedArguments["Threshold"];

            if (rule.ParsedArguments.ContainsKey("LabTestTypes"))
            {
                var testTypeBuilder = new StringBuilder();
                foreach (var testType in rule.ParsedArguments["LabTestTypes"].Split("|".ToCharArray()))
                {
                    testTypeBuilder.Append("(");
                    testTypeBuilder.Append(dataContext.Fetch<Domain.Models.LabTestType>(Convert.ToInt32(testType)).Name);
                    testTypeBuilder.Append(")");
                }

                args["Lab Test Types"] = testTypeBuilder.ToString();
            }

            if (rule.ParsedArguments.ContainsKey("Pathogens"))
            {
                var pathogenBuilder = new StringBuilder();
                foreach (var pathogen in rule.ParsedArguments["Pathogens"].Split("|".ToCharArray()))
                {
                    pathogenBuilder.Append("(");
                    pathogenBuilder.Append(dataContext.Fetch<Domain.Models.Pathogen>(Convert.ToInt32(pathogen)).Name);
                    pathogenBuilder.Append(")");
                }
                args["Pathogens"] = pathogenBuilder.ToString();
            }

            return args;
        }

        public override void Run(WarningRule rule, Facility facility, ILog log, IStatelessDataContext dataContext)
        {
            foreach (var floor in dataContext.CreateQuery<Floor>().FilterBy(x => x.Facility.Id == facility.Id).FetchAll())
            {
                foreach (var wing in dataContext.CreateQuery<Wing>().FilterBy(x => x.Floor.Id == floor.Id).FetchAll())
                {
                    EvaluateWing(rule, facility, wing, dataContext,log);
                }
            }
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
 
            IEnumerable<int> matchIds = new List<int>();

            var descriptionTokens = new Dictionary<string, string>();
            descriptionTokens["Wing"] = wing.Name;

            if (rule.ParsedArguments.ContainsKey("LabTestTypes"))
            {
                var testTypes = new List<int>();

                foreach (var value in rule.ParsedArguments["LabTestTypes"].Split("|".ToCharArray()))
                {
                    testTypes.Add(Convert.ToInt32(value));
                }

                matchIds = matchIds.Append(
                        dataContext.CreateQuery<InfectionLabResult>()
                        .FilterBy(x => x.InfectionVerification.Room.Wing.Id == wing.Id)
                        .FilterBy(x => testTypes.Contains(x.LabTestType.Id))
                        .FilterBy(x => x.LabResult.Positive == true)
                        .FilterBy(x => x.InfectionVerification.Deleted == null || x.InfectionVerification.Deleted == false)
                        .FetchAll().Select(x => x.Id).ToArray<int>()
                    );
            }

            if (rule.ParsedArguments.ContainsKey("Pathogens"))
            {
                var pathogensTypes = new List<int>();

                foreach (var value in rule.ParsedArguments["Pathogens"].Split("|".ToCharArray()))
                {
                    pathogensTypes.Add(Convert.ToInt32(value));
                }

                matchIds = matchIds.Append(
                    dataContext.CreateQuery<InfectionLabResultPathogen>()
                    .FilterBy(x => x.InfectionLabResult.InfectionVerification.Room.Wing.Id == wing.Id)
                    .FilterBy(x => pathogensTypes.Contains(x.Pathogen.Id))
                    .FilterBy(x => x.InfectionLabResult.InfectionVerification.Deleted == null || x.InfectionLabResult.InfectionVerification.Deleted == false)
                    .FetchAll().Select(x => x.InfectionLabResult.Id).ToArray<int>()
                );
            }


            matchIds = matchIds.ToList();

            var matchingInfectionIds = dataContext.CreateQuery<InfectionLabResult>()
                .FilterBy(x => matchIds.Contains(x.Id))
                .FetchAll().Select(x => x.InfectionVerification.Id).ToList();

            var infections = dataContext.CreateQuery<InfectionVerification>()
                .FilterBy(x => matchingInfectionIds.Contains(x.Id))
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
                        if (!WarningExists(dataContext, rule, facility.Id, null, infection.FirstNotedOn.Value))
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

                            AddWarning(dataContext, rule, facility.Id, null, infection.FirstNotedOn.Value, descriptionTokens, itemTokens, log);
                        }
                    }
                }
            }


        }
    }
}
