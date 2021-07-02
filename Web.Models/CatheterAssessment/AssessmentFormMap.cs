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
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Domain;

namespace IQI.Intuition.Web.Models.CatheterAssessment
{
    public class AssessmentFormMap : ModelMap<AssessmentForm, Domain.Models.CatheterAssessment>
    {
        protected ICatheterRepository CatheterRespository { get; set; }

        public AssessmentFormMap(
           IActionContext actionContext,
            ICatheterRepository catheterRespository)
        {
            CatheterRespository = catheterRespository;

            ForProperty(model => model.AssessmentDate)
                .Bind(domain => domain.AssessmentDate)
                .AtMostToday("Date")
                .Default(DateTime.Now)
                .Required()
                .DisplayName("Date");

            ForProperty(model => model.Id)
                .Read(domain => domain != null ? domain.Id.ToString() : string.Empty)
            .ReadOnly();

            ForProperty(model => model.TherapyGoal)
                .Bind(domain => domain.TherapyGoal)
                .MultilineText()
                .DisplayName("Goal of therapy/overall tx plan");

            ForProperty(model => model.AlternativeInterventions)
                .Bind(domain => domain.AlternativeInterventions)
                .MultilineText()
                .DisplayName("Alternatice interventions");

            ForProperty(model => model.ReversibleFactors)
                .Bind(domain => domain.ReversibleFactors)
                .MultilineText()
                .DisplayName("Reversible or irreversible factors");

            ForProperty(model => model.NextAssessmentDate)
                .Bind(domain => domain.NextAssessmentDate)
                .AtLeast(DateTime.Now)
                .Required()
                .Default(DateTime.Now.AddDays(30))
                .DisplayName("Next assessment date");

            ForProperty(model => model.ContinuedUseEstimateDays)
                .Bind(domain => domain.ContinuedUseEstimateDays)
                .DisplayName("Estimated time for continued use");

            ForProperty(model => model.TubeHolderUsed)
                .Bind(domain => domain.TubeHolderUsed)
                .DisplayName("Tube holder in use / consistently");

            ForProperty(model => model.CSResults)
                .Bind(domain => domain.CSResults)
                .MultilineText()
                .DisplayName("C & S Results");

            ForProperty(model => model.Action)
                .Bind(x => x.Action)
                    .OnRead(domain => domain)
                    .OnWrite(SelectAction)
                .DropDownList(GenerateAction)
                .Required();

            ForProperty(model => model.ClientData)
                .Map(domain => domain)
                .ReadOnly();

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

            ForProperty(model => model.RemovalReason)
                .Bind(domain => domain.RemovalReason)
                .DisplayName("Reason");

            ForProperty(model => model.RemovalPvr1)
                .Bind(domain => domain.RemovalPvr1)
                .DisplayName("Pvr 1 (ml)");

            ForProperty(model => model.RemovalPvr1Hours)
                .Bind(domain => domain.RemovalPvr1Hours)
                .DisplayName("Hours from removal"); ;

            ForProperty(model => model.RemovalPvr2)
                .Bind(domain => domain.RemovalPvr2)
                .DisplayName("Pvr 2 (ml)");

            ForProperty(model => model.RemovalPvr2Hours)
                .Bind(domain => domain.RemovalPvr2Hours)
                .DisplayName("Hours from removal");

            ForProperty(model => model.RemovalPvr3)
                .Bind(domain => domain.RemovalPvr3)
                .DisplayName("Pvr 3 (ml)");

            ForProperty(model => model.RemovalPvr3Hours)
                .Bind(domain => domain.RemovalPvr3Hours)
                .DisplayName("Hours from removal");

            ForProperty(model => model.RemovalComplications)
                .Bind(domain => domain.RemovalComplications)
                .DisplayName("Complications")
                .MultilineText();


            ForProperty(model => model.RemoveAlternativeTherapeutic)
                .Bind(domain => domain.RemoveAlternativeTherapeutic)
                .DisplayName("Alternative therapeutic interventions attempted:")
                .MultilineText();

            ForProperty(model => model.RemoveIntakeDaily)
                .Bind(domain => domain.RemoveIntakeDaily)
                .DisplayName("Intake Daily");

            ForProperty(model => model.RemoveQualityOfUrine)
                .Bind(domain => domain.RemoveQualityOfUrine)
                .DisplayName("Quality Of Urine");

            ForProperty(model => model.RemoveResidentResponse)
                .Bind(domain => domain.RemoveResidentResponse)
                .DisplayName("Resident Response")
                .MultilineText();

            ForProperty(model => model.RemovedAndReplaced)
                .Bind(domain => domain.RemovedAndReplaced)
                .DisplayName("Replaced?");

            ForProperty(model => model.RemovedAndReplacedReason)
                .Bind(domain => domain.RemovedAndReplacedReason)
                .DisplayName("Replaced Reason"); ;

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

        private int? SelectAction(int? action)
        {
            return action;
        }

        private IEnumerable<SelectListItem> GenerateAction()
        {
            var temp = new List<SelectListItem>();
            temp.Add(new SelectListItem() { Text = "Select...", Value = string.Empty });
            temp.Add(new SelectListItem() { Text = "Continue with catheter", Value = ((int)Enumerations.CatheterAction.Continue).ToString() });
            temp.Add(new SelectListItem() { Text = "Attempt catheter removal", Value = ((int)Enumerations.CatheterAction.Attempt).ToString() });
            return (IEnumerable<SelectListItem>)temp;
        }

    }
}
