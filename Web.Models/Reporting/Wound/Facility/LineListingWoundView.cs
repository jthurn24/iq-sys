using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using IQI.Intuition.Domain.Models;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Extensions.IO;
using RedArrow.Framework.Extensions.Common;
using IQI.Intuition.Reporting.Graphics;

namespace IQI.Intuition.Web.Models.Reporting.Wound.Facility
{
    public class LineListingWoundView
    {
        public const int MODE_ACTIVE_ONLY = 0;
        public const int MODE_ALL = 1;
        public const int MODE_INACTIVE_ONLY = 2;

        public IEnumerable<SelectListItem> WingOptions { get; set; }
        public int? Wing { get; set; }

        public IEnumerable<SelectListItem> FloorOptions { get; set; }
        public int? Floor { get; set; }

        public IEnumerable<SelectListItem> TypeOptions { get; set; }
        public int? Type { get; set; }

        public IEnumerable<SelectListItem> ClassificationOptions { get; set; }
        public int? Classification { get; set; }

        public IEnumerable<SelectListItem> StageOptions { get; set; }
        public int? CurrentStage { get; set; }

        public int? MostRecentProgress { get; set; }
        public IEnumerable<SelectListItem> MostRecentProgressOptions { get; set; }


        public int Mode { get; set; }
        public IEnumerable<SelectListItem> ModeOptions { get; set; }

        


        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public IList<WoundRow> Rows { get; set; }


        public void SetData(IEnumerable<WoundReport> WoundData, int? progressFilter, IEnumerable<PatientPrecaution> precautionData)
        {
            Rows = new List<WoundRow>();

            foreach (var item in WoundData)
            {
                var row = new WoundRow(item, precautionData.Where(x => x.Patient.Id == item.Patient.Id));

                if (progressFilter.HasValue)
                {
                    if ((Domain.Enumerations.WoundProgress)progressFilter == row.RecentProgress)
                    {
                        Rows.Add(row);
                    }
                }
                else
                {
                    Rows.Add(row);
                }
                
            }
        }

        public class WoundRow
        {
            public string ID { get; set; }

            public string Type { get; set; }

            public string IntialAssessment { get; set; }
            public string LastAssessment { get; set; }
            public string Resolved { get; set; }

            public string PatientName { get; set; }
            public string PatientBirthDate { get; set; }
            public string CurrentRoom { get; set; }
            public string CurrentWing { get; set; }
            public string CurrentFloor { get; set; }
            public string AdmissionDate { get; set; }


            public string CurrentStage { get; set; }
            public string Classification { get; set; }
            public string MaxStage { get; set; }
            public string MinStage { get; set; }

            public string AdditionalSiteDetails { get; set; }

            public IEnumerable<string> Precautions { get; set; }

            public string SiteName { get; set; }
            public int? LocationX { get; set; }
            public int? LocationY { get; set; }

            public Domain.Enumerations.WoundProgress? RecentProgress { get; set; }

            public SeriesLineChart PushChart { get; set; }

            public WoundRow(WoundReport report,IEnumerable<PatientPrecaution> precautions)
            {



                PushChart = new SeriesLineChart();

                foreach (var assessment in report.Assessments.OrderBy(x => x.AssessmentDate))
                {
                    PushChart.AddItem(new SeriesLineChart.Item()
                    {
                        Category = assessment.AssessmentDate.Value.ToString("MM/dd"),
                        Series = "Score",
                        Value = assessment.PushScore.HasValue ? assessment.PushScore.Value : 0
                    });
                }

                ID = report.Id.ToString();

                Type = report.WoundType.Name;
                PatientName = report.Patient.FullName;
                CurrentRoom = report.Patient.Room.Name;
                CurrentWing = report.Patient.Room.Wing.Name;
                CurrentFloor = report.Patient.Room.Wing.Floor.Name;
                PatientBirthDate = report.Patient.BirthDate.FormatAsShortDate();
                var lastAdmitDate = report.Patient.GetLastAdmissionDate();
                AdmissionDate = lastAdmitDate != null ? lastAdmitDate.Value.ToString("MM/dd/yy") : string.Empty;

                var assessments = report.Assessments.OrderBy(x => x.AssessmentDate);

                var firstAssessment = assessments.First();
                var lastAssessment = assessments.Last();

                IntialAssessment = string.Concat("Date: ",
                    firstAssessment.AssessmentDate.FormatAsShortDate());


                if (assessments.Count() < 1)
                {
                    LastAssessment = "N/A";
                }
                else
                {

                    var assessmentBuilder = new System.Text.StringBuilder();

                    if (lastAssessment.PainManagedWith.IsNotNullOrEmpty())
                    {
                        assessmentBuilder.AppendFormat("<div style='padding-top:5px;padding-bottom:5px;'>Pain Managed With: {0} </div>",
                                    lastAssessment.PainManagedWith);
                    }

                    assessmentBuilder.AppendFormat("<div class='woundLineListingEntry'><div style='font-weight:bold;'>On</div> {0} </div>", lastAssessment.AssessmentDate.FormatAsShortDate());

                    assessmentBuilder.AppendFormat("<div class='woundLineListingEntry'><div style='font-weight:bold;'>LxWxD</div> {0}x{1}x{2} ",
                        lastAssessment.Lcm.HasValue ? lastAssessment.Lcm.Value.ToString("#.##") : string.Empty,
                        lastAssessment.Wcm.HasValue ? lastAssessment.Wcm.Value.ToString("#.##") : string.Empty,
                        lastAssessment.Dcm.HasValue ? lastAssessment.Dcm.Value.ToString("#.##") : string.Empty);

                    if (lastAssessment.Superficial.HasValue && lastAssessment.Superficial.Value)
                    {
                        assessmentBuilder.Append("<div>[Superficial]</div>");
                    }

                    assessmentBuilder.Append("</div>");

                    if (lastAssessment.Undermining1Depth.HasValue && lastAssessment.Undermining1From.HasValue && lastAssessment.Undermining1To.HasValue)
                    {
                        assessmentBuilder.AppendFormat("<div class='woundLineListingEntry'><div style='font-weight:bold;'>Undermining Area 1</div> {0} o'clock to {1} o'clock {2} cm deep  </div>", 
                            lastAssessment.Undermining1To.Value.ToString("#.#"),
                            lastAssessment.Undermining1From.Value.ToString("#.#"),
                            lastAssessment.Undermining1Depth.Value.ToString("#.##"));
                    }

                    if (lastAssessment.Undermining2Depth.HasValue && lastAssessment.Undermining2From.HasValue && lastAssessment.Undermining2To.HasValue)
                    {
                        assessmentBuilder.AppendFormat("<div class='woundLineListingEntry'><div style='font-weight:bold;'>Undermining Area 2</div> {0} o'clock to {1} o'clock {2} cm deep  </div>",
                            lastAssessment.Undermining2To.Value.ToString("#.#"),
                            lastAssessment.Undermining2From.Value.ToString("#.#"),
                            lastAssessment.Undermining2Depth.Value.ToString("#.##"));
                    }

                    if (lastAssessment.Tunnel1Depth.HasValue && lastAssessment.Tunnel1Location.HasValue)
                    {
                        assessmentBuilder.AppendFormat("<div class='woundLineListingEntry'><div style='font-weight:bold;'>Tunnel 1</div> {0} o'clock &amp; {1} cm deep </div>",
                            lastAssessment.Tunnel1Location.Value.ToString("#.#"),
                            lastAssessment.Tunnel1Depth.Value.ToString("#.##"));
                    }

                    if (lastAssessment.Tunnel2Depth.HasValue && lastAssessment.Tunnel2Location.HasValue)
                    {
                        assessmentBuilder.AppendFormat("<div class='woundLineListingEntry'><div style='font-weight:bold;'>Tunnel 2</div> {0} o'clock &amp; {1} cm deep </div>",
                            lastAssessment.Tunnel2Location.Value.ToString("#.#"),
                            lastAssessment.Tunnel2Depth.Value.ToString("#.##"));
                    }


                    if (lastAssessment.Tunnel3Depth.HasValue && lastAssessment.Tunnel3Location.HasValue)
                    {
                        assessmentBuilder.AppendFormat("<div class='woundLineListingEntry'><div style='font-weight:bold;'>Tunnel 3</div> {0} o'clock &amp; {1} cm deep </div>",
                            lastAssessment.Tunnel3Location.Value.ToString("#.#"),
                            lastAssessment.Tunnel3Depth.Value.ToString("#.##"));
                    }


                    if (lastAssessment.Exudate != Domain.Enumerations.WoundExudate.None)
                    {

                        assessmentBuilder.AppendFormat("<div class='woundLineListingEntry'><div style='font-weight:bold;'>Exudate</div> {0} </div>",
                                System.Enum.GetName(typeof(Domain.Enumerations.WoundExudate), lastAssessment.Exudate).SplitPascalCase());
                    }


                    if (lastAssessment.ExudateType != Domain.Enumerations.WoundExudateType.None)
                    {
                        assessmentBuilder.AppendFormat("<div class='woundLineListingEntry'><div style='font-weight:bold;'>Exudate Type</div> {0} </div>",
                                System.Enum.GetName(typeof(Domain.Enumerations.WoundExudateType), lastAssessment.ExudateType).SplitPascalCase());
                    }

                    if (lastAssessment.WoundBedEpithelial.HasValue)
                    {
                        assessmentBuilder.AppendFormat("<div class='woundLineListingEntry'><div style='font-weight:bold;'>Wound Bed Epithelial</div> {0} </div>",
                            lastAssessment.WoundBedEpithelial.Value.ToString("#.##"));    
                    }

                    if (lastAssessment.WoundBedGranulation.HasValue)
                    {
                        assessmentBuilder.AppendFormat("<div class='woundLineListingEntry'><div style='font-weight:bold;'>Wound Bed Granulation</div> {0} </div>",
                            lastAssessment.WoundBedGranulation.Value.ToString("#.##"));
                    }

                    if (lastAssessment.WoundBedNecrosis.HasValue)
                    {
                        assessmentBuilder.AppendFormat("<div class='woundLineListingEntry'><div style='font-weight:bold;'>Wound Bed Necrosis/Escar</div> {0} </div>",
                            lastAssessment.WoundBedNecrosis.Value.ToString("#.##"));
                    }

                    if (lastAssessment.WoundBedSlough.HasValue)
                    {
                        assessmentBuilder.AppendFormat("<div class='woundLineListingEntry'><div style='font-weight:bold;'>Wound Bed Slough</div> {0} </div>",
                            lastAssessment.WoundBedSlough.Value.ToString("#.##"));
                    }

                    if (lastAssessment.WoundBedOther != null && lastAssessment.WoundBedOther != string.Empty)
                    {
                        assessmentBuilder.AppendFormat("<div class='woundLineListingEntry'><div style='font-weight:bold;'>Wound Bed Other</div> {0} </div>",
                            lastAssessment.WoundBedSlough);
                    }

                    if (lastAssessment.WoundEdge != Domain.Enumerations.WoundEdge.None)
                    {
                        assessmentBuilder.AppendFormat("<div class='woundLineListingEntry'><div style='font-weight:bold;'>Wound Edge</div> {0} </div>",
                                System.Enum.GetName(typeof(Domain.Enumerations.WoundEdge), lastAssessment.WoundEdge).SplitPascalCase());
                    }

                    if (lastAssessment.PeriwoundTissue != Domain.Enumerations.WoundPeriwoundTissue.None)
                    {
                        assessmentBuilder.AppendFormat("<div class='woundLineListingEntry'><div style='font-weight:bold;'>Periwound Tissue</div> {0} </div>",
                                System.Enum.GetName(typeof(Domain.Enumerations.WoundPeriwoundTissue), lastAssessment.PeriwoundTissue).SplitPascalCase());
                    }

                    assessmentBuilder.AppendFormat("<div class='woundLineListingEntry'><div style='font-weight:bold;'>Pain</div> {0} </div>",
                        lastAssessment.Pain.FormatAsAnswer());

                    assessmentBuilder.AppendFormat("<div class='woundLineListingEntry'><div style='font-weight:bold;'>Progress</div> {0} </div>",
                                System.Enum.GetName(typeof(Domain.Enumerations.WoundProgress), lastAssessment.Progress).SplitPascalCase());


                    this.RecentProgress = lastAssessment.Progress;

                    assessmentBuilder.AppendFormat("<div class='woundLineListingEntry'><div style='font-weight:bold;'>Treatment</div> {0} </div>",
                                System.Enum.GetName(typeof(Domain.Enumerations.WoundTreatmentStatus), lastAssessment.TreatmentStatus).SplitPascalCase());


                    assessmentBuilder.AppendFormat("<div class='woundLineListingEntry'><div style='font-weight:bold;'>Push Score</div> {0} </div>",
                                lastAssessment.PushScore);



                    LastAssessment = assessmentBuilder.ToString();

                }


                if (report.IsResolved == true)
                {
                    Resolved = report.ResolvedOn.FormatAsShortDate();
                }
                else
                {
                    Resolved = string.Empty;
                }


                SiteName = report.Site != null ? report.Site.Name : string.Empty;
                LocationX = report.LocationX;
                LocationY = report.LocationY;

                AdditionalSiteDetails = report.AdditionalSiteDetails;

                CurrentStage = report.CurrentStage.Name;
                Classification = System.Enum.GetName(typeof(Domain.Enumerations.WoundClassification), report.Classification).SplitPascalCase();


                var stageSortedAssessments = report.Assessments.Where(x => x.Stage.RatingValue.HasValue).OrderBy(x => x.Stage.RatingValue);

                if (stageSortedAssessments.Count() < 1)
                {
                    MinStage = string.Empty;
                    MaxStage = string.Empty;
                }
                else
                {
                    MinStage = stageSortedAssessments.First().Stage.Name;
                    MaxStage = stageSortedAssessments.Last().Stage.Name;
                }



                var relatedPrecautions = new List<string>();

                foreach (var prec in precautions)
                {
                    if (prec.StartDate >= report.FirstNotedOn.Value)
                    {
                        if (report.ResolvedOn.HasValue)
                        {
                            if (prec.StartDate <= report.ResolvedOn.Value)
                            {
                                relatedPrecautions.Add(prec.PrecautionType.Name);
                            }
                        }
                        else
                        {
                            relatedPrecautions.Add(prec.PrecautionType.Name);
                        }
                    }
                }

                Precautions = relatedPrecautions;
            }

        }
    }
}
