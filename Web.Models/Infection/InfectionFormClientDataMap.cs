using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Extensions.Formatting;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;

namespace IQI.Intuition.Web.Models.Infection
{
    // This map is used to populate the ClientData model via DI, but we still need to reference a "real" domain model
    public class InfectionFormClientDataMap : ModelMap<InfectionFormClientData, InfectionVerification>
    {
        public InfectionFormClientDataMap(
            IInfectionRepository infectionRepository,
            ILabResultRepository labResultRepository,
            IActionContext actionContext)
        {
            InfectionRepository = infectionRepository.ThrowIfNullArgument("infectionRepository");
            LabResultRepository = labResultRepository.ThrowIfNullArgument("labResultRepository");
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");


            ForProperty(model => model.Floors)
               .Assign(() => actionContext.CurrentFacility.Floors
                   .Select(floor => new
                   {
                       Text = floor.Name,
                       Value = floor.Id
                   })
                   .ToArray());

            ForProperty(model => model.Wings)
                .Assign(() => actionContext.CurrentFacility.Floors
                    .SelectMany(floor => floor.Wings)
                    .Select(wing => new
                    {
                        Text = wing.Name,
                        Value = wing.Id,
                        Floor = wing.Floor.Id
                    })
                    .ToArray());

            ForProperty(model => model.Rooms)
                .Assign(() => actionContext.CurrentFacility.Floors
                    .SelectMany(floor => floor.Wings)
                    .SelectMany(wing => wing.Rooms)
                    .OrderBy(x => x.IsInactive).ThenBy(x => x.Name)
                    .Select(room => new 
                    {
                        Text = room.IsInactive == true ? string.Concat("(inactive) ", room.Name) : room.Name,
                        Value = room.Id,
                        Wing = room.Wing.Id
                    })
                    .ToArray());


            /* Remove this and do at the controller level. This has to be set based on active def set for facility
             * or the set used at the time of creation 

            ForProperty(model => model.InfectionTypes)
                .Assign(() => InfectionRepository
                    .AllInfectionTypes
                    .Where(x => x.IsHidden != true)
                    .Where(x => x.Definition.Id == ActionContext.CurrentFacility.InfectionDefinition.Id)
                    .Select(type => new 
                    {
                        Text = type.Name, 
                        Value = type.Id
                    })
                    .ToArray());
            */

            ForProperty(model => model.UnSatisfiedRuleClassifications)
                .Assign(() => new []
                {
                    new { 
                         Text = System.Enum.GetName(typeof(InfectionClassification),InfectionClassification.NoInfection).SplitPascalCase() ,
                         Value = System.Enum.GetName(typeof(InfectionClassification),InfectionClassification.NoInfection),
                         Warning = String.Empty
                    },
                    new { 
                         Text = System.Enum.GetName(typeof(InfectionClassification),InfectionClassification.AdmissionHospitalDiagnosed).SplitPascalCase() ,
                         Value = System.Enum.GetName(typeof(InfectionClassification),InfectionClassification.AdmissionHospitalDiagnosed),
                         Warning = String.Empty
                    }
                });

            ForProperty(model => model.SatisfiedRuleClassifications)
                .Assign(() => new[]
                {
                    new { 
                         Text = System.Enum.GetName(typeof(InfectionClassification),InfectionClassification.Admission).SplitPascalCase() ,
                         Value = System.Enum.GetName(typeof(InfectionClassification),InfectionClassification.Admission),
                         Warning = String.Empty
                    },
                    new { 
                         Text = System.Enum.GetName(typeof(InfectionClassification),InfectionClassification.HealthCareAssociatedInfection).SplitPascalCase() ,
                         Value = System.Enum.GetName(typeof(InfectionClassification),InfectionClassification.HealthCareAssociatedInfection),
                         Warning = String.Empty
                    }
                    ,
                    new { 
                         Text = System.Enum.GetName(typeof(InfectionClassification),InfectionClassification.NoInfection).SplitPascalCase() ,
                         Value = System.Enum.GetName(typeof(InfectionClassification),InfectionClassification.NoInfection),
                         Warning = "Only select \"No Infection\" option if subsequent clinical information indicates this does not qualify as an infection."
                    }
                });

            
            /*
            ForProperty(model => model.InfectionSites)
                .Assign(() => GenerateInfectionSites(
                    InfectionRepository
                    .AllInfectionSites
                    .Where(x => x.Type != null && x.Type.Definition.Id ==  ActionContext.CurrentFacility.InfectionDefinition.Id )));
            */
             
            ForProperty( model => model.LabTestTypes)
            .Assign( () => LabResultRepository.AllTestTypes.OrderBy(x => x.Name)
                .Select( type => new 
                    {
                        Name = type.Name,
                        Id = type.Id
                    })
                    .ToArray());


        }

        private IInfectionRepository InfectionRepository { get; set; }
        private ILabResultRepository LabResultRepository { get; set; }
        private IActionContext ActionContext { get; set; }

 


    }
}