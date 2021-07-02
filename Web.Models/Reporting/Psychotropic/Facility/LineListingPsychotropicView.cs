using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using IQI.Intuition.Domain.Models;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Extensions.IO;
using RedArrow.Framework.Extensions.Common;
using IQI.Intuition.Infrastructure.Services.BusinessLogic.Psychotropic;

namespace IQI.Intuition.Web.Models.Reporting.Psychotropic.Facility
{
    public class LineListingPsychotropicView
    {

        public IEnumerable<SelectListItem> WingOptions { get; set; }
        public IEnumerable<SelectListItem> DrugTypeOptions { get; set; }

        public int? Wing { get; set; }
        public int? DrugType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IList<DrugTypeEntry> Entries { get; set; }
        public bool ActiveOnly { get; set; }

        public void SetDate(IEnumerable<Domain.Models.PsychotropicAdministration> data,
            IEnumerable<Domain.Models.PsychotropicDrugType> types,
            IEnumerable<Domain.Models.PsychotropicFrequency> frequencies)
        {
            this.Entries = new List<DrugTypeEntry>();
            var calculator = new AdministrationCalculator();

            foreach(var type in types)
            {
                var entry = new DrugTypeEntry();
                entry.Name = type.Name;
                entry.Entries = new List<PatientEntry>();
                this.Entries.Add(entry);

                var typeData = data.Where(x => x.DrugType.Id == type.Id);

                foreach (var patient in typeData.Select(x => x.Patient).Distinct())
                {
                    var patientEntry = new PatientEntry();
                    patientEntry.Entries = new List<AdminEntry>();
                    patientEntry.PatientName = patient.FullName;
                    patientEntry.Id = patient.Guid.ToString();
                    entry.Entries.Add(patientEntry);

                    foreach (var admin in typeData.Where(x => x.Patient.Id == patient.Id))
                    {
                        var adminEntry = new AdminEntry();
                        patientEntry.Entries.Add(adminEntry);
                        

 
                        adminEntry.DrugName = admin.Name;
                        adminEntry.SideEffects = admin.SideEffects.ToStringSafely();
                        adminEntry.StartDate = admin.GetStartDate().FormatAsShortDate();
                        adminEntry.EndDate = admin.GetEndDate().FormatAsShortDate();
                        adminEntry.TargetBehavior = admin.TargetBehavior;
                        adminEntry.Id = admin.Id.ToString();

                        /* Current dosage */

                        var currentDosage = admin.DosageChanges.OrderBy(x => x.StartDate).Last();
                        adminEntry.CurrentDosageDescription = string.Concat(currentDosage.Frequency.GetFrequencyDefinition().GetTotal(currentDosage),
                            admin.DosageForm.Name,
                            " ",
                            currentDosage.Frequency.Name);

                        /* Prn */

                        adminEntry.Prns = new List<PrnEntry>();
                        decimal totalPrn = 0;

                        foreach (var prn in admin.PRNs.Where(x => x.GivenOn >= this.StartDate && x.GivenOn < this.EndDate).OrderBy(x => x.GivenOn))
                        {
                            adminEntry.Prns.Add(new PrnEntry()
                            {
                                Description = string.Concat(prn.Dosage, " ", prn.Administration.DosageForm.Name),
                                Date = prn.GivenOn.FormatAsShortDate()
                            });

                            adminEntry.TotalPrn = adminEntry.TotalPrn + prn.Dosage.Value;
                            totalPrn = totalPrn + prn.Dosage.Value;
                        }

                        adminEntry.TotalPrn = string.Concat(totalPrn,admin.DosageForm.Name);

                        /* dosage changes */

                        var changes = admin.DosageChanges
                            .OrderBy(x => x.StartDate)
                            .ToList();


                        decimal? lastDosage = null;

                        adminEntry.Changes = new List<ChangeEntry>();

                        foreach (var change in changes)
                        {
                            if (change.StartDate >= this.StartDate && change.StartDate < this.EndDate)
                            {

                                var priorIndex = changes.IndexOf(change) -1;
                                var changeEntry = new ChangeEntry();
                                decimal c = 0;

                                if (priorIndex >= 0)
                                {
                                    lastDosage = changes[priorIndex].GetDailyAverageDosage().Value;
                                    c = 0 -(lastDosage.Value - change.GetDailyAverageDosage().Value);
                                }

                                changeEntry.ChangeAmount = 0 - ((lastDosage.HasValue ? lastDosage.Value : 0) - change.GetDailyAverageDosage().Value);

                                changeEntry.Description = string.Concat(change.GetTotalDosage(), admin.DosageForm.Name, " ", change.Frequency.Name);

                                changeEntry.Date = change.StartDate.FormatAsShortDate();

                                adminEntry.Changes.Add(changeEntry);

                                adminEntry.DosageChange = adminEntry.DosageChange + c;
                            }
                        }



                        adminEntry.TotalChangeDescription = string.Concat(adminEntry.DosageChange, admin.DosageForm.Name, " per day");

                        /* calc total admins */
                       var totalGiven = calculator.Calculate(this.StartDate.Value,
                            this.EndDate.Value,
                            admin.DosageChanges,
                            admin.PRNs,
                            frequencies);

                       adminEntry.TotalGiven = string.Concat(totalGiven, admin.DosageForm.Name);
                       
                    }

                }

            }

        }

        public class DrugTypeEntry
        {
            public string Name { get; set; }
            public IList<PatientEntry> Entries { get; set; }
        }

        public class PatientEntry
        {
            public string PatientName { get; set; }
            public IList<AdminEntry> Entries { get; set; }
            public string Id { get; set; }
        }

        public class AdminEntry
        {
            public string Id { get; set; }
            public string DrugName { get; set; }
            public string TotalChangeDescription { get; set; }
            public decimal DosageChange { get; set; }
            public string TotalPrn { get; set; }
            public string TotalGiven { get; set; }

            public int ChangeType { get; set; }
            public string SideEffects { get; set; }
            public string StartDate { get; set; }
            public string EndDate { get; set; }
            public string TargetBehavior { get; set; }

            public string CurrentDosageDescription { get; set; }

            public IList<ChangeEntry> Changes { get; set; }
            public IList<PrnEntry> Prns { get; set; }

        }

        public class ChangeEntry
        {
            public string Description { get; set; }
            public decimal ChangeAmount { get; set; }
            public string Date { get; set; }
        }

        public class PrnEntry
        {
            public string Description { get; set; }
            public string Date { get; set; }
        }
    }
}
