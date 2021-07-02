using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.Extensions;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Extensions;

namespace IQI.Intuition.Web.Models.Infection
{
    public class InfectionFormMap : ModelMap<InfectionForm, InfectionVerification>
    {
        public InfectionFormMap(
            IInfectionRepository infectionRepository,
            ILabResultRepository labResultRepository,
            ITreatmentRepository treatmentRepository,
            IModelMapper modelMapper,
            IActionContext actionContext
            )
        {
            InfectionRepository = infectionRepository.ThrowIfNullArgument("infectionRepository");
            LabResultRepository = labResultRepository.ThrowIfNullArgument("labResultRepository");
            TreatmentRepository = treatmentRepository.ThrowIfNullArgument("treatmentRepository");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");

            AutoFormatDisplayNames();

            //Verify(model => model.SelectedRiskFactors.IsNotNullOrEmpty())
            //    .ErrorMessage("Please select at least one Risk Factor");
            

            ForProperty(model => model.InfectionVerificationId)
                .Read(domain => domain.Id)
                .Exclude(On.Create)
                .HiddenInput();

            ForProperty(model => model.Guid)
                .Read(domain => domain.Guid);

            ForProperty(model => model.PatientWasHospitalized)
                .Bind(domain => domain.PatientWasHospitalized)
                .DisplayName("Resident hospitalized");

            ForProperty(model => model.NursePractitioner)
                .Bind(domain => domain.NursePractitioner)
                .DisplayName("Nurse Practitioner");

            ForProperty(model => model.FirstNotedOn)
                .Bind(domain => domain.FirstNotedOn)
                .AtMostToday("First Noted On")
                .DisplayName("Noted on")
                .Required();

            ForProperty(model => model.IsResolved)
                .Bind(domain => domain.IsResolved)
                .DisplayName("Resolved?")
                .CheckBoxQuestion();

            ForProperty(model => model.LabsPending)
                .Bind(domain => domain.LabsPending)
                .DisplayName("Labs Pending")
                .CheckBoxQuestion();

            ForProperty(model => model.ResolvedOn)
                .Bind(domain => domain.ResolvedOn)
                .RequiredWhen(model => model.IsResolved, "Resolved On")
                .AtMostToday("Resolved On")
                .DisplayName("On");

            ForProperty(model => model.InfectionType)
                .Read(domain => domain.InfectionSite.Type.Id)
                .Required()
                .DisplayName("Type")
                .DropDownList();

            ForProperty(model => model.InfectionSite)
                .Bind(domain => domain.InfectionSite)
                    .OnRead(site => site.Id)
                    .OnWrite(SelectInfectionSite)
                .Required()
                .DisplayName("Condition")
                .DropDownList();

            ForProperty(model => model.InfectionSiteDetail)
                .Bind(domain => domain.InfectionSiteSupportingDetail)
                    .OnRead(detail => detail.Id)
                    .OnWrite(SelectInfectionSiteDetail);

            ForProperty(model => model.Classification)
                .Bind(domain => domain.Classification)
                .EnumList();

            ForProperty(model => model.SelectedCriteria)
                .Read(domain => domain.Criteria.Select(item => item.Id))
                .Write((domain, value) => domain
                    .AssignCriteria(ConvertCriteriaSelections(value)))
                .Default(Enumerable.Empty<int>());

            ForProperty(model => model.SelectedRiskFactors)
                .Read(domain => domain.RiskFactors.Select(item => item.Id))
                .Write((domain, value) => domain
                    .AssignRiskFactors(ConvertRiskFactorSelections(value)))
                .Default(Enumerable.Empty<int>())
                .HorizontalCheckBoxList(GenerateRiskFactorItems());

            ForProperty(model => model.AdditionalRiskFactors)
                .Bind(domain => domain.AdditionalRiskFactorsText)
                .MultilineText();

            ForProperty(model => model.Treatement)
                .Bind(domain => domain.TreatementText)
                .MultilineText();



            ForProperty(model => model.SpecifiedLabCultureType)
                .Bind(domain => domain.SpecifiedLabCultureType)
                .DisplayName("Culture type");

            ForProperty(model => model.LabCultureType)
                .Bind(domain => domain.LabCultureType)
                .RequiredWhen(model => model.SpecifiedLabCultureType.IsTrue());

            ForProperty(model => model.SpecifiedLabCbc)
                .Bind(domain => domain.SpecifiedLabCbc)
                .DisplayName("CBC");

            ForProperty(model => model.SpecifiedLabOther)
                .Bind(domain => domain.SpecifiedLabOther)
                .DisplayName("Other");

            ForProperty(model => model.LabOtherDetails)
                .Bind(domain => domain.LabOtherDetailsText)
                .RequiredWhen(model => model.SpecifiedLabOther.IsTrue());

            ForProperty(model => model.LabResults)
                .Bind(domain => domain.LabResultsText)
                .DisplayName("Results")
                .MultilineText();

            ForProperty(model => model.HadChestXray)
                .Bind(domain => domain.HadChestXray)
                .DisplayName("Chest X-ray completed?")
                .CheckBoxQuestion();

            ForProperty(model => model.ChestXrayCompletedOn)
                .Bind(domain => domain.ChestXrayCompletedOn)
                .RequiredWhen(model => model.HadChestXray.IsTrue(), "Chest X-ray Completed On")
                .AtMostToday("Chest X-ray Completed On")
                .DisplayName("On");

            ForProperty(model => model.ChestXRayResults)
                .Bind(domain => domain.ChestXRayResultsText)
                .DisplayName("Results")
                .MultilineText();


            ForProperty(model => model.IsUpdateMode)
                .Read(domain => true)
                .HiddenInput();

            ForProperty(model => model.ClientData)
                .Map(domain => domain)
                .ReadOnly();

            ForProperty(model => model.InfectionLabResults)
                .Read(ReadLabResults)
                .Write(WriteLabResults)
                .Default(new List<InfectionLabResult>());

            ForProperty(model => model.InfectionNotes)
                .Read(ReadNotes)
                .Write(WriteNotes)
                .Default(new List<InfectionNote>());

            ForProperty(model => model.InfectionTreatmentTypes)
                .Read(ReadTreatments)
                .Write(WriteTreatments)
                .Default(ReadTreatments(null));

            ForProperty(model => model.Floor)
                .Read(domain => domain.Room.Wing.Floor.Id)
                .DropDownList();

            ForProperty(model => model.Wing)
                .Read(domain => domain.Room.Wing.Id)
                .DropDownList();

            ForProperty(model => model.Room)
                .Bind(domain => domain.Room)
                    .OnRead(room => room.Id)
                    .OnWrite(roomId => SelectFacilityRoom(actionContext.CurrentFacility, roomId))
                .Required()
                .DropDownList();

            ForProperty(model => model.CriteriaLocked)
                .Read(IsCriteriaLocked);

            ForProperty(model => model.MdNotifiedIneligibleCriteria)
                .Bind(domain => domain.MdNotifiedIneligibleCriteria);
            
        }
        
        private IInfectionRepository InfectionRepository { get; set; }
        private ILabResultRepository LabResultRepository { get; set; }
        private ITreatmentRepository TreatmentRepository { get; set; }
        private IModelMapper ModelMapper { get; set; }

        private bool IsCriteriaLocked(InfectionVerification domain)
        {
            if (domain.InfectionSite.Type.IsHidden == true)
            {
                return true;
            }

            var activeRuleSets = InfectionRepository.AllInfectionSites.Select(x => x.RuleSet);

            foreach (var criteria in domain.Criteria)
            {
                if (!IsRuleSetActive(criteria.Rule.RuleSet,activeRuleSets))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsRuleSetActive(InfectionCriteriaRuleSet ruleSet, IEnumerable<InfectionCriteriaRuleSet> activeRuleSets)
        {
            if (ruleSet.ParentCriteriaRuleSet != null)
            {
                return IsRuleSetActive(ruleSet.ParentCriteriaRuleSet, activeRuleSets);
            }

            if (activeRuleSets.Where(x => x.Id == ruleSet.Id).Count() < 1)
            {
                return false;
            }

            return true;
        }

        private IList<InfectionNote> ReadNotes(InfectionVerification domain)
        {
            var results = new List<InfectionNote>();

            foreach (var note in domain.InfectionNotes.OrderByDescending(x => x.CreatedAt))
            {
                var result = new InfectionNote();
                result.CreatedBy = note.CreatedBy;
                result.CreatedOn = note.CreatedAt.HasValue ? note.CreatedAt.Value.ToString("MM/dd/yy") : string.Empty;
                result.Note = note.Note;
                result.InfectionNoteId = note.Id;
                result.Removed = false;
                results.Add(result);
            }

            return results;
        }

        private void WriteNotes(InfectionVerification domain, IEnumerable<InfectionNote> results)
        {
            if (results == null)
            {
                return;
            }

            foreach (var note in results)
            {
                Domain.Models.InfectionNote domainNote = null;

                if (note.InfectionNoteId.HasValue)
                {
                    if (note.Removed)
                    {
                        domain.InfectionNotes.Remove(x => x.Id == note.InfectionNoteId.Value);
                    }
                    else
                    {
                        domainNote = domain.InfectionNotes.Where(x => x.Id == note.InfectionNoteId.Value).FirstOrDefault();
                    }
                }
                else
                {
                    if (note.Removed == false)
                    {
                        domainNote = new Domain.Models.InfectionNote();
                        domainNote.InfectionVerification = domain;
                        domain.InfectionNotes.Add(domainNote);
                    }
                }

                if (domainNote != null)
                {
                    domainNote.Note = note.Note;
                }
       
            }

        }

        private IList<InfectionLabResult> ReadLabResults(InfectionVerification domain)
        {
            var results = new List<InfectionLabResult>();

            foreach (var d in domain.LabResults)
            {
                var m = this.ModelMapper.MapForReadOnly<InfectionLabResult>(d);

                m.Pathogens = new List<InfectionLabResultPathogen>();

                foreach (var p in d.ResultPathogens)
                {
                    var pp = new InfectionLabResultPathogen();
                    pp.PathogenId = p.Pathogen.Id;
                    pp.PathogenName = p.Pathogen.Name;
                    pp.Id = p.Id;
                    pp.Removed = false;

                    if (p.PathogenQuantity != null)
                    {
                        pp.PathogenQuantityName = p.PathogenQuantity.Name;
                        pp.PathogenQuantityId = p.PathogenQuantity.Id;
                    }
                    else
                    {
                        pp.PathogenQuantityName = string.Empty;
                    }
                    m.Pathogens.Add(pp);
                }

                var test = LabResultRepository.AllTestTypes.Where(x => x.Id == d.LabTestType.Id).FirstOrDefault();
                var result = LabResultRepository.AllResults.Where(x => x.Id == d.LabResult.Id).FirstOrDefault();

                if (result.Positive && test.PathogenSet != null)
                {
                    m.PathogenOptions = test.PathogenSet.Pathogens.Select(x => new Web.Models.Option() { Id = x.Id, Name = x.Name }).ToList();
                }
                else
                {
                    m.PathogenOptions = new List<Web.Models.Option>();
                }

                if (result.Positive && test.PathogenQuantitySet != null)
                {
                    m.PathogenQuantityOptions = test.PathogenQuantitySet.PathogenQuantities.Select(x => new Web.Models.Option() { Id = x.Id, Name = x.Name }).ToList();
                }
                else
                {
                    m.PathogenQuantityOptions = new List<Web.Models.Option>();
                }


                results.Add(m);
            }

            return results;
        }

        private void WriteLabResults(InfectionVerification domain, IEnumerable<InfectionLabResult> results)
        {
            if (results == null)
            {
                return;
            }

            if (domain.LabResults == null)
            {
                domain.LabResults = new List<Domain.Models.InfectionLabResult>();
            }

            foreach (var result in results)
            {
                if (result.Notes == null)
                {
                    result.Notes = string.Empty;
                }

                Domain.Models.InfectionLabResult domainResult = null;
                if (result.InfectionLabResultId.HasValue)
                {
                    if (result.Removed)
                    {
                        InfectionRepository.Delete(domain.LabResults.Where(x => x.Id == result.InfectionLabResultId.Value).FirstOrDefault());
                        domain.LabResults.Remove(x => x.Id == result.InfectionLabResultId.Value);
                    }
                    else
                    {
                        domainResult = domain.LabResults.Where(x => x.Id == result.InfectionLabResultId).FirstOrDefault();                       
                    }
                }
                else
                {
                    if (result.Removed == false)
                    {
                        domainResult = new Domain.Models.InfectionLabResult();
                        domainResult.InfectionVerification = domain;
                        domainResult.LabResult = LabResultRepository.AllResults.Where(x => x.Id == result.LabResultId).FirstOrDefault();
                        domainResult.LabTestType = LabResultRepository.AllTestTypes.Where(x => x.Id == result.LabTestTypeId).FirstOrDefault();

                        if (result.LabCompletedOn.IsNullOrEmpty())
                        {
                            domainResult.CompletedOn = null;
                        }
                        else
                        {
                            domainResult.CompletedOn = DateTime.Parse(result.LabCompletedOn);
                        }

                        domain.LabResults.Add(domainResult);
                    }
                }


                // *** update pathogens if required ****//

                if(domainResult != null)
                {
                    domainResult.Notes = result.Notes;

                    if (result.Pathogens != null)
                    {
                        foreach (var pathogenResult in result.Pathogens)
                        {
                            Domain.Models.InfectionLabResultPathogen pathogenDomain = null;

                            if (domainResult.ResultPathogens == null)
                            {
                                domainResult.ResultPathogens = new List<Domain.Models.InfectionLabResultPathogen>();
                            }

                            if (pathogenResult.Id.HasValue)
                            {
                                if (pathogenResult.Removed)
                                {
                                    InfectionRepository.Delete(domainResult.ResultPathogens.Where(x => x.Id == pathogenResult.Id.Value).FirstOrDefault());
                                    domainResult.ResultPathogens.Remove(x => x.Id == pathogenResult.Id.Value);
                                }
                            }
                            else
                            {
                                if (pathogenResult.Removed == false)
                                {
                                    pathogenDomain = new Domain.Models.InfectionLabResultPathogen();
                                    pathogenDomain.InfectionLabResult = domainResult;
                                    pathogenDomain.Pathogen = LabResultRepository.AllPathogens.Where(x => x.Id == pathogenResult.PathogenId).FirstOrDefault();

                                    if (pathogenResult.PathogenQuantityId.HasValue)
                                    {
                                        pathogenDomain.PathogenQuantity = LabResultRepository.AllPathogenQuantities.Where(x => x.Id == pathogenResult.PathogenQuantityId.Value).FirstOrDefault();
                                    }

                                    domainResult.ResultPathogens.Add(pathogenDomain);
                                }
                            }
                          
                        }
                    }
                
                }

            }
        }

        private Room SelectFacilityRoom(Facility facility, int? roomId)
        {
            if (!roomId.HasValue)
            {
                return null;
            }

            return facility.Floors
                .SelectMany(floor => floor.Wings)
                .SelectMany(wing => wing.Rooms)
                .SingleOrDefault(room => room.Id == roomId);
        }

        private IList<InfectionTreatmentType> ReadTreatments(InfectionVerification domain)
        {
            var results = new List<InfectionTreatmentType>();

            foreach (var treatmentType in TreatmentRepository.AllTreatmentTypes)
            {
                var type = new InfectionTreatmentType();
                type.Name = treatmentType.Name;
                type.Id = treatmentType.Id;
                type.TreatmentNames = treatmentType.Treatments.Select(x => x.Name).ToList();
                type.Treatments = new List<InfectionTreatment>();

                if (domain != null)
                {
                    foreach (var t in domain.Treatments.Where(x => x.TreatmentType.Id == treatmentType.Id).OrderBy(x => x.AdministeredOn))
                    {
                        type.Treatments.Add(this.ModelMapper.MapForReadOnly<InfectionTreatment>(t));
                    }
                }

                results.Add(type);

            }

            return results;
        }

        private void WriteTreatments(InfectionVerification domain, IEnumerable<InfectionTreatmentType> results)
        {
            if (results == null)
            {
                return;
            }

            if (domain.Treatments == null)
            {
                domain.Treatments = new List<Domain.Models.InfectionTreatment>();
            }

            foreach (var treatmentType in results)
            {
                if (treatmentType.Treatments != null)
                {
                    foreach (var treatment in treatmentType.Treatments)
                    {
                        Domain.Models.InfectionTreatment domainTreatment = null;


                        if (treatment.Id.HasValue)
                        {
                            if (treatment.Removed)
                            {
                                InfectionRepository.Delete(domain.Treatments.Where(x => x.Id == treatment.Id.Value).FirstOrDefault());
                                domain.Treatments.Remove(x => x.Id == treatment.Id.Value);
                            }
                            else
                            {
                                domainTreatment = domain.Treatments.Where(x => x.Id == treatment.Id.Value).FirstOrDefault();
                            }
                        }
                        else
                        {
                            if (treatment.Removed == false)
                            {
                                domainTreatment = new Domain.Models.InfectionTreatment();
                                domainTreatment.TreatmentType = TreatmentRepository.AllTreatmentTypes.Where(x => x.Id == treatmentType.Id).FirstOrDefault();
                                domainTreatment.InfectionVerification = domain;
                                domain.Treatments.Add(domainTreatment);
                            }
                        }

                        if (domainTreatment != null)
                        {
                            domainTreatment.TreatmentName = treatment.TreatmentName.ToStringSafely();
                            domainTreatment.DeliveryMethod = treatment.DeliveryMethod.ToStringSafely();
                            domainTreatment.Dosage = treatment.Dosage.ToStringSafely();
                            domainTreatment.Frequency = treatment.Frequency.ToStringSafely();
                            domainTreatment.SpecialInstructions = treatment.SpecialInstructions.ToStringSafely();
                            domainTreatment.Duration = treatment.Duration.ToStringSafely();
                            domainTreatment.Units = treatment.Units.ToStringSafely();
                            domainTreatment.MDName = treatment.MDName.ToStringSafely();

                            DateTime treatmentDate;
                            if (DateTime.TryParse(treatment.AdministeredOn, out treatmentDate))
                            {
                                domainTreatment.AdministeredOn = treatmentDate;
                            }


                            if (treatment.DiscontinuedOn.IsNullOrEmpty())
                            {
                                domainTreatment.DiscontinuedOn = null;
                            }
                            else
                            {
                                DateTime discontinuedDate;
                                if (DateTime.TryParse(treatment.DiscontinuedOn, out discontinuedDate))
                                {
                                    domainTreatment.DiscontinuedOn = discontinuedDate;
                                }
                            }

                        }
                    }
                }
            }
           
        }

        private InfectionSite SelectInfectionSite(int? siteId)
        {
            return InfectionRepository.AllInfectionSites
                .Single(item => item.Id == siteId);
        }

        private InfectionSiteSupportingDetail SelectInfectionSiteDetail(int? detailId)
        {
            if (detailId.HasValue)
            {
                return InfectionRepository.AllInfectionSites
                    .SelectMany(item => item.SupportingDetails)
                    .Single(item => item.Id == detailId);
            }

            return null;
        }

        private InfectionCriteria[] ConvertCriteriaSelections(IEnumerable<int> selections)
        {
            return InfectionRepository.AllInfectionCriteria
                .Where(item => selections
                    .EmptyIfNull()
                    .Contains(item.Id))
                .ToArray();
        }

        private IEnumerable<SelectListItem> GenerateRiskFactorItems()
        {
            return InfectionRepository.AllRiskFactors
                .ToSelectListItems(
                    item => item.Name,
                    item => item.Id);
        }

        private InfectionRiskFactor[] ConvertRiskFactorSelections(IEnumerable<int> selections)
        {
            return InfectionRepository.AllRiskFactors
                .Where(item => selections
                    .EmptyIfNull()
                    .Contains(item.Id))
                .ToArray();
        }

    }
}
