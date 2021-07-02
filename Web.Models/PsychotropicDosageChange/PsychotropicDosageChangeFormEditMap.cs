using System;
using System.Collections.Generic;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.Extensions;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using System.Linq;
using System.Web.Mvc;
using IQI.Intuition.Web.Models.Extensions;
using IQI.Intuition.Domain.Services.Psychotropic;

namespace IQI.Intuition.Web.Models.PsychotropicDosageChange
{
    public class PsychotropicDosageChangeFormEditMap : ModelMap<PsychotropicDosageChangeFormEdit, Domain.Models.PsychotropicDosageChange>
    {
        protected IPsychotropicRespository PsychotropicRespository { get; set; }

        public PsychotropicDosageChangeFormEditMap(
            IPsychotropicRespository psychotropicRespository)
        {

            PsychotropicRespository = psychotropicRespository;

            ForProperty(model => model.Frequency)
            .Bind(domain => domain.Frequency)
                .OnRead(domain => domain.Id)
                .OnWrite(SelectFrequency)
            .DropDownList(GenerateFrequencies)
            .Required();

            ForProperty(model => model.StartDate)
                .Bind(domain => domain.StartDate)
                .Required()
                .DisplayName("Change Date");

            ForProperty(model => model.Id)
                .Bind(domain => domain.Id);

            ForProperty(model => model.Segments)
                .Write(WriteSegments)
                .Read(ReadSegments)
                .Verify(VerifySegments)
                .ErrorMessage("The dosages specified are not valid");

        }


        private PsychotropicFrequency SelectFrequency(int? id)
        {
            if (!id.HasValue)
            {
                return null;
            }

            return PsychotropicRespository.AllFrequencies
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }

        private IEnumerable<SelectListItem> GenerateFrequencies()
        {
            return PsychotropicRespository.AllFrequencies
                .ToSelectListItems(
                    item => item.Name,
                    item => item.Id);
        }

        private void WriteSegments(Domain.Models.PsychotropicDosageChange change, PsychotropicDosageChangeFormEdit form, IEnumerable<DosageSegmentEntry> segments)
        {

            var s = segments.EmptyIfNull().Select(x => new DosageSegment()
            {
                Description = x.Description,
                ID = x.ID,
                Dosage = x.Dosage,
                Label = x.Label,
                DescriptionOptions = x.DescriptionOptions
            });

            change.DosageSegments = PsychotropicRespository
                .AllFrequencies.Where(x => x.Id == form.Frequency.Value)
                .First()
                .GetFrequencyDefinition()
                .WriteSegments(s);

        }

        private IEnumerable<DosageSegmentEntry> ReadSegments(Domain.Models.PsychotropicDosageChange change)
        {

            var segments = change.Frequency.GetFrequencyDefinition().ReadSegments(change.DosageSegments);

            if (segments == null)
            {
                return null;
            }

            return segments.Select(x => new DosageSegmentEntry()
            {
                Description = x.Description,
                ID = x.ID,
                Dosage = x.Dosage,
                Label = x.Label,
                DescriptionOptions = x.DescriptionOptions
            });
        }

        private bool VerifySegments(IEnumerable<DosageSegmentEntry> segments)
        {
            foreach (var segment in segments.EmptyIfNull())
            {
                if (segment.Dosage.HasValue == false)
                {
                    return false;
                }

                if (segment.Description.IsNullOrEmpty())
                {
                    return false;
                }
            }

            return true;
        }

    }
}
