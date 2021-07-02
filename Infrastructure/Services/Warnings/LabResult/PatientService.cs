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
    public class PatientService : BaseService
    {
        public override string DescribeRuleType(WarningRule rule, IDataContext dataContext)
        {
            return "Lab Result: Patient";
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
            foreach (var patient in dataContext.CreateQuery<Patient>()
                .FilterBy(x => x.Room.Wing.Floor.Facility.Id == facility.Id)
                .FilterBy(x => x.Deleted == null || x.Deleted == false)
                .FetchAll())
            {
                EvaluatePatient(rule, facility, patient, dataContext,log);
            }
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

            IEnumerable<int> matchIds = new List<int>();

            var descriptionTokens = new Dictionary<string, string>();

            if (rule.ParsedArguments.ContainsKey("LabTestTypes"))
            {
                var testTypes = new List<int>();

                foreach (var value in rule.ParsedArguments["LabTestTypes"].Split("|".ToCharArray()))
                {
                    testTypes.Add(Convert.ToInt32(value));
                }

                matchIds = matchIds.Append(
                        dataContext.CreateQuery<InfectionLabResult>()
                        .FilterBy(x => x.InfectionVerification.Patient.Id == patient.Id)
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
                    .FilterBy(x => x.InfectionLabResult.InfectionVerification.Patient.Id == patient.Id)
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
                        if (!WarningExists(dataContext, rule, null, patient.Id, infection.FirstNotedOn.Value))
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
