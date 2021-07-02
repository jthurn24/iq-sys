using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Persistence.NHibernate;
using RedArrow.Framework.Logging;
using SnyderIS.sCore.Persistence;
using System.Collections.Generic;

namespace IQI.Intuition.Infrastructure.Services.Migration.InfectionCriteria
{
    public class Nhsn : IConsoleService
    {
        private IStatelessDataContext _DataContext;


        public Nhsn(IStatelessDataContext dataContext)
        {
            _DataContext = dataContext;
        }

        public void Run(string[] args)
        {


            /************************* BJ-Bone And Joint ********************************/

            Type.Create("BJ-Bone And Joint", "#f3d6b6", _DataContext)
                .AddCondition("BONE-Osteomyelitis")
                    .AddL1RuleSet(1)
                        .SetInstructions("Osteomyelitis must meet at least 1 of the following criteria:")
                        .SetComment("<b>Reporting instruction</b> <br> * Report mediastinitis following cardiac surgery that is accompanied by osteomyelitis as SSI-MED rather than SSI-BONE.")
                        .AddRule(1)
                            .SetInstructions("1.")
                            .AddCriteria("Patient has organisms cultured from bone.")
                        .AddRule(1)
                            .SetInstructions("2.")
                            .AddCriteria("Patient has evidence of osteomyelitis on direct examination of the bone during an invasive procedure or histopathologic examination.")
                        .AddL2RuleSet(2)
                            .AddRule(2)
                                .SetInstructions("3. Patient has at least 2 of the following signs or symptoms: (* With no other recognized cause)")
                                .AddCriteria("fever (>38&deg;C), localized swelling*")
                                .AddCriteria("tenderness*")
                                .AddCriteria("heat*")
                                .AddCriteria("drainage at suspected site of bone infection*")
                            .AddRule(1)
                                .SetInstructions("and at least 1 of the following: ")
                                .AddCriteria("a. organisms cultured from blood")
                                .AddCriteria("b. positive laboratory test on blood (e.g., antigen tests for H influenzae or S pneumoniae)")
                                .AddCriteria("c. imaging test evidence of infection (e.g., abnormal findings on x-ray, CT scan, MRI, radiolabel scan [gallium, technetium, etc.]).")

                .AddCondition("DISC-Disc space")
                    .AddL1RuleSet(1)
                        .SetInstructions("Vertebral disc space infection must meet at least 1 of the following criteria:")
                        .AddRule(1)
                            .SetInstructions("1.")
                            .AddCriteria("Patient has organisms cultured from vertebral disc space tissue obtained during an invasive procedure.")
                        .AddRule(1)
                            .SetInstructions("2.")
                            .AddCriteria("Patient has evidence of vertebral disc space infection seen during an invasive procedure or histopathologic examination.")
                        .AddRule(2)
                            .SetInstructions("3. (* With no other recognized cause)")
                            .AddCriteria("Patient has fever (>38&deg;C) or pain at the involved vertebral disc space *")
                            .AddCriteria("Imaging test evidence of infection, (e.g., abnormal findings on x-ray, CT scan, MRI, radiolabel scan [gallium, technetium, etc.]).")
                        .AddRule(2)
                            .SetInstructions("4. (* With no other recognized cause)")
                            .AddCriteria("Patient has fever (>38&deg;C) and pain at the involved vertebral disc space * ")
                            .AddCriteria("Positive laboratory test on blood or urine (e.g., antigen tests for H influenzae, S pneumoniae, N meningitidis, or Group B Streptococcus).")

                .AddCondition("JNT-Joint or bursa")
                        .AddL1RuleSet(1)
                            .SetInstructions("Joint or bursa infections must meet at least 1 of the following criteria:")
                            .AddRule(1)
                                .SetInstructions("1.")
                                .AddCriteria("Patient has organisms cultured from joint fluid or synovial biopsy.")
                            .AddRule(1)
                                .SetInstructions("2.")
                                .AddCriteria("Patient has evidence of joint or bursa infection seen during an invasive procedure or histopathologic examination.")
                            .AddL2RuleSet(2)
                                .SetInstructions("3.")
                                .AddRule(2)
                                    .SetInstructions("Patient has at least 2 of the following signs or symptoms with no other recognized cause:")
                                    .AddCriteria("joint pain")
                                    .AddCriteria("swelling")
                                    .AddCriteria("tenderness")
                                    .AddCriteria("heat")
                                    .AddCriteria("evidence of effusion")
                                    .AddCriteria("limitation of motion")
                                .AddRule(1)
                                    .SetInstructions("and at least 1 of the following:")
                                    .AddCriteria("a. organisms and white blood cells seen on Gram’s stain of joint fluid")
                                    .AddCriteria("b. positive laboratory test on blood, urine, or joint fluid")
                                    .AddCriteria("c. cellular profile and chemistries of joint fluid compatible with infection and not explained by an underlying rheumatologic disorder")
                                    .AddCriteria("d. imaging test evidence of infection (e.g., abnormal findings on x-ray, CT scan, MRI, radiolabel scan [gallium, technetium, etc.]).");


            /************************* BSI-BloodStream ********************************/

            string BSI_Comments = "<p>1. In LCBI criterion 1, the phrase \"one or more blood cultures\" means that at least one bottle from a blood draw is reported by the laboratory as having grown at least one organism (i.e., is a positive blood culture).</p>";
            BSI_Comments += "<p>2. In LCBI criterion 1, the term “recognized pathogen” does not include organisms considered common commensals (see criteria 2 and 3 for the list of common commensals). A few of the recognized pathogens are S. aureus, Enterococcus spp., E. coli, Pseudomonas spp., Klebsiella spp., Candida spp., etc.</p>";
            BSI_Comments += "<p>3. In LCBI criteria 2 and 3, the phrase “two or more blood cultures drawn on separate occasions” means 1) that blood from at least two blood draws were collected within two calendar days of each other (e.g., blood draws on Monday and Tuesday would be acceptable for blood cultures drawn on separate occasions, but blood draws on Monday and Wednesday would be too far apart in time to meet this criterion), and 2) that at least one bottle from each blood draw is reported by the laboratory as having grown the same";
            BSI_Comments += "common commensal (i.e., is a positive blood culture). (See Comment 4 for determining sameness of organisms.)";
            BSI_Comments += "<br>a. For example, an adult patient has blood drawn at 8 a.m. and again at 8:15 a.m. of the same day. Blood from each blood draw is inoculated into two bottles and incubated (four bottles total). If one bottle from each blood draw set is positive for coagulase-negative staphylococci, this part of the criterion is met.";
            BSI_Comments += "<br>b. For example, a neonate has blood drawn for culture on Tuesday and again on Thursday and both grow the same common commensal. Because the time between these blood cultures exceeds the 2-day period for blood draws stipulated in LCBI and MBI-LCBI criteria 2 and 3, this part of the criterion is not met.";
            BSI_Comments += "<br>c. “Separate occasions” also means blood draws collected from separate sites or separate accesses of the same site, such as two draws from a single lumen catheter or draws from separate lumens of a catheter. In the latter case, the draws may be just minutes apart (i.e., just the time it takes to disinfect and draw the specimen from each lumen). For example, a patient with a triple lumen central line has blood drawn from each lumen within 15 minutes of each other. Each of these is considered a separate blood draw.";
            BSI_Comments += "<br>d. A blood culture may consist of a single bottle for a pediatric blood draw due to volume constraints. Therefore, to meet this part of the criterion, each bottle from two or more draws would have to be culture-positive for the same commensal.";
            BSI_Comments += "</p>";
            BSI_Comments += "<p>4. If the pathogen or common commensal is identified to the species level from one blood culture, and a companion blood culture is identified with only a descriptive name (e.g., to the genus level), then it is assumed that the organisms are the same. The organism identified to the species level should be reported as the infecting organism along with its antibiogram if available (see Table 4 below).</p>";
            BSI_Comments += "<p>5. Only genus and species identification should be utilized to determine the sameness of organisms (i.e., matching organisms). No additional comparative methods should be used (e.g., morphology or antibiograms) because laboratory testing capabilities and protocols may vary between facilities. This will reduce reporting variability, solely due to laboratory practice, between facilities reporting LCBIs meeting criterion 2. Report the organism to the genus/species level only once, and if antibiogram data are available, report the results from the most resistant panel.</p>";
            BSI_Comments += "<p>6. LCBI criteria 1 and 2 and MBI-LCBI criteria 1 and 2 may be used for patients of any age, including those patients ≤1 year of age.</p>";
            BSI_Comments += "<p>7. Specimen Collection Considerations: Ideally, blood specimens for culture should be obtained from two to four blood draws from separate venipuncture sites (e.g., right and left antecubital veins), not through a vascular catheter. These blood draws should be performed simultaneously or over a short period of time (i.e., within a few hours). If your facility does not currently obtain specimens using this technique, you must still report BSIs using the criteria and comments above, but you should work with appropriate personnel to facilitate better specimen collection practices for blood cultures.</p>";
            BSI_Comments += "<p>8. \"No other organisms isolated\" means there is not isolation in a blood culture of another recognized pathogen (e.g., S. aureus) or common commensal (e.g., coagulase-negative staphylococci) other than listed in MBI-LCBI criterion 1, 2 or 3 that would otherwise meet LCBI criteria. If this occurs, the infection should not be classified as MBI-LCBI.</p>";
            BSI_Comments += "<p>9. Grade III/IV GI GVHD is defined as follows:";
            BSI_Comments += "<ul><li>In adults: ≥1 L diarrhea/day or ileus with abdominal pain</li>";
            BSI_Comments += "<li>In pediatric patients: ≥20 cc/kg/day of diarrhea</li></ul></p>";
            BSI_Comments += "<p><b>Reporting Instructions</b></p>";
            BSI_Comments += "<p>1. Report organisms cultured from blood as BSI–LCBI when no other site of infection is evident (see Appendix 1. Secondary Bloodstream Infection (BSI) Guide).</p>";
            BSI_Comments += "<p>2. Catheter tip cultures are not used to determine whether a patient has a primary BSI.</p>";
            BSI_Comments += "<p>3. When there is a positive blood culture and clinical signs or symptoms of localized infection at a vascular access site, but no other infection can be found, the infection is considered a primary BSI.</p>";
            BSI_Comments += "<p>4. Purulent phlebitis confirmed with a positive semiquantitative culture of a catheter tip, but with either negative or no blood culture is considered a CVS-VASC, not a BSI or an SST-SKIN or ST infection.</p>";
            BSI_Comments += "<p>5. Occasionally a patient with both peripheral and central IV lines develops a primary bloodstream infection (LCBI) that can clearly be attributed to the peripheral line (e.g., pus at the insertion site and matching pathogen from pus and blood). In this situation, enter “Central Line = No” in the NHSN application. You should, however, include the patient’s central line days in the summary denominator count.</p>";
            BSI_Comments += "<p>6. If your state or facility requires that you report healthcare-associated BSIs that are not central line-associated, enter “Central Line = No” in the NHSN application when reporting these BSIs. You should, however, include all of the patient’s central line days in the summary denominator count. </p>";


            string M_BSI_COMMENTS = "<p>In 2013 when reporting an LCBI, it is optional to indicate which of the underlying conditions of the MBI-LCBI criterion was met, if any. However, all CLABSI, whether LCBI or MBI-LCBI, must be reported if CLABSI is part of your Monthly Reporting Plan. </p>" + BSI_Comments;
            string L_BSI_COMMENTS = "<p>Comments and reporting instructions that follow the site-specific criteria provide further explanation and are integral to the correct application of the criteria.</p> " + BSI_Comments;

            Type.Create("BSI-BloodStream", "#b31b3b", _DataContext)
                .AddCondition("LCBI 1")
                   .AddL1RuleSet(1)
                     .SetComment(L_BSI_COMMENTS)
                     .SetInstructions("Must meet one of the following criteria:")
                     .AddRule(2)
                       .AddCriteria("Patient has a recognized pathogen cultured from one or more blood cultures")
                       .AddCriteria("Organism cultured from blood is not related to an infection at another site")

                .AddCondition("LCBI 2")
                   .AddL1RuleSet(3)
                     .SetComment(L_BSI_COMMENTS)
                     .AddRule(1)
                        .SetInstructions("Patient has at least one of the following signs or symptoms: (* With no other recognized cause)")
                        .AddCriteria("fever (>38&deg;C)")
                        .AddCriteria("chills*")
                        .AddCriteria("hypotension*")
                    .AddRule(1)
                        .SetInstructions("and")
                        .AddCriteria("positive laboratory results are not related to an infection at another site")
                    .AddRule(1)
                        .SetInstructions("and")
                        .AddCriteria("common commensal (i.e., diphtheroids [Corynebacterium spp. not C. diphtheriae], Bacillus spp. [not B. anthracis], Propionibacterium spp., coagulase-negative staphylococci [including S. epidermidis], viridans group streptococci, Aerococcus spp., and Micrococcus spp.) is cultured from two or more blood cultures drawn on separate occasions. Criterion elements must occur within a timeframe that does not exceed a gap of 1 calendar day.")

               .AddCondition("LCBI 3")
                  .AddL1RuleSet(3)
                    .SetComment(L_BSI_COMMENTS)
                       .AddRule(1)
                        .SetInstructions("Patient ≤ 1 year of age has at least one of the following signs or symptoms: (* With no other recognized cause)")
                        .AddCriteria("(>38&deg;C core) hypothermia (<36&deg;C core)")
                        .AddCriteria("apnea*")
                        .AddCriteria("bradycardia*")
                       .AddRule(1)
                        .SetInstructions("and")
                        .AddCriteria("positive laboratory results are not related to an infection at another site")
                       .AddRule(1)
                        .SetInstructions("and")
                        .AddCriteria("common skin commensal (i.e., diphtheroids [Corynebacterium spp. not C. diphtheriae], Bacillus spp. [not B. anthracis], Propionibacterium spp., coagulase-negative staphylococci [including S. epidermidis], viridans group streptococci, Aerococcus spp., Micrococcus spp.) is cultured from two or more blood cultures drawn on separate occasions. Criterion elements must occur within a timeframe that does not exceed a gap of 1 calendar day.")


               .AddCondition("MBI-LCBI 1")
                  .AddL1RuleSet(2)
                    .SetComment(M_BSI_COMMENTS)
                        .AddRule(1)
                        .SetInstructions("Patient of any age meets criterion 1 for LCBI with:")
                        .AddCriteria("At least one blood culture growing any of the following intestinal organisms with no other organisms isolated: Bacteroides spp., Candida spp., Clostridium spp., Enterococcus spp., Fusobacterium spp., Peptostreptococcus spp., Prevotella spp., Veillonella spp., or Enterobacteriaceae*")
                        .AddL2RuleSet(1)
                            .SetInstructions("And patient meets at least one of the following:")
                            .AddRule(1)
                                .SetInstructions("1. Is an allogeneic hematopoietic stem cell transplant recipient within the past year with one of the following documented during same hospitalization as positive blood culture:")
                                .AddCriteria("a. Grade III or IV gastrointestinal graft versus host disease (GI GVHD)")
                                .AddCriteria("b. ≥1 liter diarrhea in a 24-hour period (or ≥20 mL/kg in a 24-hour period for patients <18 years of age) with onset on or within 7 calendar days before the date the positive blood culture was collected.")
                            .AddRule(1)
                                .SetInstructions("2.")
                                .AddCriteria("Is neutropenic, defined as at least 2 separate days with values of absolute neutrophil count (ANC) or total white blood cell count (WBC) <500 cells/mm on or within 3 calendar days before the date the positive blood culture was collected (Day 1). (See Table 6 for example.)")


              .AddCondition("MBI-LCBI 2")
                  .AddL1RuleSet(2)
                    .SetComment(M_BSI_COMMENTS)
                        .AddRule(1)
                            .SetInstructions("Patient of any age meets criterion 2 for LCBI when:")
                            .AddCriteria("The blood cultures are growing only viridans group streptococci with no other organisms isolated")
                        .AddL2RuleSet(1)
                            .SetInstructions("And patient meets at least one of the following:")
                            .AddRule(1)
                                .SetInstructions("1. Is an allogeneic hematopoietic stem cell transplant recipient within the past year with one of the following documented during same hospitalization as positive blood culture:")
                                .AddCriteria("a. Grade III or IV gastrointestinal graft versus host disease (GI GVHD)")
                                .AddCriteria("b. ≥1 liter diarrhea in a 24-hour period (or ≥20 mL/kg in a 24-hour period for patients <18 years of age) with onset on or within 7 calendar days before the date the first positive blood culture was collected.")
                            .AddRule(1)
                                .SetInstructions("2.")
                                .AddCriteria("Is neutropenic, defined as at least 2 separate days with values of absolute neutrophil count (ANC) or total white blood cell count (WBC) <500 cells/mm on or within 3 calendar days before the date the positive blood culture was collected (Day 1). (See Table 6 for example.)")



              .AddCondition("MBI-LCBI 3")
                  .AddL1RuleSet(2)
                    .SetComment(M_BSI_COMMENTS)
                        .AddRule(1)
                            .SetInstructions("Patient ≤1 year of age meets criterion 3 for LCBI when")
                            .AddCriteria("The blood cultures are growing only viridans group streptococci with no other organisms isolated")
                        .AddL2RuleSet(1)
                            .SetInstructions("And patient meets at least one of the following:")
                            .AddRule(1)
                                .SetInstructions("1. Is an allogeneic hematopoietic stem cell transplant recipient within the past year with one of the following documented during same hospitalization as positive blood culture:")
                                .AddCriteria("a. Grade III or IV gastrointestinal graft versus host disease (GI GVHD)")
                                .AddCriteria("b. ≥20 mL/kg in a 24 hour period with onset on or within 7 calendar days before the date the first positive blood culture is collected.")
                            .AddRule(1)
                                .SetInstructions("2.")
                                .AddCriteria("Is neutropenic, defined as at least 2 separate days with values of absolute neutrophil count (ANC) or total white blood cell count (WBC) <500 cells/mm on or within 3 calendar days before the date the positive blood culture was collected (Day 1). (See Table 6 for example.)");



            /************************* CNS-CENTRAL NERVOUS SYSTEM INFECTION ********************************/

            Type.Create("CNS-Central Nervous System", "#4e5060", _DataContext)
                .AddCondition("IC-Intracranial infection")
                   .AddL1RuleSet(1)
                     .SetComment("If meningitis and a brain abscess are present together, report the infection as IC.")
                     .SetInstructions("Intracranial infection must meet at least 1 of the following criteria:")
                     .AddRule(1)
                       .SetInstructions("1.")
                       .AddCriteria("Patient has organisms cultured from brain tissue or dura")
                     .AddRule(1)
                       .SetInstructions("2.")
                       .AddCriteria("Patient has an abscess or evidence of intracranial infection seen during an invasive procedure or histopathologic examination")
                     .AddL2RuleSet(3)
                        .SetInstructions("3.")
                        .AddRule(2)
                        .SetInstructions("Patient has at least 2 of the following signs or symptoms: (* With no other recognized cause)")
                        .AddCriteria("headache*")
                        .AddCriteria("dizziness*")
                        .AddCriteria("fever (>38&deg;C)")
                        .AddCriteria("localizing neurologic signs*")
                        .AddCriteria("changing level of consciousness*")
                        .AddCriteria("confusion*")
                        .AddRule(1)
                        .SetInstructions("and at least 1 of the following:")
                        .AddCriteria("a. organisms seen on microscopic examination of brain or abscess tissue obtained by needle aspiration or by biopsy during an invasive procedure or autopsy")
                        .AddCriteria("b. positive laboratory test on blood or urine")
                        .AddCriteria("c. imaging test evidence of infection, (e.g., abnormal findings on ultrasound, CT scan, MRI, radionuclide brain scan, or arteriogram)")
                        .AddCriteria("d. diagnostic single antibody titer (IgM) or 4-fold increase in paired sera (IgG) for pathogen")
                        .AddRule(1)
                        .SetInstructions("and")
                        .AddCriteria("if diagnosis is made antemortem, physician institutes appropriate antimicrobial therapy")
                    .AddL2RuleSet(3)
                        .SetInstructions("4.")
                            .AddRule(2)
                            .SetInstructions("Patient ≤1 year of age has at least 2 of the following signs or symptoms: (* With no other recognized cause)")
                            .AddCriteria("fever (>38&deg;C core)")
                            .AddCriteria("hypothermia (<37&deg;C core)")
                            .AddCriteria("apnea*")
                            .AddCriteria("bradycardia*")
                            .AddCriteria("localizing neurologic signs*")
                            .AddCriteria("changing level of consciousness*")
                            .AddRule(1)
                            .SetInstructions("and at least 1 of the following:")
                            .AddCriteria("a. organisms seen on microscopic examination of brain or abscess tissue obtained by needle aspiration or by biopsy during an invasive procedure or autopsy")
                            .AddCriteria("b. positive laboratory test on blood or urine")
                            .AddCriteria("c. imaging test evidence of infection, (e.g., abnormal findings on ultrasound, CT scan, MRI, radionuclide brain scan, or arteriogram)")
                            .AddCriteria("d. diagnostic single antibody titer (IgM) or 4-fold increase in paired sera (IgG) for pathogen")
                            .AddRule(1)
                            .SetInstructions("and")
                            .AddCriteria("if diagnosis is made antemortem, physician institutes appropriate antimicrobial therapy")

                .AddCondition("MEN-Meningitis or ventriculitis")
                   .AddL1RuleSet(1)
                     .SetComment("Report meningitis in the newborn as healthcare associated unless there is compelling evidence indicating the meningitis was acquired transplacentally (i.e., unless it was apparent on the day of birth or the next day). <ul><li>Report CSF shunt infection as SSI-MEN if it occurs ≤1 year of placement; if later or after manipulation/access of the shunt, report as CNS-MEN.</li> <li>Report meningoencephalitis as MEN.</li> <li>Report spinal abscess with meningitis as </li></ul>")
                     .SetInstructions("Meningitis or ventriculitis must meet at least 1 of the following criteria:")
                        .AddRule(1)
                            .SetInstructions("1.")
                            .AddCriteria("Patient has organisms cultured from cerebrospinal fluid (CSF).")
                        .AddL2RuleSet(3)
                            .SetInstructions("2.")
                                .AddRule(1)
                                    .SetInstructions("Patient has at least 1 of the following signs or symptoms:  (* With no other recognized cause)")
                                    .AddCriteria("fever (>38&deg;C)")
                                    .AddCriteria("headache*")
                                    .AddCriteria("stiff neck*")
                                    .AddCriteria("meningeal signs*")
                                    .AddCriteria("cranial nerve signs*")
                                    .AddCriteria("irritability*")
                                .AddRule(1)
                                    .SetInstructions("and 1 of the following:")
                                    .AddCriteria("a. increased white cells, elevated protein, and decreased glucose in CSF")
                                    .AddCriteria("b. organisms seen on Gram’s stain of CSF")
                                    .AddCriteria("c. organisms cultured from blood")
                                    .AddCriteria("d. positive laboratory test of CSF, blood, or urine")
                                    .AddCriteria("e. diagnostic single antibody titer (IgM) or 4-fold increase in paired sera (IgG) for pathogen")
                                .AddRule(1)
                                    .SetInstructions("and")
                                    .AddCriteria("if diagnosis is made antemortem, physician institutes appropriate antimicrobial therapy")
                     .AddL2RuleSet(3)
                        .SetInstructions("3.")
                            .AddRule(1)
                            .SetInstructions("Patient ≤1 year of age has at least 1 of the following signs or symptoms: (* With no other recognized cause)")
                                .AddCriteria("fever (>38&deg;C core)")
                                .AddCriteria("hypothermia (<37&deg;C core)")
                                .AddCriteria("apnea*")
                                .AddCriteria("bradycardia*")
                                .AddCriteria("stiff neck*")
                                .AddCriteria("meningeal signs*")
                                .AddCriteria("cranial nerve signs*")
                                .AddCriteria("irritability*")
                            .AddRule(1)
                            .SetInstructions("and at least 1 of the following:")
                                .AddCriteria("a. increased white cells, elevated protein, and decreased glucose in CSF")
                                .AddCriteria("b. organisms seen on Gram’s stain of CSF")
                                .AddCriteria("c. organisms cultured from blood")
                                .AddCriteria("d. positive laboratory test of CSF, blood, or urine")
                                .AddCriteria("e. diagnostic single antibody titer (IgM) or 4-fold increase in paired sera (IgG) for pathogen")
                            .AddRule(1)
                            .SetInstructions("and")
                                .AddCriteria("if diagnosis is made antemortem, physician institutes appropriate antimicrobial therapy")


            .AddCondition("SA-Spinal abscess without meningitis")
                   .AddL1RuleSet(1)
                     .SetComment("Report spinal abscess with meningitis as MEN.")
                     .SetInstructions("An abscess of the spinal epidural or subdural space, without involvement of the cerebrospinal fluid or adjacent bone structures, must meet at least 1 of the following criteria:")
                        .AddRule(1)
                            .SetInstructions("1.")
                            .AddCriteria("Patient has organisms cultured from abscess in the spinal epidural or subdural space.")
                        .AddRule(1)
                            .SetInstructions("2.")
                            .AddCriteria("Patient has an abscess in the spinal epidural or subdural space seen during an invasive procedure or at autopsy or evidence of an abscess seen during a histopathologic examination.")
                        .AddL2RuleSet(3)
                            .SetInstructions("3.")
                            .AddRule(1)
                            .SetInstructions("Patient has at least 1 of the following signs or symptoms:")
                                .AddCriteria("fever (>38&deg;C)")
                                .AddCriteria("back pain*")
                                .AddCriteria("focal tenderness*")
                                .AddCriteria("radiculitis*")
                                .AddCriteria("paraparesis*")
                                .AddCriteria("paraplegia*")
                            .AddRule(1)
                            .SetInstructions("and at least 1 of the following:")
                                .AddCriteria("a. organisms cultured from blood")
                                .AddCriteria("b. imaging test evidence of a spinal abscess (e.g., abnormal findings on myelography, ultrasound, CT scan, MRI, or other scans [gallium, technetium, etc.]).")
                            .AddRule(1)
                            .SetInstructions("and")
                                .AddCriteria("if diagnosis is made antemortem, physician institutes appropriate antimicrobial therapy");





            /************************* CVS-CARDIOVASCULAR SYSTEM INFECTION ********************************/

            Type.Create("CVS-CardioVascular System", "#a791f8", _DataContext)
                .AddCondition("CARD-Myocarditis or pericarditis")
                   .AddL1RuleSet(1)
                   .SetInstructions("Myocarditis or pericarditis must meet at least 1 of the following criteria:")
                   .SetComment("Most cases of postcardiac surgery or postmyocardial infarction pericarditis are not infectious.")
                   .AddRule(1)
                    .SetInstructions("1.")
                    .AddCriteria("Patient has organisms cultured from pericardial tissue or fluid obtained during an invasive procedure.")
                   .AddL2RuleSet(2)
                    .SetInstructions("2.")
                        .AddRule(2)
                        .SetInstructions("Patient has at least 2 of the following signs or symptoms: (* With no other recognized cause)")
                            .AddCriteria("fever (>38&deg;C)")
                            .AddCriteria("chest pain*")
                            .AddCriteria("paradoxical pulse*")
                            .AddCriteria("or increased heart size*")
                        .AddRule(1)
                        .SetInstructions("and at least 1 of the following:")
                            .AddCriteria("a. abnormal EKG consistent with myocarditis or pericarditis")
                            .AddCriteria("b. positive laboratory test on blood (e.g., antigen tests for H influenzae or S pneumoniae)")
                            .AddCriteria("c. evidence of myocarditis or pericarditis on histologic examination of heart tissue")
                            .AddCriteria("d. 4-fold rise in type-specific antibody with or without isolation of virus from pharynx or feces")
                            .AddCriteria("e. pericardial effusion identified by echocardiogram, CT scan, MRI, or angiography.")
                    .AddL2RuleSet(2)
                        .SetInstructions("3.")
                            .AddRule(2)
                            .SetInstructions("Patient ≤1 year of age has at least 2 of the following signs or symptoms: (* With no other recognized cause)")
                            .AddCriteria("fever (>38&deg;C core)")
                            .AddCriteria("hypothermia (<37&deg;C core)")
                            .AddCriteria("apnea*")
                            .AddCriteria("bradycardia*")
                            .AddCriteria("paradoxical pulse*")
                            .AddCriteria("increased heart size*")
                            .AddRule(1)
                            .SetInstructions("and at least 1 of the following:")
                            .AddCriteria("a. abnormal EKG consistent with myocarditis or pericarditis")
                            .AddCriteria("b. positive laboratory test on blood (e.g., Antigen tests for H influenza or S pneumoniae)")
                            .AddCriteria("c. histologic examination of heart tissue shows evidence of myocarditis or pericarditis")
                            .AddCriteria("d. 4-fold rise in type-specific antibody with or without isolation of virus from pharynx or feces")
                            .AddCriteria("e. pericardial effusion identified by echocardiogram, CT scan, MRI, or angiography")

                .AddCondition("ENDO-Endocarditis")
                   .AddL1RuleSet(1)
                   .SetInstructions("Endocarditis of a natural or prosthetic heart valve must meet at least 1 of the following criteria:")
                   .SetComment("")
                   .AddRule(1)
                    .SetInstructions("1.")
                    .AddCriteria("Patient has organisms cultured from valve or vegetation.")
                   .AddL2RuleSet(3)
                    .SetInstructions("2.")
                        .AddRule(2)
                        .SetInstructions("Patient has 2 or more of the following signs or symptoms: (* With no other recognized cause)")
                            .AddCriteria("fever (>38&deg;C)")
                            .AddCriteria("new or changing murmur*")
                            .AddCriteria("embolic phenomena*")
                            .AddCriteria("skin manifestations* (i.e., petechiae, splinter hemorrhages, painful subcutaneous nodules)")
                            .AddCriteria("congestive heart failure*")
                            .AddCriteria("cardiac conduction abnormality*")
                        .AddRule(1)
                        .SetInstructions("and at least 1 of the following:")
                            .AddCriteria("a. organisms cultured from 2 or more blood cultures")
                            .AddCriteria("b. organisms seen on Gram’s stain of valve when culture is negative or not done")
                            .AddCriteria("c. valvular vegetation seen during an invasive procedure or autopsy")
                            .AddCriteria("d. positive laboratory test on blood or urine (e.g., antigen tests for H influenzae, S pneumoniae, N meningitidis, or Group B Streptococcus)")
                            .AddCriteria("e. evidence of new vegetation seen on echocardiogram")
                        .AddRule(1)
                        .SetInstructions("and")
                            .AddCriteria("if diagnosis is made antemortem, physician institutes appropriate antimicrobial therapy.")
                    .AddL2RuleSet(3)
                        .SetInstructions("3.")
                            .AddRule(2)
                            .SetInstructions("Patient ≤1 year of age has 2 or more of the following signs or symptoms: (* With no other recognized cause)")
                                .AddCriteria("fever (>38&deg;C core)")
                                .AddCriteria("hypothermia (<37&deg;C core)")
                                .AddCriteria("apnea*")
                                .AddCriteria("bradycardia*")
                                .AddCriteria("new or changing murmur*")
                                .AddCriteria("embolic phenomena*")
                                .AddCriteria("skin manifestations* (i.e., petechiae, splinter hemorrhages, painful subcutaneous nodules)")
                                .AddCriteria("congestive heart failure*")
                                .AddCriteria("cardiac conduction abnormality*")
                            .AddRule(1)
                            .SetInstructions("and at least 1 of the following:")
                                .AddCriteria("a. organisms cultured from 2 or more blood cultures")
                                .AddCriteria("b. organisms seen on Gram’s stain of valve when culture is negative or not done")
                                .AddCriteria("c. valvular vegetation seen during an invasive procedure or autopsy")
                                .AddCriteria("d. positive laboratory test on blood or urine (e.g., antigen tests for H influenzae, S pneumoniae, N meningitidis, or Group B Streptococcus)")
                                .AddCriteria("e. evidence of new vegetation seen on echocardiogram")
                            .AddRule(1)
                            .SetInstructions("and")
                                .AddCriteria("if diagnosis is made antemortem, physician institutes appropriate antimicrobial therapy")


             .AddCondition("MED-Mediastinitis")
                   .AddL1RuleSet(1)
                   .SetInstructions("Mediastinitis must meet at least 1 of the following criteria:")
                   .SetComment("Report mediastinitis following cardiac surgery that is accompanied by osteomyelitis as SSI-MED rather than SSI-BONE.")
                    .AddRule(1)
                        .SetInstructions("1.")
                        .AddCriteria("Patient has organisms cultured from mediastinal tissue or fluid obtained during an invasive procedure.")
                    .AddRule(1)
                        .SetInstructions("2.")
                        .AddCriteria("Patient has evidence of mediastinitis seen during an invasive procedure or histopathologic examination.")
                    .AddL2RuleSet(2)
                        .SetInstructions("3.")
                            .AddRule(1)
                            .SetInstructions("Patient has at least 1 of the following signs or symptoms: (* With no other recognized cause)")
                            .AddCriteria("fever (>38&deg;C)")
                            .AddCriteria("chest pain*")
                            .AddCriteria("sternal instability*")
                            .AddRule(1)
                            .SetInstructions("and at least 1 of the following:")
                            .AddCriteria("a. purulent discharge from mediastinal area")
                            .AddCriteria("b. organisms cultured from blood or discharge from mediastinal area")
                            .AddCriteria("c. mediastinal widening on imaging test.")
                    .AddL2RuleSet(2)
                        .SetInstructions("4.")
                            .AddRule(1)
                            .SetInstructions("Patient ≤1 year of age has at least 1 of the following signs or symptoms:")
                            .AddCriteria("fever (>38&deg;C core)")
                            .AddCriteria("hypothermia (<37&deg;C core)")
                            .AddCriteria("apnea*")
                            .AddCriteria("bradycardia*")
                            .AddCriteria("sternal instability*")
                            .AddRule(1)
                            .SetInstructions("and at least 1 of the following:")
                            .AddCriteria("a. purulent discharge from mediastinal area")
                            .AddCriteria("b. organisms cultured from blood or discharge from mediastinal area")
                            .AddCriteria("c. mediastinal widening on imaging test")


            .AddCondition("VASC-Arterial or venous infection")
                .AddL1RuleSet(1)
                .SetInstructions("Arterial or venous infection must meet at least 1 of the following criteria:")
                .SetComment("<p><b>Reporting instructions</b></p><ul><li>Report infections of an arteriovenous graft, shunt, or fistula or intravascular cannulation site without organisms cultured from blood as CVS-VASC.</li><li>Report intravascular infections with organisms cultured from the blood as BSI-LCBI.</li></ul>")
                    .AddL2RuleSet(2)
                        .SetInstructions("1.")
                        .AddRule(1)
                        .AddCriteria("Patient has organisms cultured from arteries or veins removed during an invasive procedure")
                        .AddRule(1)
                        .SetInstructions("and")
                        .AddCriteria("blood culture not done or no organisms cultured from blood")
                    .AddL2RuleSet(1)
                        .AddRule(1)
                        .SetInstructions("2.")
                        .AddCriteria("Patient has evidence of arterial or venous infection seen during an invasive procedure or histopathologic examination")
                    .AddL2RuleSet(3)
                        .SetInstructions("3.")
                            .AddRule(1)
                            .SetInstructions("Patient has at least 1 of the following signs or symptoms: (* With no other recognized cause)")
                            .AddCriteria("fever (>38&deg;C)")
                            .AddCriteria("pain*")
                            .AddCriteria("erythema*")
                            .AddCriteria("heat at involved vascular site")
                            .AddRule(1)
                            .SetInstructions("and")
                            .AddCriteria("more than 15 colonies cultured from intravascular cannula tip using semiquantitative culture method")
                            .AddRule(1)
                            .SetInstructions("and")
                            .AddCriteria("blood culture not done or no organisms cultured from blood")
                    .AddL2RuleSet(2)
                        .SetInstructions("4.")
                            .AddRule(1)
                            .SetInstructions("Patient has")
                            .AddCriteria("purulent drainage at involved vascular site")
                            .AddRule(1)
                            .SetInstructions("and")
                            .AddCriteria("blood culture not done or no organisms cultured from blood")
                    .AddL2RuleSet(3)
                        .SetInstructions("5.")
                            .AddRule(1)
                            .SetInstructions("Patient ≤1 year of age has at least 1 of the following signs or symptoms:")
                            .AddCriteria("fever (>38&deg;C core)")
                            .AddCriteria("hypothermia (<37&deg;C core)")
                            .AddCriteria("apnea*")
                            .AddCriteria("bradycardia*")
                            .AddCriteria("lethargy*")
                            .AddCriteria("pain*")
                            .AddCriteria("erythema*")
                            .AddCriteria("heat at involved vascular site*")
                            .AddRule(1)
                            .SetInstructions("and")
                            .AddCriteria("more than 15 colonies cultured from intravascular cannula tip using semiquantitative culture method")
                            .AddRule(1)
                            .SetInstructions("and")
                            .AddCriteria("blood culture not done or no organisms cultured from blood");



            /************************* EENT-EYE, EAR, NOSE, THROAT, OR MOUTH INFECTION ********************************/

            Type.Create("EENT-Eye,Ear,Nose,Throat,or Mouth", "#42f3f9", _DataContext)
                .AddCondition("CONJ-Conjunctivitis")
                   .AddL1RuleSet(1)
                       .SetComment("<p><b>Reporting instructions</b></p><ul><li>Report other infections of the eye as EYE.</li><li>Do not report chemical conjunctivitis caused by silver nitrate (AgNO<sub>3</sub>) as a healthcare–associated infection.</li><li>Do not report conjunctivitis that occurs as a part of a more widely disseminated viral illness (such as measles, chickenpox, or a URI).</li></ul>")
                       .SetInstructions("Conjunctivitis must meet at least 1 of the following criteria:")
                   .AddRule(1)
                       .SetInstructions("1.")
                       .AddCriteria("Patient has pathogens cultured from purulent exudate obtained from the conjunctiva or contiguous tissues, such as eyelid, cornea, meibomian glands, or lacrimal glands.")
                    .AddL2RuleSet(2)
                        .SetInstructions("2.")
                        .AddRule(1)
                            .SetInstructions("Patient has")
                            .AddCriteria("pain or redness of conjunctiva or around eye")
                        .AddRule(1)
                            .SetInstructions("and at least 1 of the following:")
                            .AddCriteria("a. WBCs and organisms seen on Gram’s stain of exudate")
                            .AddCriteria("b. purulent exudate")
                            .AddCriteria("c. positive laboratory test (e.g., antigen tests such as ELISA or IF for Chlamydia trachomatis, herpes simplex virus, adenovirus) on exudate or conjunctival scraping")
                            .AddCriteria("d. multinucleated giant cells seen on microscopic examination of conjunctival exudate or scrapings")
                            .AddCriteria("e. positive viral culture")
                            .AddCriteria("f. diagnostic single antibody titer (IgM) or 4-fold increase in paired sera (IgG) for pathogen.")

                 .AddCondition("EAR-Ear, mastoid infection")
                    .AddL1RuleSet(1)
                    .SetComment("")
                    .SetInstructions("Ear and mastoid infections must meet at least 1 of the following criteria:")
                        .AddL2RuleSet(1)
                            .SetInstructions("Otitis externa must meet at least 1 of the following criteria:")
                                .AddRule(1)
                                .SetInstructions("1.")
                                .AddCriteria("Patient has pathogens cultured from purulent drainage from ear canal.")
                                .AddL3RuleSet(2)
                                .SetInstructions("2.")
                                    .AddRule(1)
                                    .SetInstructions("Patient has at least 1 of the following signs or symptoms: (* With no other recognized cause)")
                                        .AddCriteria("fever (>38&deg;C)")
                                        .AddCriteria("pain*")
                                        .AddCriteria("redness*")
                                        .AddCriteria("drainage from ear canal*")
                                    .AddRule(1)
                                    .SetInstructions("and")
                                    .AddCriteria("organisms seen on Gram’s stain of purulent drainage")

                        .AddL2RuleSet(1)
                            .SetInstructions("Otitis media must meet at least 1 of the following criteria:")
                                .AddRule(1)
                                .SetInstructions("1.")
                                .AddCriteria("Patient has organisms cultured from fluid from middle ear obtained by tympanocentesis or at invasive procedure.")
                                .AddRule(2)
                                .SetInstructions("2. Patient has at least 2 of the following signs or symptoms: (* With no other recognized cause)")
                                    .AddCriteria("fever (>38&deg;C)")
                                    .AddCriteria("pain in the eardrum*")
                                    .AddCriteria("inflammation*")
                                    .AddCriteria("retraction*")
                                    .AddCriteria("decreased mobility of eardrum*")
                                    .AddCriteria("fluid behind eardrum*")

                        .AddL2RuleSet(1)
                            .SetInstructions("Otitis interna must meet at least 1 of the following criteria:")
                                .AddRule(1)
                                .SetInstructions("1.")
                                .AddCriteria("Patient has organisms cultured from fluid from inner ear obtained at invasive procedure")
                                .AddRule(2)
                                .SetInstructions("2.")
                                .AddCriteria("Patient has a physician diagnosis of inner ear infection")

                        .AddL2RuleSet(1)
                            .SetInstructions("Mastoiditis must meet at least 1 of the following criteria:")
                                .AddRule(1)
                                .SetInstructions("1.")
                                .AddCriteria("Patient has organisms cultured from purulent drainage from mastoid.")
                                .AddL3RuleSet(2)
                                .SetInstructions("2.")
                                    .AddRule(2)
                                    .SetInstructions("Patient has at least 2 of the following signs or symptoms: (* With no other recognized cause)")
                                    .AddCriteria("fever (>38&deg;C), pain*")
                                    .AddCriteria("tenderness*")
                                    .AddCriteria("erythema*")
                                    .AddCriteria("headache*")
                                    .AddCriteria("facial paralysis*")
                                    .AddRule(1)
                                    .SetInstructions("and at least 1 of the following:")
                                    .AddCriteria("a. organisms seen on Gram’s stain of purulent material from mastoid")
                                    .AddCriteria("b. positive laboratory test on blood.")



               .AddCondition("EYE-Eye infection, other than conjunctivitis")
                    .AddL1RuleSet(1)
                    .SetComment("")
                    .SetInstructions("An infection of the eye, other than conjunctivitis, must meet at least 1 of the following criteria:")
                        .AddRule(1)
                        .SetInstructions("1.")
                        .AddCriteria("Patient has organisms cultured from anterior or posterior chamber or vitreous fluid.")
                        .AddL2RuleSet(2)
                            .SetInstructions("2.")
                            .AddRule(2)
                            .SetInstructions("Patient has at least 2 of the following signs or symptoms with no other recognized cause:")
                            .AddCriteria("eye pain")
                            .AddCriteria("visual disturbance")
                            .AddCriteria("hypopyon")
                            .AddRule(1)
                            .SetInstructions("and at least 1 of the following:")
                            .AddCriteria("a. physician diagnosis of an eye infection")
                            .AddCriteria("b. positive laboratory test on blood (e.g., antigen tests for H influenzae or S pneumoniae)")
                            .AddCriteria("c. organisms cultured from blood.")


               .AddCondition("ORAL-Oral cavity infection (mouth, tongue, or gums)")
                    .AddL1RuleSet(1)
                    .SetComment("<p><b>Reporting instruction</b></p>Report healthcare–associated primary herpes simplex infections of the oral cavity as ORAL; recurrent herpes infections are not healthcare associated.")
                    .SetInstructions("Oral cavity infections must meet at least 1 of the following criteria:")
                    .AddRule(1)
                        .SetInstructions("1.")
                        .AddCriteria("Patient has organisms cultured from purulent material from tissues of oral cavity.")
                    .AddRule(1)
                        .SetInstructions("2.")
                        .AddCriteria("Patient has an abscess or other evidence of oral cavity infection seen on direct examination, during an invasive procedure, or during a histopathologic examination.")
                    .AddL2RuleSet(2)
                        .SetInstructions("3.")
                            .AddRule(1)
                            .SetInstructions("Patient has at least 1 of the following signs or symptoms with no other recognized cause:")
                            .AddCriteria("abscess")
                            .AddCriteria("ulceration")
                            .AddCriteria("raised white patches on inflamed mucosa")
                            .AddCriteria("plaques on oral mucosa")
                            .AddRule(1)
                            .SetInstructions("and at least 1 of the following:")
                            .AddCriteria("a. positive laboratory test of mucosal scrapings, oral secretions, or blood (e.g., Gram’s stain, KOH stain, mucosal scrapings with multinucleated giant cells, antigen test on oral secretions, antibody titers)")
                            .AddCriteria("b. physician diagnosis of infection and treatment with appropriate antifungal therapy.")

               .AddCondition("SINU-Sinusitis")
                    .AddL1RuleSet(1)
                    .SetComment("")
                    .SetInstructions("Sinusitis must meet at least 1 of the following criteria:")
                        .AddRule(1)
                        .SetInstructions("1.")
                        .AddCriteria("Patient has organisms cultured from purulent material obtained from sinus cavity")
                        .AddL2RuleSet(2)
                        .SetInstructions("2.")
                            .AddRule(1)
                            .SetInstructions("Patient has at least 1 of the following signs or symptoms: *(With no other recognized cause)")
                            .AddCriteria("fever (>38&deg;C)")
                            .AddCriteria("pain or tenderness over the involved sinus*")
                            .AddCriteria("headache*")
                            .AddCriteria("purulent exudate*")
                            .AddCriteria("nasal obstruction*")
                            .AddRule(1)
                            .SetInstructions("and at least 1 of the following:")
                            .AddCriteria("a. positive transillumination")
                            .AddCriteria("b. positive imaging test")

                .AddCondition("UR-Upper respiratory tract infection, pharyngitis, laryngitis, epiglottitis")
                    .AddL1RuleSet(1)
                    .SetComment("")
                    .SetInstructions("Upper respiratory tract infections must meet at least 1 of the following criteria:")
                    .AddL2RuleSet(2)
                    .SetInstructions("1.")
                        .AddRule(2)
                        .SetInstructions("Patient has at least 2 of the following signs or symptoms: *(With no other recognized cause)")
                        .AddCriteria("fever (>38&deg;C)")
                        .AddCriteria("erythema of pharynx*")
                        .AddCriteria("sore throat*")
                        .AddCriteria("cough*")
                        .AddCriteria("hoarseness*")
                        .AddCriteria("purulent exudate in throat*")
                        .AddRule(1)
                        .SetInstructions("and at least 1 of the following:")
                        .AddCriteria("a. organisms cultured from the specific site")
                        .AddCriteria("b. organisms cultured from blood")
                        .AddCriteria("c. positive laboratory test on blood or respiratory secretions")
                        .AddCriteria("d. diagnostic single antibody titer (IgM) or 4-fold increase in paired sera (IgG) for pathogen")
                        .AddCriteria("e. physician diagnosis of an upper respiratory infection.")
                    .AddL2RuleSet(1)
                        .AddRule(1)
                        .SetInstructions("2.")
                        .AddCriteria("Patient has an abscess seen on direct examination, during an invasive procedure, or during a histopathologic examination.")
                    .AddL2RuleSet(2)
                        .SetInstructions("3.")
                            .AddRule(2)
                            .SetInstructions("Patient ≤1 year of age has at least 2 of the following signs or symptoms: *(With no other recognized cause)")
                            .AddCriteria("fever (>38&deg;C core)")
                            .AddCriteria("hypothermia (<37&deg;C core)")
                            .AddCriteria("apnea*")
                            .AddCriteria("bradycardia*")
                            .AddCriteria("nasal discharge*")
                            .AddCriteria("purulent exudate in throat*")
                            .AddRule(1)
                            .SetInstructions("and at least 1 of the following:")
                            .AddCriteria("a. organisms cultured from the specific site")
                            .AddCriteria("b. organisms cultured from blood")
                            .AddCriteria("c. positive laboratory test on blood or respiratory secretions")
                            .AddCriteria("d. diagnostic single antibody titer (IgM) or 4-fold increase in paired sera (IgG) for pathogen")
                            .AddCriteria("e. physician diagnosis of an upper respiratory infection.");


            /************************* GI-GASTROINTESTINAL SYSTEM INFECTION ********************************/

            Type.Create("GI-GastroIntestinal System", "#b3431b", _DataContext)
                .AddCondition("GE-Gastroenteritis")
                   .AddL1RuleSet(1)
                   .SetComment("")
                   .SetInstructions("Gastroenteritis must meet at least 1 of the following criteria:")
                    .AddRule(1)
                    .SetInstructions("1.")
                    .AddCriteria("Patient has an acute onset of diarrhea (liquid stools for more than 12 hours) with or without vomiting or fever (>38&deg;C) and no likely noninfectious cause (e.g., diagnostic tests, therapeutic regimen other than antimicrobial agents, acute exacerbation of a chronic condition, or psychologic stress).")
                    .AddL2RuleSet(2)
                        .SetInstructions("2.")
                        .AddRule(2)
                            .SetInstructions("Patient has at least 2 of the following signs or symptoms: *(With no other recognized cause)")
                            .AddCriteria("nausea*")
                            .AddCriteria("vomiting*")
                            .AddCriteria("abdominal pain*")
                            .AddCriteria("fever (>38&deg;C)")
                            .AddCriteria("headache*")
                        .AddRule(1)
                            .SetInstructions("and at least 1 of the following:")
                            .AddCriteria("a. an enteric pathogen is cultured from stool or rectal swab")
                            .AddCriteria("b. an enteric pathogen is detected by routine or electron microscopy")
                            .AddCriteria("c. an enteric pathogen is detected by antigen or antibody assay on blood or feces")
                            .AddCriteria("d. evidence of an enteric pathogen is detected by cytopathic changes in tissue culture (toxin assay)")
                            .AddCriteria("e. diagnostic single antibody titer (IgM) or 4-fold increase in paired sera (IgG) for pathogen")

                .AddCondition("GIT-Gastrointestinal tract infection")
                    .AddL1RuleSet(1)
                    .SetComment("")
                    .SetInstructions("Gastrointestinal tract infections, excluding gastroenteritis and appendicitis, must meet at least 1 of the following criteria:")
                        .AddRule(1)
                        .SetInstructions("1.")
                        .AddCriteria("Patient has an abscess or other evidence of infection seen during an invasive procedure or histopathologic examination.")
                        .AddL2RuleSet(2)
                            .SetInstructions("2.")
                            .AddRule(2)
                            .SetInstructions("Patient has at least 2 of the following signs or symptoms compatible with infection of the organ or tissue involved: *(With no other recognized cause)")
                            .AddCriteria("fever (>38&deg;C)")
                            .AddCriteria("nausea*")
                            .AddCriteria("vomiting*")
                            .AddCriteria("abdominal pain*")
                            .AddCriteria("tenderness*")
                            .AddRule(1)
                            .SetInstructions("and at least 1 of the following:")
                            .AddCriteria("a. organisms cultured from drainage or tissue obtained during an invasive procedure or endoscopy or from an aseptically-placed drain")
                            .AddCriteria("b. organisms seen on Gram’s or KOH stain or multinucleated giant cells seen on microscopic examination of drainage or tissue obtained during an invasive procedure or endoscopy or from an aseptically-placed drain")
                            .AddCriteria("c. organisms cultured from blood")
                            .AddCriteria("d. evidence of pathologic findings on imaging test")
                            .AddCriteria("e. evidence of pathologic findings on endoscopic examination (e.g., Candida esophagitis or proctitis)")

                .AddCondition("HEP-Hepatitis")
                    .AddL1RuleSet(2)
                    .SetComment("<p><b>Reporting instructions</b></p><ul><li>Do not report hepatitis or jaundice of noninfectious origin (alpha-1 antitrypsin deficiency, etc.). </li><li>Do not report hepatitis or jaundice that result from exposure to hepatotoxins (alcoholic or acetaminophen- induced hepatitis, etc.).</li><li>Do not report hepatitis or jaundice that result from biliary obstruction (cholecystitis).</li></ul>")
                    .SetInstructions("Hepatitis must meet the following criterion:")
                    .AddRule(2)
                    .SetInstructions("Patient has at least 2 of the following signs or symptoms: *(With no other recognized cause)")
                        .AddCriteria("fever (>38&deg;C)")
                        .AddCriteria("anorexia*")
                        .AddCriteria("nausea*")
                        .AddCriteria("vomiting*")
                        .AddCriteria("abdominal pain*")
                        .AddCriteria("jaundice*")
                        .AddCriteria("history of transfusion within the previous 3 months")
                    .AddRule(1)
                    .SetInstructions("and at least 1 of the following:")
                        .AddCriteria("a. positive laboratory test for hepatitis A, hepatitis B, hepatitis C, or delta hepatitis")
                        .AddCriteria("b. abnormal liver function tests (e.g., elevated ALT/AST, bilirubin)")
                        .AddCriteria("c. cytomegalovirus (CMV) detected in urine or oropharyngeal secretions.")




               .AddCondition("IAB-Intraabdominal infection, not specified elsewhere")
                    .AddL1RuleSet(1)
                    .SetComment("Do not report pancreatitis (an inflammatory syndrome characterized by abdominal pain, nausea, and vomiting associated with high serum levels of pancreatic enzymes) unless it is determined to be infectious in origin.")
                    .SetInstructions("Intraabdominal infections must meet at least 1 of the following criteria:")
                        .AddRule(1)
                        .SetInstructions("1.")
                        .AddCriteria("Patient has organisms cultured from purulent material from intraabdominal space obtained during an invasive procedure.")
                        .AddRule(1)
                        .SetInstructions("2.")
                        .AddCriteria("Patient has abscess or other evidence of intraabdominal infection seen during an invasive procedure or histopathologic examination.")
                        .AddL2RuleSet(2)
                            .SetInstructions("3.")
                                .AddRule(2)
                                .SetInstructions("Patient has at least 2 of the following signs or symptoms: *(With no other recognized cause)")
                                .AddCriteria("fever (>38&deg;C)")
                                .AddCriteria("nausea*")
                                .AddCriteria("vomiting*")
                                .AddCriteria("abdominal pain*")
                                .AddCriteria("jaundice*")
                                .AddRule(1)
                                .SetInstructions("and at least 1 of the following:")
                                .AddCriteria("a. organisms cultured from drainage from an aseptically-placed drain (e.g., closed suction drainage system, open drain, T-tube drain)")
                                .AddCriteria("b. organisms seen on Gram’s stain of drainage or tissue obtained during invasive procedure or from an aseptically-placed drain")
                                .AddCriteria("c. organisms cultured from blood and imaging test evidence of infection (e.g., abnormal findings on ultrasound, CT scan, MRI, or radiolabel scans [gallium, technetium, etc.] or on abdominal x-ray).")



                .AddCondition("NEC-Necrotizing enterocolitis")
                    .AddL1RuleSet(2)
                    .SetComment("")
                    .SetInstructions("Necrotizing enterocolitis in infants (≤1 year of age) must meet the following criterion:")
                        .AddL2RuleSet(2)
                        .SetInstructions("1. Infant has at least 1 of the clinical and 1 of the imaging test findings from the lists below: (* Bilious aspirate as a result of a transpyloric placement of a nasogastric tube should be excluded)")
                            .AddRule(1)
                            .SetInstructions("At least 1 clinical sign:")
                            .AddCriteria("a. Bilious aspirate*")
                            .AddCriteria("b. Vomiting")
                            .AddCriteria("c. Abdominal distention")
                            .AddCriteria("d. Occult or gross blood in stools (with no rectal fissure)")
                            .AddRule(1)
                            .SetInstructions("and at least 1 imaging test finding:")
                            .AddCriteria("a. Pneumatosis intestinalis")
                            .AddCriteria("b. Portal venous gas (Hepatobiliary gas)")
                            .AddCriteria("c. Pneumoperitoneum")
                        .AddL2RuleSet(1)
                        .AddRule(1)
                        .SetInstructions("2. Surgical NEC: Infant has at least 1 of the following surgical findings:")
                        .AddCriteria("a. Surgical evidence of extensive bowel necrosis (>2 cm of bowel affected)")
                        .AddCriteria("b. Surgical evidence of pneumatosis intestinalis with or without intestinal perforation");



            /************************* LRI-LOWER RESPIRATORY TRACT INFECTION, OTHER THAN PNEUMONIA ********************************/

            Type.Create("LRI-Lower Respiratory Tract", "#a2fe11", _DataContext)
                .AddCondition("BRON-Bronchitis, tracheobronchitis, bronchiolitis, tracheitis, without evidence of pneumonia")
                   .AddL1RuleSet(1)
                   .SetComment("Do not report chronic bronchitis in a patient with chronic lung disease as an infection unless there is evidence of an acute secondary infection, manifested by change in organism.")
                    .AddL2RuleSet(3)
                        .SetInstructions("1.")
                            .AddRule(1)
                            .AddCriteria("Patient has no clinical or imaging test evidence of pneumonia")
                            .AddRule(2)
                            .SetInstructions("and patient has at least 2 of the following signs or symptoms: (* With no other recognized cause)")
                                .AddCriteria("fever (>38&deg;C)")
                                .AddCriteria("cough*")
                                .AddCriteria("new or increased sputum production*")
                                .AddCriteria("rhonchi*")
                                .AddCriteria("wheezing*")
                            .AddRule(1)
                            .SetInstructions("and at least 1 of the following:")
                                .AddCriteria("a. positive culture obtained by deep tracheal aspirate or bronchoscopy")
                                .AddCriteria("b. positive laboratory test on respiratory secretions.")
                    .AddL2RuleSet(3)
                        .SetInstructions("2.")
                            .AddRule(1)
                            .AddCriteria("Patient ≤1 year of age has no clinical or imaging test evidence of pneumonia")
                            .AddRule(2)
                            .SetInstructions("and patient has at least 2 of the following signs or symptoms: (* With no other recognized cause)")
                            .AddCriteria("fever (>38&deg;C core)")
                            .AddCriteria("cough*")
                            .AddCriteria("new or increased sputum production*")
                            .AddCriteria("rhonchi*")
                            .AddCriteria("wheezing*")
                            .AddCriteria("respiratory distress*")
                            .AddCriteria("apnea*")
                            .AddCriteria("bradycardia*")
                            .AddRule(1)
                            .SetInstructions("and at least 1 of the following:")
                            .AddCriteria("a. organisms cultured from material obtained by deep tracheal aspirate or bronchoscopy")
                            .AddCriteria("b. positive laboratory test on respiratory secretions")
                            .AddCriteria("c. diagnostic single antibody titer (IgM) or 4-fold increase in paired sera (IgG) for pathogen")

                    .AddCondition("LUNG-Other infection of the lower respiratory tract")
                        .AddL1RuleSet(1)
                        .SetComment("<p><b>Reporting instructions</b></p><ul><li>Report concurrent lower respiratory tract infection and pneumonia with the same organism(s) as PNEU.</li><li>Report lung abscess or empyema without pneumonia as LUNG.</li></ul>")
                        .SetInstructions("Other infections of the lower respiratory tract must meet at least 1 of the following criteria:")
                        .AddRule(1)
                            .AddCriteria("Patient has organisms seen on smear or cultured from lung tissue or fluid, including pleural fluid.")
                            .AddCriteria("Patient has a lung abscess or empyema seen during an invasive procedure or histopathologic examination.")
                            .AddCriteria("Patient has an abscess cavity seen on radiographic examination of lung.");



            /************************* PNEU-Pneumonia ********************************/


            string pnewComment = "";

            pnewComment += "<p>There are 3 specific types of pneumonia: clinically-defined pneumonia (PNU1), pneumonia with specific laboratory findings (PNU2), and pneumonia in immunocompromised patients (PNU3). Listed below are general comments applicable to all specific types of pneumonia, along with abbreviations used in the algorithms and reporting instructions (Tables 7-10).</p>";
            pnewComment += "</p>Table 11 shows threshold values for cultured specimens used in the surveillance diagnosis of pneumonia.</p>";
            pnewComment += "<p><b>General comments</b></p>";
            pnewComment += "<p>1. Physician diagnosis of pneumonia alone is not an acceptable criterion for healthcare-associated pneumonia.</p>";
            pnewComment += "<p>2. Although specific criteria are included for infants and children, pediatric patients may meet any of the other pneumonia specific site criteria.</p>";
            pnewComment += "<p>3. When assessing a patient for presence of pneumonia, it is important to distinguish between changes in clinical status due to other conditions such as myocardial infarction, pulmonary embolism, respiratory distress syndrome, atelectasis, malignancy, chronic obstructive pulmonary disease, hyaline membrane disease, bronchopulmonary dysplasia, etc. Also, care must be taken when assessing intubated patients to distinguish between tracheal colonization, upper respiratory tract infections (e.g., tracheobronchitis), and early onset pneumonia. Finally, it should be recognized that it may be difficult to determine healthcare-associated pneumonia in the elderly, infants, and immunocompromised patients because such conditions may mask typical signs or symptoms associated with pneumonia. Alternate specific criteria for the elderly, infants and immunocompromised patients have been included in this definition of healthcare-associated pneumonia.</p>";
            pnewComment += "<p>4. Healthcare–associated pneumonia can be characterized by its onset: early or late. Early-onset pneumonia occurs during the first 4 days of hospitalization and is often caused by Moraxella catarrhalis, H influenzae, and S pneumoniae. Causative agents of late-onset pneumonia are frequently Gram-negative bacilli or S aureus, including methicillin-resistant S aureus. Viruses (e.g., influenza A and B or respiratory syncytial virus) can cause early- and late-onset nosocomial pneumonia, whereas yeasts, fungi, legionellae, and Pneumocystis carinii are usually pathogens of late-onset pneumonia.</p>";
            pnewComment += "<p>5. Pneumonia due to gross aspiration (for example, in the setting of intubation in the emergency room or operating room) is considered healthcare associated if it meets any specific criteria and the infection itself was not clearly present at the time of admission to the hospital.</p>";
            pnewComment += "<p>6. Multiple episodes of healthcare-associated pneumonia may occur in critically ill patients with lengthy hospital stays. When determining whether to report multiple episodes of healthcare–associated pneumonia in a single patient, look for evidence of resolution of the initial infection. The addition of or change in pathogen alone is not indicative of a new episode of pneumonia. The combination of new signs and symptoms and radiographic evidence or other diagnostic testing is required.</p>";
            pnewComment += "<p>7. Positive Gram’s stain for bacteria and positive KOH (potassium hydroxide) mount for elastin fibers and/or fungal hyphae from appropriately collected sputum specimens are important clues that point toward the etiology of the infection. However, sputum samples are frequently contaminated with airway colonizers and therefore must be interpreted cautiously. In particular, Candida is commonly seen on strain, but infrequently causes healthcare-associated pneumonia, especially in immunocompetent patients.</p>";
            pnewComment += "<p><b>Abbreviations</b></p>";
            pnewComment += "<p>BAL–bronchoalveolar lavage<br>EIA–enzyme immunoassay<br>FAMA–fluorescent-antibody staining of membrane antigen<br>IFA–immunofluorescent antibody<br>LRT–lower respiratory tract<br>PCR–polymerase chain reaction<br>PMN–polymorphonuclear leukocyte<br>RIA–radioimmunoassay</p>";
            pnewComment += "<p><b>Reporting instructions</b></p>";
            pnewComment += "<ul><li>There is a hierarchy of specific categories within the major type pneumonia (PNEU). Even if a patient meets criteria for more than 1 specific site, report only 1:";
            pnewComment += "<br>- If a patient meets criteria for both PNU1 and PNU2, report PNU2.";
            pnewComment += "<br>- If a patient meets criteria for both PNU2 and PNU3, report PNU3.";
            pnewComment += "<br>- If a patient meets criteria for both PNU1 and PNU3, report PNU3.</li>";
            pnewComment += "<li>Report concurrent lower respiratory tract infection (e.g., abscess or empyema) and pneumonia with the same organism(s) as PNEU.</li>";
            pnewComment += "<li>Lung abscess or empyema without pneumonia is classified as LUNG.</li>";
            pnewComment += "<li>Bronchitis, tracheitis, tracheobronchitis, or bronchiolitis without pneumonia are classified as BRON.</li></ul>";

            Type.Create("PNEU-Pneumonia", "#094939", _DataContext)
                .AddCondition("PNU1")
                   .AddL1RuleSet(2)
                   .SetComment(pnewComment)
                    .AddL2RuleSet(1)
                      .SetInstructions("<b>Radiology</b>")
                        .AddRule(1)
                        .SetInstructions("Two or more serial chest radiograph with at least one of the following:  (NOTE: In patients without underlying pulmonary or cardiac disease (e.g., respiratory distress syndrome, bronchopulmonary dysplasia, pulmonary edema, or chronic obstructive pulmonary disease), one definitive chest radiograph is acceptable ")
                        .AddCriteria("New or progressive and persistent infiltrate")
                        .AddCriteria("Consolidation")
                        .AddCriteria("Cavitation")
                        .AddCriteria("Pneumatoceles, in infants ≤1 year old")
                    .AddL2RuleSet(1)
                        .SetInstructions("<b>Signs/Symptoms/Laboratory</b>")
                            .AddL3RuleSet(2)
                            .SetInstructions("FOR ANY PATIENT")
                                .AddRule(1)
                                .SetInstructions("at least one of the following:")
                                .AddCriteria("Fever (>38&deg;C or >100.4&deg;F)")
                                .AddCriteria("Leukopenia (<4000 WBC/mm) or leukocytosis (≥12,000 WBC/mm)")
                                .AddCriteria("For adults ≥70 years old, altered mental status with no other recognized cause")
                                .AddRule(2)
                                .SetInstructions("and at least two of the following:")
                                .AddCriteria("New onset of purulent sputum, or change in character of sputum, or increased respiratory secretions, or increased suctioning requirements")
                                .AddCriteria("New onset or worsening cough, or dyspnea, or tachypnea")
                                .AddCriteria("Rales6 or bronchial breath sounds")
                                .AddCriteria("Worsening gas exchange (e.g., O<sub>2</sub> desaturations (e.g., PaO<sub>2</sub>/FiO<sub>2</sub> ≤240), increased oxygen requirements, or increased ventilator demand) ")
                           .AddL3RuleSet(2)
                           .SetInstructions("ALTERNATE CRITERIA, for infants <1 year old:")
                                .AddRule(1)
                                .AddCriteria("Worsening gas exchange (e.g., O<sub>2</sub>desaturations [e.g., pulse oximetry <94%], increased oxygen requirements, or increased ventilator demand)")
                                .AddRule(3)
                                .SetInstructions("and at least three of the following:")
                                .AddCriteria("Temperature instability")
                                .AddCriteria("Leukopenia (<4000 WBC/mm) or leukocytosis (≥15,000 WBC/mm) and left shift (≥10% band forms)")
                                .AddCriteria("New onset of purulent sputum or change in character of sputum, or increased respiratory secretions or increased suctioning requirements")
                                .AddCriteria("Apnea, tachypnea , nasal flaring with retraction of chest wall or grunting")
                                .AddCriteria("Wheezing, rales, or rhonchi")
                                .AddCriteria("Cough")
                                .AddCriteria("Bradycardia (<100 beats/min) or tachycardia (>170 beats/min)")
                           .AddL3RuleSet(1)
                           .AddRule(3)
                           .SetInstructions("ALTERNATE CRITERIA, for child >1 year old or ≤12 years old, at least three of the following:")
                            .AddCriteria("Fever (>38.4&deg;C or >101.1&deg;F) or hypothermia (<36.5&deg;C or <97.7&deg;F)")
                            .AddCriteria("Leukopenia (<4000 WBC/mm) or leukocytosis (≥15,000 WBC/mm)")
                            .AddCriteria("New onset of purulent sputum, or change in character of sputum4, or increased respiratory secretions, or increased suctioning requirements")
                            .AddCriteria("New onset or worsening cough, or dyspnea, apnea, or tachypnea.")
                            .AddCriteria("Rales6 or bronchial breath sounds")
                            .AddCriteria("Worsening gas exchange (e.g., O<sub>2</sub> desaturations [e.g., pulse oximetry <94%], increased oxygen requirements, or increased ventilator demand)")
            .AddCondition("PNU2")
                .AddL1RuleSet(3)
                       .SetComment(pnewComment)
                        .AddL2RuleSet(1)
                          .SetInstructions("<b>Radiology</b>")
                          .AddRule(1)
                            .SetInstructions("Two or more serial chest radiograph with at least one of the following (NOTE: In patients without underlying pulmonary or cardiac disease (e.g., respiratory distress syndrome, bronchopulmonary dysplasia, pulmonary edema, or chronic obstructive pulmonary disease), one definitive chest radiograph is acceptable.)")
                            .AddCriteria("New or progressive and persistent infiltrate")
                            .AddCriteria("Consolidation")
                            .AddCriteria("Cavitation")
                            .AddCriteria("Pneumatoceles, in infants ≤1 year old")
                        .AddL2RuleSet(2)
                            .SetInstructions("<b>Signs/Symptoms</b>")
                                .AddRule(1)
                                .SetInstructions("At least one of the following:")
                                .AddCriteria("Fever (>38&deg;C or >100.4&deg;F)")
                                .AddCriteria("Leukopenia (<4000 WBC/mm) or leukocytosis (≥12,000 WBC/mm)")
                                .AddCriteria("For adults ≥70 years old, altered mental status with no other recognized cause")
                                .AddRule(1)
                                .SetInstructions("and at least one of the following:")
                                .AddCriteria("New onset of purulent sputum, or change in character of sputum, or increased respiratory secretions, or increased suctioning requirements")
                                .AddCriteria("New onset or worsening cough, or dyspnea or tachypnea")
                                .AddCriteria("Rales or bronchial breath sounds")
                                .AddCriteria("Worsening gas exchange (e.g., O<sub>2</sub> desaturations [e.g., PaO<sub>2</sub>/FiO<sub>2</sub> ≤240], increased oxygen requirements, or increased ventilator demand)")
                        .AddL2RuleSet(1)
                            .SetInstructions("<b>Laboratory</b>")
                                .AddL3RuleSet(1)
                                    .AddRule(1)
                                    .SetInstructions("At least one of the following:")
                                    .AddCriteria("Positive growth in blood culture not related to another source of infection")
                                    .AddCriteria("Positive growth in culture of pleural fluid")
                                    .AddCriteria("Positive quantitative culture from minimally contaminated LRT specimen (e.g., BAL or protected specimen brushing)")
                                    .AddCriteria("≥5% BAL-obtained cells contain intracellular bacteria on direct microscopic exam (e.g., Gram’s stain)")
                                    .AddRule(1)
                                    .SetInstructions("or Histopathologic exam shows at least one of the following evidences of pneumonia:")
                                    .AddCriteria("Abscess formation or foci of consolidation with intense PMN accumulation in bronchioles and alveoli")
                                    .AddCriteria("Positive quantitative culture of lung parenchyma")
                                    .AddCriteria("Evidence of lung parenchyma invasion by fungal hyphae or pseudohyphae")
            .AddCondition("PNU3")
                            .AddL1RuleSet(3)
                                   .SetComment(pnewComment)
                                    .AddL2RuleSet(1)
                                      .SetInstructions("<b>Radiology</b>")
                                      .AddRule(1)
                                      .SetInstructions("Two or more serial chest radiograph with at least one of the following (NOTE: In patients without underlying pulmonary or cardiac disease (e.g., respiratory distress syndrome, bronchopulmonary dysplasia, pulmonary edema, or chronic obstructive pulmonary disease), one definitive chest radiograph is acceptable)")
                                      .AddCriteria("New or progressive and persistent infiltrate")
                                      .AddCriteria("Consolidation")
                                      .AddCriteria("Cavitation")
                                      .AddCriteria("Pneumatoceles, in infants ≤1 year old")
                                    .AddL2RuleSet(1)
                                    .SetInstructions("<b>Signs/Symptoms</b>")
                                        .AddRule(1)
                                        .SetInstructions("Patient who is immunocompromised has at least one of the following:")
                                        .AddCriteria("Fever (>38&deg;C or >100.4&deg;F)")
                                        .AddCriteria("For adults ≥70 years old, altered mental status with no other recognized cause")
                                        .AddCriteria("New onset of purulent sputum, or change in character of sputum, or increased respiratory secretions, or increased suctioning requirements")
                                        .AddCriteria("New onset or worsening cough, or dyspnea, or tachypnea")
                                        .AddCriteria("Rales or bronchial breath sounds")
                                        .AddCriteria("Worsening gas exchange (e.g., O<sub>2</sub> desaturations [e.g., PaO<sub>2</sub>/FiO<sub>2</sub> ≤240], increased oxygen requirements, or increased ventilator demand)")
                                        .AddCriteria("Hemoptysis")
                                        .AddCriteria("Pleuritic chest pain")
                                    .AddL2RuleSet(1)
                                    .SetInstructions("<b>Laboratory</b>")
                                        .AddRule(1)
                                        .SetInstructions("At least one of the following:")
                                        .AddCriteria("Matching positive blood and sputum cultures with Candida spp.")
                                        .AddCriteria("Positive culture of virus or Chlamydia from respiratory secretions")
                                        .AddCriteria("Positive detection of viral antigen or antibody from respiratory secretions (e.g., EIA, FAMA, shell vial assay, PCR)")
                                        .AddCriteria("Fourfold rise in paired sera (IgG) for pathogen (e.g., influenza viruses, Chlamydia)")
                                        .AddCriteria("Positive PCR for Chlamydia or Mycoplasma")
                                        .AddCriteria("Positive micro-IF test for Chlamydia")
                                        .AddCriteria("Positive culture or visualization by micro-IF of Legionella spp, from respiratory secretions or tissue.")
                                        .AddCriteria("Detection of Legionella pneumophila serogroup 1 antigens in urine by RIA or EIA")
                                        .AddCriteria("Fourfold rise in L. pneumophila serogroup 1 antibody titer to ≥1:128 in paired acute and convalescent sera by indirect IFA.")
                                        .AddRule(1)
                                        .SetInstructions("or Evidence of fungi or Pneumocystis carinii from minimally contaminated LRT specimen (e.g., BAL or protected specimen brushing) from one of the following:")
                                        .AddCriteria("Direct microscopic exam")
                                        .AddCriteria("Positive culture of fungi");


            /************************* REPR-REPRODUCTIVE TRACT INFECTION ********************************/

            Type.Create("REPR-Reproductive Tract", "#58b47a", _DataContext)
                 .AddCondition("EMET-Endometritis")
                   .AddL1RuleSet(1)
                   .SetComment("<b>Reporting instruction</b><br>Report postpartum endometritis as a healthcare-associated infection unless the amniotic fluid is infected at the time of admission or the patient was admitted more than 2 days after rupture of the membrane. (Day 1 = rupture day) ")
                   .SetInstructions("Endometritis must meet at least 1 of the following criteria:")
                   .AddRule(1)
                    .AddCriteria("1. Patient has organisms cultured from fluid (including amniotic fluid) or tissue from endometrium obtained during an invasive procedure or biopsy.")
                    .AddCriteria("2. Patient has at least 2 of the following signs or symptoms: fever (>38&deg;C), abdominal pain*, uterine tenderness*, or purulent drainage from uterus*. (* With no other recognized cause)")
                 .AddCondition("EPIS-Episiotomy")
                    .AddL1RuleSet(1)
                    .SetComment("<b>Comment</b><br>Episiotomy is not considered an operative procedure in NHSN.")
                    .AddRule(1)
                        .SetInstructions("Episiotomy infections must meet at least 1 of the following criteria:")
                        .AddCriteria("1. Postvaginal delivery patient has purulent drainage from the episiotomy.")
                        .AddCriteria("2. Postvaginal delivery patient has an episiotomy abscess.")
                .AddCondition("OREP-Other infection of the male or female reproductive tract")
                    .AddL1RuleSet(1)
                    .SetComment("<b>Reporting Instructions</b><ul><li>Report endometritis as EMET.</li><li>Report vaginal cuff infections as VCUF.</li></ul>")
                    .SetInstructions("Other infections of the male or female reproductive tract must meet at least 1 of the following criteria:")
                    .AddRule(1)
                    .SetInstructions("1.")
                        .AddCriteria("Patient has organisms cultured from tissue or fluid from affected site.")
                    .AddRule(1)
                    .SetInstructions("2.")
                        .AddCriteria("Patient has an abscess or other evidence of infection of affected site seen during an invasive procedure or histopathologic examination.")
                    .AddL2RuleSet(2)
                    .SetInstructions("3.")
                        .AddRule(2)
                        .SetInstructions("Patient has 2 of the following signs or symptoms: (* With no other recognized cause)")
                        .AddCriteria("fever (>38&deg;C)")
                        .AddCriteria("nausea*")
                        .AddCriteria("vomiting*")
                        .AddCriteria("pain*")
                        .AddCriteria("tenderness*")
                        .AddCriteria("dysuria*")
                        .AddRule(1)
                        .SetInstructions("and at least 1 of the following:")
                        .AddCriteria("a. organisms cultured from blood")
                        .AddCriteria("b. physician diagnosis")
               .AddCondition("VCUF-Vaginal cuff infection")
                   .AddL1RuleSet(1)
                   .SetComment("<b>Reporting Instructions</b><br>Report vaginal cuff infections as SSI-VCUF.")
                   .AddRule(1)
                   .SetInstructions("Vaginal cuff infections must meet at least 1 of the following criteria:")
                   .AddCriteria("1. Posthysterectomy patient has purulent drainage from the vaginal cuff.")
                   .AddCriteria("2. Posthysterectomy patient has an abscess at the vaginal cuff.")
                   .AddCriteria("3. Posthysterectomy patient has pathogens cultured from fluid or tissue obtained from the vaginal cuff.");

                        
            /************************* SSI-SURGICAL SITE INFECTION ********************************/

            string ddisComments = string.Empty;
            ddisComments += "<p><b>Comments</b></p>";
             ddisComments += "<p>There are two specific types of deep incisional SSIs:";
             ddisComments += "<br>1. Deep Incisional Primary (DIP) – a deep incisional SSI that is identified in a primary incision in a patient that has had an operation with one or more incisions (e.g., C-section incision or chest incision for CBGB)";
             ddisComments += "<br>2. Deep Incisional Secondary (DIS) – a deep incisional SSI that is identified in the secondary incision in a patient that has had an operation with more than one incision (e.g., donor site [leg] incision for CBGB)</p>";
             ddisComments += "<p><b>Reporting instructions</b></p>";
             ddisComments += "<p>Classify infection that involves both superficial and deep incisional sites as deep incisional SSI.";
             ddisComments += "<br>Classify infection that involves superficial incisional, deep incisional, and organ/space sites as deep incisional SSI. This is considered a complication of the incision.</p>";

            string organComments = string.Empty;
            organComments += "<p><b>Comments</b></p>";
            organComments += "<p>Because an organ/space SSI involves any part of the body, excluding the skin incision, fascia, or muscle layers, that is opened or manipulated during the operative procedure, the criterion for infection at these body sites must be met in addition to the organ/space SSI criteria. For example, an appendectomy with subsequent subdiaphragmatic abscess would be reported as an organ/space SSI at the intraabdominal specific site (SSI-IAB) when both organ/space SSI and IAB criteria are met. Table 13 lists the specific sites that must be used to differentiate organ/space SSI.</p>";
            organComments += "<p><b>Reporting instructions</b>";
            organComments += "<ul><li>If a patient has an infection in the organ/space being operated on in the first 2-day period of hospitalization and the surgical incision was closed primarily, subsequent continuation of this infection type during the remainder of the surveillance period is considered an organ/space SSI, if organ/space SSI and site-specific infection criteria are met. Rationale: Risk continuing or new infection is considered to be minimal when a surgeon elects to close a wound primarily.</li>";
            organComments += "<li>Report mediastinitis following cardiac surgery that is accompanied by osteomyelitis as SSI-MED rather than SSI-BONE.</li>";
            organComments += "<li>If meningitis (MEN) and a brain abscess (IC) are present together after operation, report as SSI-IC.</li>";
            organComments += "<li>Report CSF shunt infection as SSI-MEN if it occurs within 90 days of placement; if later or after manipulation/access, it is considered CNS-MEN and is not reportable as a SSI.</li>";
            organComments += "<li>Report spinal abscess with meningitis as SSI-MEN following spinal surgery.</li></ul>";

            string sipComments = string.Empty;
            sipComments += "<p><b>Comments</b></p>";
            sipComments += "<p>There are two specific types of superficial incisional SSIs:";
            sipComments += "<br>1. Superficial Incisional Primary (SIP) – a superficial incisional SSI that is identified in the primary incision in a patient that has had an operation with one or more incisions (e.g., C-section incision or chest incision for CBGB)";
            sipComments += "<br>2. Superficial Incisional Secondary (SIS) – a superficial incisional SSI that is identified in the secondary incision in a patient that has had an operation with more than one incision (e.g., donor site [leg] incision for CBGB) </p>";
            sipComments += "<p><b>Reporting instructions</b></p>";
            sipComments += "<ul<li>Do not report a stitch abscess (minimal inflammation and discharge confined to the points of suture penetration) as an infection.</li>";
            sipComments += "<li>Do not report a localized stab wound or pin site infection as SSI. It would be considered either a skin (SKIN) or soft tissue (ST) infection, depending on its depth.</li>";
            sipComments += "<li>\"Cellulitis\", by itself, does not meet criteria for superficial incisional SSI.</li>";
            sipComments += "<li>If the superficial incisional infection involves or extends into the fascial or muscle layers, report as a deep incisional SSI only.</li>";
            sipComments += "<li>If the superficial incisional infection extends into the fascial or muscle layers, report as a deep incisional SSI only.</li>";
            sipComments += "<li>An infected circumcision site in newborns is classified as CIRC. Circumcision is not an NHSN operative procedure.</li>";
            sipComments += "<li>An infected burn wound is classified as BURN.</li></ul>";

            Type.Create("SSI-Surgical Site", "#3111b8", _DataContext)
               .AddCondition("DIP/DIS-Deep incisional surgical site infection")
                   .AddL1RuleSet(3)
                   .SetComment(ddisComments)
                   .SetInstructions("Deep incisional SSI must meet the following criterion:")
                   .AddRule(1)
                    .AddCriteria("Infection occurs within 30 or 90 days after the NHSN operative procedure according to the list in Table 12")
                   .AddRule(1)
                   .SetInstructions("and")
                    .AddCriteria("involves deep soft tissues of the incision (e.g., fascial and muscle layers)")
                   .AddRule(1)
                   .SetInstructions("and patient has at least one of the following:")
                    .AddCriteria("a. purulent drainage from the deep incision")
                    .AddCriteria("b. a deep incision that spontaneously dehisces or is deliberately opened by a surgeon and is culture-positive or not cultured <br>and<br> patient has at least one of the following signs or symptoms: fever (>38&deg;C); localized pain or tenderness. A culture-negative finding does not meet this criterion. ")
                    .AddCriteria("c. an abscess or other evidence of infection involving the deep incision is found on direct examination, during invasive procedure, or by histopathologic examination or imaging test.")
                    .AddCriteria("d. diagnosis of a deep incisional SSI by a surgeon or attending physician.")
               .AddCondition("Organ/space surgical site")
                    .AddL1RuleSet(4)
                    .SetComment(organComments)
                    .SetInstructions("Organ/Space SSI must meet the following criterion:")
                    .AddRule(1)
                        .AddCriteria("Infection occurs within 30 or 90 days after the NHSN operative procedure according to the list in Table 12")
                    .AddRule(1)
                        .SetInstructions("and")
                        .AddCriteria("infection involves any part of the body, excluding the skin incision, fascia, or muscle layers, that is opened or manipulated during the operative procedure")
                    .AddRule(1)
                        .SetInstructions("and patient has at least 1 of the following:")
                        .AddCriteria("a. purulent drainage from a drain that is placed into the organ/space")
                        .AddCriteria("b. organisms isolated from an aseptically-obtained culture of fluid or tissue in the organ/space")
                        .AddCriteria("c. an abscess or other evidence of infection involving the organ/space that is found on direct examination, during invasive procedure, or by histopathologic examination or imaging test")
                        .AddCriteria("d. diagnosis of an organ/space SSI by a surgeon or attending physician.")
                    .AddRule(1)
                        .SetInstructions("and")
                        .AddCriteria("meets at least one criterion for a specific organ/space infection site listed in Table 13")
                .AddCondition("SIS-Superficial incisional surgical site infection")
                    .AddL1RuleSet(3)
                    .SetComment(sipComments)
                    .SetInstructions("Superficial incisional SSI must meet the following criterion:")
                        .AddRule(1)
                            .AddCriteria("Infection occurs within 30 days after any NHSN operative procedure, including those coded as 'OTH'*")
                        .AddRule(1)
                            .SetInstructions("and")
                            .AddCriteria("involves only skin and subcutaneous tissue of the incision")
                        .AddRule(1)
                        .SetInstructions("patient has at least 1 of the following:")
                        .AddCriteria("a. purulent drainage from the superficial incision")
                        .AddCriteria("b. organsims isolated from an aseptically-obtained culture of fluid or tissue from the superficial incision")
                        .AddCriteria("c. superficial incision that is deliberately opened by a surgeon and is culture-positive or not cultured <br> and <br> patient has at least one of the following signs or symptoms of infection: pain or tenderness; localized swelling; redness; or heat. A culture negative finding does not meet this criterion")
                        .AddCriteria("d. diagnosis of superficial incisional SSI by the surgeon or attending physician")
                .AddCondition("SIP-Superficial incisional surgical site infection")
                    .AddL1RuleSet(3)
                    .SetComment(sipComments)
                    .SetInstructions("Superficial incisional SSI must meet the following criterion:")
                        .AddRule(1)
                            .AddCriteria("Infection occurs within 30 days after any NHSN operative procedure, including those coded as 'OTH'*")
                        .AddRule(1)
                            .SetInstructions("and")
                            .AddCriteria("involves only skin and subcutaneous tissue of the incision")
                        .AddRule(1)
                        .SetInstructions("patient has at least 1 of the following:")
                        .AddCriteria("a. purulent drainage from the superficial incision")
                        .AddCriteria("b. organsims isolated from an aseptically-obtained culture of fluid or tissue from the superficial incision")
                        .AddCriteria("c. superficial incision that is deliberately opened by a surgeon and is culture-positive or not cultured <br> and <br> patient has at least one of the following signs or symptoms of infection: pain or tenderness; localized swelling; redness; or heat. A culture negative finding does not meet this criterion")
                        .AddCriteria("d. diagnosis of superficial incisional SSI by the surgeon or attending physician");


            /************************* SST-SKIN AND SOFT TISSUE INFECTION ********************************/

            string burnComments = string.Empty;
            burnComments +=  "<p><b>Comments</b></p>";
            burnComments +=  "<ul<li>Purulence alone at the burn wound site is not adequate for the diagnosis of burn infection; such purulence may reflect incomplete wound care.</li>";
            burnComments +=  "<li>Fever alone in a burn patient is not adequate for the diagnosis of a burn infection because fever may be the result of tissue trauma or the patient may have an infection at another site.</li>";
            burnComments +=  "<li>Surgeons in Regional Burn Centers who take care of burn patients exclusively may require Criterion 1 for diagnosis of burn infection.</li>";
            burnComments +=  "<li>Hospitals with Regional Burn Centers may further divide burn infections into the following: burn wound site, burn graft site, burn donor site, burn donor site-cadaver; NHSN, however, will code all of these as BURN.</li></ul>";

            string decuComments = string.Empty;
            decuComments += "<p><b>Comments</b></p>";
            decuComments += "<ul><li>Purulent drainage alone is not sufficient evidence of an infection.</li>";
            decuComments += "<li>Organisms cultured from the surface of a decubitus ulcer are not sufficient evidence that the ulcer is infected. A properly collected specimen from a decubitus ulcer involves needle aspiration of fluid or biopsy of tissue from the ulcer margin.</li></ul>";


            string skinInfComments = string.Empty;
            skinInfComments += "<p><b>Reporting instructions</b></p>";
            skinInfComments += "<ul><li>Report omphalitis in infants as UMB.</li>";
            skinInfComments += "<li>Report infections of the circumcision site in newborns as CIRC.</li>";
            skinInfComments += "<li>Report pustules in infants as PUST.</li>";
            skinInfComments += "<li>Report infected decubitus ulcers as DECU.</li>";
            skinInfComments += "<li>Report infected burns as BURN.</li>";
            skinInfComments += "<li>Report breast abscesses or mastitis as BRST.</li>";
            skinInfComments += "<li>Even if there are clinical signs or symptoms of localized infection at a vascular access site, but no other infection can be found, the infection is considered a primary BSI.</li></ul>";



            Type.Create("SST-Skin And Soft Tissue", "#6d6c04", _DataContext)
              .AddCondition("BRST-Breast abscess or mastitis")
                  .AddL1RuleSet(1)
                  .SetInstructions("A breast abscess or mastitis must meet at least 1 of the following criteria:")
                  .AddRule(1)
                  .AddCriteria("1. Patient has a positive culture of affected breast tissue or fluid obtained by invasive procedure.")
                  .AddCriteria("2. Patient has a breast abscess or other evidence of infection seen during an invasive procedure or histopathologic examination.")
                  .AddCriteria("3. Patient has fever (>38&deg;C) and local inflammation of the breast <br> and <br> physician diagnosis of breast abscess. ")
              .AddCondition("BURN-Burn")
               .AddL1RuleSet(1)
                   .SetInstructions("Burn infections must meet at least 1 of the following criteria:")
                   .SetComment(burnComments)
                       .AddL2RuleSet(2)
                           .SetInstructions("1.")
                           .AddRule(1)
                               .AddCriteria("Patient has a change in burn wound appearance or character, such as rapid eschar separation, or dark brown, black, or violaceous discoloration of the eschar, or edema at wound margin")
                           .AddRule(1)
                           .SetInstructions("and")
                               .AddCriteria("histologic examination of burn biopsy shows invasion of organisms into adjacent viable tissue.")
                       .AddL2RuleSet(2)
                           .SetInstructions("2.")
                           .AddRule(1)
                               .AddCriteria("Patient has a change in burn wound appearance or character, such as rapid eschar separation, or dark brown, black, or violaceous discoloration of the eschar, or edema at wound margin")
                           .AddRule(1)
                               .SetInstructions("and at least 1 of the following:")
                               .AddCriteria("a. organisms cultured from blood in the absence of other identifiable infection")
                               .AddCriteria("b. isolation of herpes simplex virus, histologic identification of inclusions by light or electron microscopy, or visualization of viral particles by electron microscopy in biopsies or lesion scrapings.")
                       .AddL2RuleSet(2)
                           .SetInstructions("3.")
                           .AddRule(2)
                               .SetInstructions("Patient with a burn has at least 2 of the following signs or symptoms: (* With no other recognized cause)")
                               .AddCriteria("fever (>38&deg;C) or hypothermia (<36&deg;C), hypotension*")
                               .AddCriteria("oliguria* (<20 cc/hr)")
                               .AddCriteria("hyperglycemia at previously tolerated level of dietary carbohydrate*")
                               .AddCriteria("or mental confusion*")
                           .AddRule(1)
                               .SetInstructions("and at least 1 of the following:")
                               .AddCriteria("a. histologic examination of burn biopsy shows invasion of organisms into adjacent viable tissue")
                               .AddCriteria("b. organisms cultured from blood")
                               .AddCriteria("c. isolation of herpes simplex virus, histologic identification of inclusions by light or electron microscopy, or visualization of viral particles by electron microscopy in biopsies or lesion scrapings.")
             .AddCondition("CIRC-Newborn circumcision")
               .AddL1RuleSet(1)
               .SetInstructions("Circumcision infection in a newborn (≤30 days old) must meet at least 1 of the following criteria:")
                   .AddRule(1)
                   .SetInstructions("1.")
                   .AddCriteria("Newborn has purulent drainage from circumcision site.")
                   .AddL2RuleSet(2)
                   .SetInstructions("2.")
                       .AddRule(1)
                       .SetInstructions("Newborn has at least 1 of the following signs or symptoms with no other recognized cause at circumcision site:")
                       .AddCriteria("erythema")
                       .AddCriteria("swelling")
                       .AddCriteria("tenderness")
                       .AddRule(1)
                       .SetInstructions("and")
                       .AddCriteria("pathogen cultured from circumcision site.")
                   .AddL2RuleSet(3)
                       .SetInstructions("3.")
                           .AddRule(1)
                           .SetInstructions("Newborn has at least 1 of the following signs or symptoms with no other recognized cause at circumcision site:")
                           .AddCriteria("erythema")
                           .AddCriteria("swelling")
                           .AddCriteria("tenderness")
                           .AddRule(1)
                           .SetInstructions("and")
                           .AddCriteria("skin contaminant (i.e., diphtheroids [Corynebacterium spp], Bacillus [not B anthracis] spp, Propionibacterium spp, coagulase-negative staphylococci [including S epidermidis], viridans group streptococci, Aerococcus spp, Micrococcus spp) is cultured from circumcision site")
                           .AddRule(1)
                           .SetInstructions("and")
                           .AddCriteria("physician diagnosis of infection or physician institutes appropriate therapy.")
              .AddCondition("DECU-Decubitus ulcer infection, including both superficial and deep")
               .AddL1RuleSet(2)
                   .SetInstructions("Decubitus ulcer infections must meet the following criterion:")
                   .SetComment(decuComments)
                   .AddRule(2)
                       .SetInstructions("Patient has at least 2 of the following signs or symptoms with no other recognized cause:")
                       .AddCriteria("redness")
                       .AddCriteria("tenderness")
                       .AddCriteria("swelling of decubitus wound edges")
                   .AddRule(1)
                       .SetInstructions("at least 1 of the following:")
                       .AddCriteria("a. organisms cultured from properly collected fluid or tissue (see Comments)")
                       .AddCriteria("b. organisms cultured from blood.")
              .AddCondition("PUST-Infant pustulosis")
               .AddL1RuleSet(1)
                   .SetComment("<p><b>Reporting instructions</b></p> <br> Do not report erythema toxicum and noninfectious causes of pustulosis. ")
                   .SetInstructions("Pustulosis in an infant (≤1 year old) must meet at least 1 of the following criteria:")
                   .AddL2RuleSet(2)
                       .SetInstructions("1.")
                       .AddRule(1)
                               .AddCriteria("Infant has 1 or more pustules")
                       .AddRule(1)
                           .SetInstructions("and")
                               .AddCriteria("physician diagnosis of skin infection.")
                   .AddL2RuleSet(2)
                       .SetInstructions("2.")
                       .AddRule(1)
                               .AddCriteria("Infant has 1 or more pustules")
                       .AddRule(1)
                           .SetInstructions("and")
                               .AddCriteria("physician institutes appropriate antimicrobial therapy.")
               .AddCondition("SKIN-Skin infection")
                   .AddL1RuleSet(1)
                   .SetComment(skinInfComments)
                   .SetInstructions("Skin infections must meet at least 1 of the following criteria:")
                       .AddRule(1)
                       .SetInstructions("1.")
                           .AddCriteria("Patient has purulent drainage, pustules, vesicles, or boils.")
                       .AddL2RuleSet(2)
                           .SetInstructions("2.")
                               .AddRule(2)
                               .SetInstructions("Patient has at least 2 of the following signs or symptoms with no other recognized cause:")
                               .AddCriteria("pain or tenderness")
                               .AddCriteria("localized swelling")
                               .AddCriteria("redness")
                               .AddCriteria("heat")
                               .AddRule(1)
                               .SetInstructions("at least 1 of the following:")
                               .AddCriteria("a. organisms cultured from aspirate or drainage from affected site; if organisms are normal skin flora (i.e., diphtheroids [Corynebacterium spp], Bacillus [not B anthracis] spp, Propionibacterium spp, coagulase-negative staphylococci [including S epidermidis], viridans group streptococci, Aerococcus spp, Micrococcus spp), they must be a pure culture")
                               .AddCriteria("b. organisms cultured from blood")
                               .AddCriteria("c. positive laboratory test performed on infected tissue or blood (e.g., antigen tests for herpes simplex, varicella zoster, H influenzae, or N meningitidis)")
                               .AddCriteria("d. multinucleated giant cells seen on microscopic examination of affected tissue")
                               .AddCriteria("e. diagnostic single antibody titer (IgM) or 4-fold increase in paired sera (IgG) for pathogen.")
                .AddCondition("ST-Soft tissue infection")
                   .AddL1RuleSet(1)
                   .SetComment("<p><b>Reporting instructions</b></p><ul><li>Report infected decubitus ulcers as DECU.</li><li>Report infection of deep pelvic tissues as OREP.</li><li>Even if there are clinical signs or symptoms of localized infection at a vascular access site, but no other infection can be found, the infection is considered a primary BSI.</li></ul>")
                   .AddRule(1)
                       .SetInstructions("1.")
                       .AddCriteria("Patient has organisms cultured from tissue or drainage from affected site.")
                   .AddRule(1)
                       .SetInstructions("2.")
                       .AddCriteria("Patient has purulent drainage at affected site.")
                   .AddRule(1)
                       .SetInstructions("3.")
                       .AddCriteria("Patient has an abscess or other evidence of infection seen during an invasive procedure or histopathologic examination.")
                   .AddL2RuleSet(2)
                       .SetInstructions("4.")
                           .AddRule(2)
                           .SetInstructions("Patient has at least 2 of the following signs or symptoms at the affected site with no other recognized cause:")
                           .AddCriteria("localized pain or tenderness")
                           .AddCriteria("redness")
                           .AddCriteria("swelling")
                           .AddCriteria("heat")
                           .AddRule(1)
                           .SetInstructions("and at least 1 of the following:")
                           .AddCriteria("a. organisms cultured from blood")
                           .AddCriteria("b. positive laboratory test performed on blood or urine (e.g., antigen tests for H influenzae, S pneumoniae, N meningitidis, Group B Streptococcus, or Candida spp)")
                           .AddCriteria("c. diagnostic single antibody titer (IgM) or 4-fold increase in paired sera (IgG) for pathogen.")
             .AddCondition("UMB-Oomphalitis")
                   .AddL1RuleSet(1)
                   .SetInstructions("Omphalitis in a newborn (≤30 days old) must meet at least 1 of the following criteria:")
                   .SetComment("<p><b>Reporting instructions</b></p><p>Report infection of the umbilical artery or vein related to umbilical catheterization as VASC if there is no accompanying blood culture or a blood culture is negative</p>")
                       .AddL2RuleSet(2)
                           .SetInstructions("1.")
                           .AddRule(1)
                           .AddCriteria("Patient has erythema and/or serous drainage from umbilicus")
                           .AddRule(1)
                           .SetInstructions("and at least 1 of the following:")
                           .AddCriteria("a. organisms cultured from drainage or needle aspirate")
                           .AddCriteria("b. organisms cultured from blood.")
                       .AddL2RuleSet(1)
                           .AddRule(1)
                           .SetInstructions("2.")
                           .AddCriteria("Patient has both erythema and purulence at the umbilicus.");




           /************************* SYS-SYSTEMIC INFECTION ********************************/

            Type.Create("SYS-Systemic", "#ea13ec", _DataContext)
                .AddCondition("DI-Disseminated infection")
                   .AddL1RuleSet(1)
                   .SetComment("<p><b>Reporting instructions</b></p><ul><li>Use this code for viral infections involving multiple organ systems (e.g., measles, mumps, rubella, varicella, erythema infectiosum). These infections often can be identified by clinical criteria alone. Do not use this code for healthcare–associated infections with multiple metastatic sites, such as with bacterial endocarditis; only the primary site of these infections should be reported.</li><li>Do not report fever of unknown origin (FUO) as DI.</li><li>Report viral exanthems or rash illness as DI.</li></ul>")
                   .AddRule(1)
                   .AddCriteria("Disseminated infection is infection involving multiple organs or systems, without an apparent single site of infection, usually of viral origin, and with signs or symptoms with no other recognized cause and compatible with infectious involvement of multiple organs or systems.");

            /************************* UTI-URINARY TRACT INFECTION ********************************/

            string sutiComment = "";
            sutiComment += "<ul><li>Elements of the criterion must occur within a timeframe that does not exceed a gap of 1 calendar day.</li>";
            sutiComment += "<li>Laboratory cultures reported as “mixed flora” represent at least 2 species of organisms. Therefore an additional organism recovered from the same culture, would represent >2 species of microorganisms. Such a specimen cannot be used to meet the UTI criteria.</li>";
            sutiComment += "<li>Urinary catheter tips should not be cultured and are not acceptable for the diagnosis of a urinary tract infection.</li>";
            sutiComment += "<li>Urine cultures must be obtained using appropriate technique, such as clean catch collection or catheterization. Specimens from indwelling catheters should be aspirated through the disinfected sampling ports.</li>";
            sutiComment += "<li>In infants, urine cultures should be obtained by bladder catheterization or suprapubic aspiration; positive urine cultures from bag specimens are unreliable and should be confirmed by specimens aseptically obtained by catheterization or suprapubic aspiration.</li>";
            sutiComment += "<li>Urine specimens for culture should be processed as soon as possible, preferably within 1 to 2 hours. If urine specimens cannot be processed within 30 minutes of collection, they should be refrigerated, or inoculated into primary isolation medium before transport, or transported in an appropriate urine preservative. Refrigerated specimens should be cultured within 24 hours.</li>";
            sutiComment += "<li>Urine specimen labels should indicate whether or not the patient is symptomatic.</li>";
            sutiComment += "<li>Report secondary bloodstream infection = “Yes” for all cases of Asymptomatic Bacteremic Urinary Tract Infection (ABUTI).</li>";
            sutiComment += "<li>Report only pathogens in both blood and urine specimens for ABUTI.</li>";
            sutiComment += "<li>Report Corynebacterium (urease positive) as either Corynebacterium species unspecified (COS) or as C. urealyticum (CORUR) if speciated.</li></ul>";

            Type.Create("UTI - Urinary tract infection", "#e8e94d", _DataContext)
                .AddCondition("Asymptomatic Bacteremic Urinary Tract Infection (ABUTI)")
                   .AddL1RuleSet(3)
                   .SetComment("<p>*Patient had an indwelling urinary catheter in place for >2 calendar days, with day of device placement being Day 1, and catheter was in place when all elements of this criterion were first present together.</p><p>**Uropathogen microorganisms are: Gram-negative bacilli, Staphylococcus spp., yeasts, beta-hemolytic Streptococcus spp., Enterococcus spp., G. vaginalis, Aerococcus urinae, and Corynebacterium (urease positive)+.</p><p>+Report Corynebacterium (urease positive) as either Corynebacterium species unspecified (COS) or as C. urealyticum (CORUR) if so speciated.</p><p>(See complete list of uropathogen microorganisms.)<p>")
                   .AddRule(1)
                    .AddCriteria("Patient with* or without an indwelling urinary catheter has no signs or symptoms (i.e., for any age patient, no fever (>38&deg;C); urgency; frequency; dysuria; suprapubic tenderness; costovertebral angle pain or tenderness OR for a patient ≤1 year of age; no fever (>38&deg;C core); hypothermia (<36&deg;C core); apnea; bradycardia; dysuria; lethargy; or vomiting)")
                   .AddRule(1)
                    .SetInstructions("and")
                    .AddCriteria("a positive urine culture of ≥10<sup>5</sup> CFU/ml with no more than 2 species of uropathogen microorganisms** (see Comments section below).")
                   .AddRule(1)
                    .SetInstructions("and")
                    .AddCriteria("a positive blood culture with at least 1 matching uropathogen microorganism to the urine culture, or at least 2 matching blood cultures drawn on separate occasions if the matching pathogen is a common skin commensal.")
                .AddCondition("Other Urinary Tract Infection (OUTI)")
                    .AddL1RuleSet(1)
                    .SetInstructions("Other infections of the urinary tract must meet at least 1 of the following criteria:")
                    .SetComment("Report infections following circumcision in newborns as SST-CIRC.")
                        .AddRule(1)
                        .SetInstructions("1.")
                            .AddCriteria("Patient has microorganisms isolated from culture of fluid (other than urine) or tissue from affected site.")
                        .AddRule(1)
                        .SetInstructions("2.")
                            .AddCriteria("Patient has an abscess or other evidence of infection seen on direct examination, during an invasive procedure, or during a histopathologic examination.")
                        .AddL2RuleSet(2)
                            .SetInstructions("3.")
                                .AddRule(2)
                                    .SetInstructions("Patient has at least 2 of the following signs or symptoms: (* With no other recognized cause)")
                                    .AddCriteria("fever (>38&deg;C)")
                                    .AddCriteria("localized pain*")
                                    .AddCriteria("localized tenderness at the involved site*")
                                .AddRule(1)
                                .SetInstructions("and at least 1 of the following:")
                                    .AddCriteria("a. purulent drainage from affected site")
                                    .AddCriteria("b. microorganisms cultured from blood that are compatible with suspected site of infection")
                                    .AddCriteria("c. imaging test evidence of infection (e.g., abnormal ultrasound, CT scan, magnetic resonance imaging [MRI], or radiolabel scan [gallium, technetium]).")
                        .AddL2RuleSet(2)
                            .SetInstructions("4.")
                                .AddRule(1)
                                .SetInstructions("Patient <1 year of age has at least 1 of the following signs or symptoms: (* With no other recognized cause)")
                                    .AddCriteria("fever (>38&deg;C core)")
                                    .AddCriteria("hypothermia (<36&deg;C core)")
                                    .AddCriteria("apnea*")
                                    .AddCriteria("bradycardia*")
                                    .AddCriteria("lethargy*")
                                    .AddCriteria("vomiting*")
                                .AddRule(1)
                                .SetInstructions("and at least 1 of the following:")
                                    .AddCriteria("a. purulent drainage from affected site")
                                    .AddCriteria("b. microorganisms cultured from blood that are compatible with suspected site of infection")
                                    .AddCriteria("c. imaging test evidence of infection, (e.g., abnormal ultrasound, CT scan, magnetic resonance imaging [MRI], or radiolabel scan [gallium, technetium]).")
                .AddCondition("Symptomatic Urinary Tract Infection (SUTI)")
                    .AddL1RuleSet(1)
                    .SetComment(sutiComment)
                    .SetInstructions("Must meet at least 1 of the following criteria")
                        .AddL2RuleSet(1)
                        .SetInstructions("1a")
                            .AddL3RuleSet(3)
                                .SetInstructions("(*With no other recognized cause)")
                                .AddRule(1)
                                .AddCriteria("Patient had an indwelling urinary catheter in place for >2 calendar days, with day of device placement being Day 1, and catheter was in place time when all elements of this criterion were first present together.")
                                .AddRule(1)
                                .SetInstructions("and")
                                .AddCriteria("at least 1 of the following signs or symptoms: fever (>38&deg;C); suprapubic tenderness*; costovertebral angle pain or tenderness*")
                                .AddRule(1)
                                .SetInstructions("and")
                                .AddCriteria("a positive urine culture of ≥10<sup>5</sup> colony-forming units (CFU)/ml with no more than 2 species of microorganisms.")
                            .AddL3RuleSet(3)
                            .SetInstructions("----------- OR ------------------")
                                .AddRule(1)
                                .AddCriteria("Patient had an indwelling urinary catheter in place for >2 calendar days and had it removed the day of or the day before all elements of this criterion were first present together")
                                .AddRule(1)
                                .SetInstructions("and")
                                .AddCriteria("at least 1 of the following signs or symptoms: fever (>38&deg;C); urgency*; frequency*; dysuria*; suprapubic tenderness*; costovertebral angle pain or tenderness*")
                                .AddRule(1)
                                .SetInstructions("and")
                                .AddCriteria("a positive urine culture of ≥10<sup>5</sup> colony-forming units (CFU)/ml with no more than 2 species of microorganisms.")
                        .AddL2RuleSet(1)
                        .SetInstructions("1b")
                            .AddL3RuleSet(3)
                            .AddRule(1)
                                .AddCriteria("Patient did not have an indwelling urinary catheter in place at the time of or the day before all elements of this criterion were first present together")
                            .AddRule(1)
                                .SetInstructions("and has at least 1 of the following signs or symptoms: (*With no other recognized cause)")
                                .AddCriteria("fever (>38&deg;C) in a patient that is ≤65 years of age")
                                .AddCriteria("urgency*")
                                .AddCriteria("frequency*")
                                .AddCriteria("dysuria*")
                                .AddCriteria("suprapubic tenderness*")
                                .AddCriteria("costovertebral angle pain or tenderness*")
                            .AddRule(1)
                                .SetInstructions("and")
                                .AddCriteria("a positive urine culture of ≥10<sup>5</sup> CFU/ml with no more than 2 species of microorganisms")
                        .AddL2RuleSet(1)
                        .SetInstructions("2a")
                            .AddL3RuleSet(4)
                                .AddRule(1)
                                .AddCriteria("Patient had an indwelling urinary catheter in place for >2 calendar days, with day of device placement being Day 1, and catheter was in place when all elements of this criterion were first present together")
                                .AddRule(1)
                                .SetInstructions("at least 1 of the following signs or symptoms: (*With no other recognized cause)")
                                .AddCriteria("fever (>38&deg;C)")
                                .AddCriteria("suprapubic tenderness*")
                                .AddCriteria("costovertebral angle pain or tenderness*")
                                .AddRule(1)
                                .SetInstructions("and at least 1 of the following findings:")
                                .AddCriteria("a. positive dipstick for leukocyte esterase and/or nitrite")
                                .AddCriteria("b. pyuria (urine specimen with ≥10 white blood cells [WBC]/mm of unspun urine or >5 WBC/high power field of spun urine)")
                                .AddCriteria("c. microorganisms seen on Gram’s stain of unspun urine")
                                .AddRule(1)
                                .SetInstructions("and")
                                .AddCriteria("a positive urine culture of ≥10<sup>3</sup> and <10<sup>5</sup> CFU/ml with no more than 2 species of microorganisms")
                            .AddL3RuleSet(4)
                                .SetInstructions("----------- OR ------------------")
                                .AddRule(1)
                                .AddCriteria("Patient with an indwelling urinary catheter in place for >2 calendar days and had it removed the day of or the day before all elements of this criterion were first present together")
                                .AddRule(1)
                                .SetInstructions("and at least 1 of the following signs or symptoms: (*With no other recognized cause)")
                                .AddCriteria("fever (>38&deg;C)")
                                .AddCriteria("urgency*")
                                .AddCriteria("frequency*")
                                .AddCriteria("dysuria*")
                                .AddCriteria("suprapubic tenderness*")
                                .AddCriteria("costovertebral angle pain or tenderness*")
                                .AddRule(1)
                                .SetInstructions("at least 1 of the following findings:")
                                .AddCriteria("a. positive dipstick for leukocyte esterase and/or nitrite")
                                .AddCriteria("b. pyuria (urine specimen with ≥10 WBC/mm of unspun urine or >5 WBC/high power field of spun urine")
                                .AddCriteria("c. microorganisms seen on Gram’s stain of unspun urine")
                                .AddRule(1)
                                .SetInstructions("and")
                                .AddCriteria("a positive urine culture of ≥10<sup>3</sup> and <10<sup>5</sup> CFU/ml with no more than 2 species of microorganisms.")
                        .AddL2RuleSet(4)
                            .SetInstructions("2b")
                            .AddRule(1)
                            .AddCriteria("Patient did not have an indwelling urinary catheter in place at the time of, or the day before all elements of this criterion were first present together")
                            .AddRule(1)
                            .SetInstructions("and has at least 1 of the following signs or symptoms: (*With no other recognized cause)")
                            .AddCriteria("fever (>38&deg;C) in a patient that is ≤65 years of age")
                            .AddCriteria("urgency*")
                            .AddCriteria("frequency*")
                            .AddCriteria("dysuria*")
                            .AddCriteria("suprapubic tenderness*")
                            .AddCriteria("costovertebral angle pain or tenderness*")
                            .AddRule(1)
                            .SetInstructions("and at least 1 of the following findings:")
                            .AddCriteria("a. positive dipstick for leukocyte esterase and/or nitrite")
                            .AddCriteria("b. pyuria (urine specimen with ≥10 WBC/mm of unspun urine or >5 WBC/high power field of spun urine")
                            .AddCriteria("c. microorganisms seen on Gram’s stain of unspun urine")
                            .AddRule(1)
                            .SetInstructions("and")
                            .AddCriteria("a positive urine culture of ≥10<sup>3</sup> and <10<sup>5</sup> CFU/ml with no more than 2 species of microorganisms")
                        .AddL2RuleSet(2)
                            .SetInstructions("3")
                                .AddRule(1)
                                .SetInstructions("Patient ≤1 year of age with** or without an indwelling urinary catheter has at least 1 of the following signs or symptoms: (*With no other recognized cause) (** Patient had an indwelling urinary catheter in place for >2 calendar days, with day of device placement being Day 1, and catheter was in place when all elements of this criterion were first present together.)")
                                .AddCriteria("fever (>38&deg;C core)")
                                .AddCriteria("hypothermia (<36&deg;C core)")
                                .AddCriteria("apnea*")
                                .AddCriteria("bradycardia*")
                                .AddCriteria("dysuria*")
                                .AddCriteria("lethargy*")
                                .AddCriteria("vomiting*")
                                .AddRule(1)
                                .SetInstructions("and")
                                .AddCriteria("a positive urine culture of ≥10<sup>5</sup> CFU/ml with no more than 2 species of microorganisms. Elements of the criterion must occur within a timeframe that does not exceed a gap of 1 calendar day.")
                        .AddL2RuleSet(3)
                            .SetInstructions("4")
                                .AddRule(1)
                                .SetInstructions("Patient ≤1 year of age with** or without an indwelling urinary catheter has at least 1 of the following signs or symptoms: (*With no other recognized cause) (** Patient had an indwelling urinary catheter in place for >2 calendar days, with day of device placement being Day 1, and catheter was in place when all elements of this criterion were first present together.)")
                                .AddCriteria("fever (>38&deg;C core)")
                                .AddCriteria("hypothermia (<36&deg;C core)")
                                .AddCriteria("apnea*")
                                .AddCriteria("bradycardia*")
                                .AddCriteria("dysuria*")
                                .AddCriteria("lethargy*")
                                .AddCriteria("vomiting*")
                                .AddRule(1)
                                .SetInstructions("and at least 1 of the following findings:")
                                .AddCriteria("a. positive dipstick for leukocyte esterase and/or nitrite")
                                .AddCriteria("b. pyuria (urine specimen with ≥10 WBC/mm of unspun urine or >5 WBC/high power field of spun urine")
                                .AddCriteria("c. microorganisms seen on Gram’s stain of unspun urine")
                                .AddRule(1)
                                .SetInstructions("and")
                                .AddCriteria("a positive urine culture of between ≥10<sup>3</sup> and <10<sup>5</sup> CFU/ml with no more than two species of microorganisms.");

                                
                                


            /************************* VAE – VENTILATOR-ASSOCIATED EVENT ********************************/

            Type.Create("VAE–Ventilator-Associated Event", "#fe2c02" , _DataContext)
                .AddCondition("VAC – Ventilator-Associated Condition")
                    .AddL1RuleSet(2)
                    .AddRule(1)
                        .AddCriteria("Patient has a baseline period of stability or improvement on the ventilator, defined by ≥2 calendar days of stable or decreasing daily minimum FiO<sub>2</sub> or PEEP values. The baseline period is defined as the two calendar days immediately preceding the first day of increased daily minimum PEEP or FiO<sub>2</sub>.")
                    .AddRule(1)
                    .SetInstructions("and After a period of stability or improvement on the ventilator, the patient has at least one of the following indicators of worsening oxygenation:")
                        .AddCriteria("1. Increase in daily minimum FiO<sub>2</sub> of ≥0.20 (20 points) over the daily minimum FiO<sub>2</sub> in the baseline period, sustained for ≥2 calendar days.")
                        .AddCriteria("2. Increase in daily minimum PEEP values of ≥3 cmH<sub>2</sub>O over the daily minimum PEEP in the baseline period, sustained for ≥2 calendar days.")
                .AddCondition("IVAC – Infection-related Ventilator-Associated Complication")
                    .AddL1RuleSet(2)
                        .AddRule(1)
                            .AddCriteria("Patient meets criteria for VAC")
                        .AddRule(2)
                            .SetInstructions("and on or after calendar day 3 of mechanical ventilation and within 2 calendar days before or after the onset of worsening oxygenation, the patient meets both of the following criteria:")
                            .AddCriteria("1. Temperature >38&deg;C or <36&deg;C, OR white blood cell count ≥12,000 cells/mm or ≤4,000 cells/mm")
                            .AddCriteria("2. A new antimicrobial agent(s) <a href=\"#\" onclick=\"criteriaResource('cdc-nhsn-table-15')\">(Table 15)</a> is started, and is continued for ≥4 calendar days.")
                .AddCondition("Possible VAP – Possible Ventilator-Associated Pneumonia")
                    .AddL1RuleSet(2)
                    .AddRule(1)
                        .AddCriteria("Patient meets criteria for VAC and IVAC")
                    .AddRule(1)
                        .SetInstructions("and on or after calendar day 3 of mechanical ventilation and within 2 calendar days before or after the onset of worsening oxygenation, ONE of the following criteria is met:")
                        .AddCriteria("1. Purulent respiratory secretions (from one or more specimen collections) <br> a. Defined as secretions from the lungs, bronchi, or trachea that contain ≥25 neutrophils and ≤10 squamous epithelial cells per low power field [lpf, x100]. <br> b. If the laboratory reports semi-quantitative results, those results must be equivalent to the above quantitative thresholds.")
                        .AddCriteria("2. Positive culture (qualitative, semi-quantitative or quantitative) of sputum*, endotracheal aspirate*, bronchoalveolar lavage*, lung tissue, or protected specimen brushing* <br*Excludes the following: <br>-Normal respiratory/oral flora, mixed respiratory/oral flora or equivalent <br>-Candida species or yeast not otherwise specified <br>-Coagulase-negative Staphylococcus species <br>-Enterococcus species ")
                .AddCondition("Probable VAP – Probable Ventilator-Associated Pneumonia")
                    .AddL1RuleSet(2)
                    .AddRule(1)
                        .AddCriteria("Patient meets criteria for VAC and IVAC")
                    .AddL2RuleSet(1)
                        .SetInstructions("and on or after calendar day 3 of mechanical ventilation and within 2 calendar days before or after the onset of worsening oxygenation, ONE of the following criteria is met:")
                            .AddL3RuleSet(2)
                                .SetInstructions("1.")
                                    .AddRule(1)
                                        .AddCriteria("Purulent respiratory secretions (from one or more specimen collections—and defined as for possible VAP)")
                                    .AddRule(1)
                                        .SetInstructions("and one of the following: (*Same organism exclusions as noted for Possible VAP.)")
                                        .AddCriteria("a. Positive culture of endotracheal aspirate*, ≥10<sup>5</sup> CFU/ml or equivalent semi-quantitative result")
                                        .AddCriteria("b. Positive culture of bronchoalveolar lavage*, ≥10<sup>4</sup> CFU/ml or equivalent semi-quantitative result")
                                        .AddCriteria("c. Positive culture of lung tissue, ≥10<sup>4</sup> CFU/g or equivalent semi-quantitative result")
                                        .AddCriteria("d. Positive culture of protected specimen brush*, ≥10<sup>3</sup> CFU/ml or equivalent semi-quantitative result")
                            .AddL3RuleSet(1)
                                .SetInstructions("2.")
                                .AddRule(1)
                                .SetInstructions("One of the following (without requirement for purulent respiratory secretions):")
                                    .AddCriteria("a. Positive pleural fluid culture (where specimen was obtained during thoracentesis or initial placement of chest tube and NOT from an indwelling chest tube)")
                                    .AddCriteria("b. Positive lung histopathology")
                                    .AddCriteria("c. Positive diagnostic test for Legionella spp.")
                                    .AddCriteria("d. Positive diagnostic test on respiratory secretions for influenza virus, respiratory syncytial virus, adenovirus, parainfluenza virus, rhinovirus, human metapneumovirus, coronavirus");




        }
    }





        public class Type
        {
            public IStatelessDataContext _DataContext;
            public InfectionType _iType;
            private const int DEF_ID = 2;
            public static int _sortCount = 0;

            public static string CleanChars(string val)
            {
                val = val.Replace(" >1", " &gt;1");
                val = val.Replace(" >2", " &gt;2");
                val = val.Replace(" >3", " &gt;3");
                val = val.Replace(" >4", " &gt;4");
                val = val.Replace(" >5", " &gt;5");
                val = val.Replace(" >6", " &gt;6");
                val = val.Replace(" >7", " &gt;7");
                val = val.Replace(" >8", " &gt;8");
                val = val.Replace(" >9", " &gt;9");
                val = val.Replace(" >0", " &gt;0");

                val = val.Replace("(>1", "(&gt;1");
                val = val.Replace("(>2", "(&gt;2");
                val = val.Replace("(>3", "(&gt;3");
                val = val.Replace("(>4", "(&gt;4");
                val = val.Replace("(>5", "(&gt;5");
                val = val.Replace("(>6", "(&gt;6");
                val = val.Replace("(>7", "(&gt;7");
                val = val.Replace("(>8", "(&gt;8");
                val = val.Replace("(>9", "(&gt;9");
                val = val.Replace("(>0", "(&gt;0");

                val = val.Replace(" <1", " &lt;1");
                val = val.Replace(" <2", " &lt;2");
                val = val.Replace(" <3", " &lt;3");
                val = val.Replace(" <4", " &lt;4");
                val = val.Replace(" <5", " &lt;5");
                val = val.Replace(" <6", " &lt;6");
                val = val.Replace(" <7", " &lt;7");
                val = val.Replace(" <8", " &lt;8");
                val = val.Replace(" <9", " &lt;9");
                val = val.Replace(" <0", " &lt;0");


                val = val.Replace("(<1", "(&lt;1");
                val = val.Replace("(<2", "(&lt;2");
                val = val.Replace("(<3", "(&lt;3");
                val = val.Replace("(<4", "(&lt;4");
                val = val.Replace("(<5", "(&lt;5");
                val = val.Replace("(<6", "(&lt;6");
                val = val.Replace("(<7", "(&lt;7");
                val = val.Replace("(<8", "(&lt;8");
                val = val.Replace("(<9", "(&lt;9");
                val = val.Replace("(<0", "(&lt;0");

                return val;
            }

            public static Type Create(string name, string color, IStatelessDataContext d)
            {
                Type._sortCount++;

                var t = new Type();
                t._DataContext = d;
                t._iType = new InfectionType(name);
                t._iType.Definition = d.Fetch<InfectionDefinition>(DEF_ID);
                t._iType.SortOrder = Type._sortCount;
                t._iType.UsedForEmployees = false;
                t._iType.Color = color;
                t._DataContext.Insert(t._iType);
                return t;
            }

            public Condition AddCondition(string name)
            {
                var c = new Condition();
                c._DataContext = _DataContext;
                c._iType = _iType;
                c._iCondition = new InfectionSite(name, _iType);
                c._pType = this;
                _DataContext.Insert(c._iCondition);
                return c;
            }
        }


        public class Condition
        {
            public IStatelessDataContext _DataContext;
            public InfectionType _iType;
            public InfectionSite _iCondition;
            public Type _pType;

            public Condition AddCondition(string name)
            {
                return _pType.AddCondition(name);
            }

            public L1RuleSet AddL1RuleSet(int minRules)
            {
                var rs = new L1RuleSet();
                rs._DataContext = _DataContext;
                rs._iRuleSet = new InfectionCriteriaRuleSet();
                rs._iRuleSet.MinimumRuleCount = minRules;
                rs._pCondition = this;
                _DataContext.Insert(rs._iRuleSet);
                _iCondition.RuleSet = rs._iRuleSet;
                _DataContext.Update(_iCondition);
                return rs;
            }
        }

        public class L1RuleSet
        {
            public IStatelessDataContext _DataContext;
            public InfectionCriteriaRuleSet _iRuleSet;
            public Condition _pCondition;

            public L1RuleSet SetComment(string val)
            {
                _iRuleSet.CommentsText = val;
                _DataContext.Update(_iRuleSet);
                return this;
            }

            public L1RuleSet SetInstructions(string val)
            {
                val = Type.CleanChars(val);
                _iRuleSet.InstructionsText = val;
                _DataContext.Update(_iRuleSet);
                return this;
            }

            public L1RuleSet AddL1RuleSet(int minRules)
            {
                return _pCondition.AddL1RuleSet(minRules);
            }

            public L2RuleSet AddL2RuleSet(int minRules)
            {
                var rs = new L2RuleSet();
                rs._DataContext = _DataContext;
                rs._iRuleSet = new InfectionCriteriaRuleSet();
                rs._iRuleSet.ParentCriteriaRuleSet = _iRuleSet;
                rs._iRuleSet.MinimumRuleCount = minRules;
                rs._pRuleSet = this;
                _DataContext.Insert(rs._iRuleSet);
                return rs;
            }

            public L1Rule AddRule(int minCriteria)
            {
                var r = new L1Rule();
                r._DataContext = _DataContext;
                r._iRule = new InfectionCriteriaRule(_iRuleSet);
                r._iRule.MinimumCriteriaCount = minCriteria;
                r._pRuleSet = this;
                _DataContext.Insert(r._iRule);
                return r;

            }

            public Condition AddCondition(string name)
            {
                return _pCondition.AddCondition(name);
            }

        }


        public class L2RuleSet
        {
            public IStatelessDataContext _DataContext;
            public InfectionCriteriaRuleSet _iRuleSet;
            public L1RuleSet _pRuleSet;

            public L2RuleSet SetComment(string val)
            {
                _iRuleSet.CommentsText = val;
                _DataContext.Update(_iRuleSet);
                return this;
            }

            public L2RuleSet SetInstructions(string val)
            {
                val = Type.CleanChars(val);
                _iRuleSet.InstructionsText = val;
                _DataContext.Update(_iRuleSet);
                return this;
            }

            public L2RuleSet AddL2RuleSet(int minRules)
            {
                return _pRuleSet.AddL2RuleSet(minRules);
            }

            public L3RuleSet AddL3RuleSet(int minRules)
            {
                var rs = new L3RuleSet();
                rs._DataContext = _DataContext;
                rs._iRuleSet = new InfectionCriteriaRuleSet();
                rs._iRuleSet.ParentCriteriaRuleSet = _iRuleSet;
                rs._iRuleSet.MinimumRuleCount = minRules;
                rs._pRuleSet = this;
                _DataContext.Insert(rs._iRuleSet);
                return rs;
            }

            public L2Rule AddRule(int minCriteria)
            {
                var r = new L2Rule();
                r._DataContext = _DataContext;
                r._iRule = new InfectionCriteriaRule(_iRuleSet);
                r._iRule.MinimumCriteriaCount = minCriteria;
                r._pRuleSet = this;
                _DataContext.Insert(r._iRule);
                return r;

            }

            public Condition AddCondition(string name)
            {
                return _pRuleSet.AddCondition(name);
            }

        }


        public class L3RuleSet
        {
            public IStatelessDataContext _DataContext;
            public InfectionCriteriaRuleSet _iRuleSet;
            public L2RuleSet _pRuleSet;

            public L3RuleSet SetComment(string val)
            {
                _iRuleSet.CommentsText = val;
                _DataContext.Update(_iRuleSet);
                return this;
            }

            public L3RuleSet SetInstructions(string val)
            {
                val = Type.CleanChars(val);
                _iRuleSet.InstructionsText = val;
                _DataContext.Update(_iRuleSet);
                return this;
            }

            public L2RuleSet AddL2RuleSet(int minRules)
            {
                return _pRuleSet.AddL2RuleSet(minRules);
            }

            public L3RuleSet AddL3RuleSet(int minRules)
            {
                return _pRuleSet.AddL3RuleSet(minRules);
            }

            public L3Rule AddRule(int minCriteria)
            {
                var r = new L3Rule();
                r._DataContext = _DataContext;
                r._iRule = new InfectionCriteriaRule(_iRuleSet);
                r._iRule.MinimumCriteriaCount = minCriteria;
                r._pRuleSet = this;
                _DataContext.Insert(r._iRule);
                return r;

            }

            public Condition AddCondition(string name)
            {
                return _pRuleSet.AddCondition(name);
            }

        }

        public class L1Rule
        {
            public IStatelessDataContext _DataContext;
            public L1RuleSet _pRuleSet;
            public InfectionCriteriaRule _iRule;

            public L1Rule AddCriteria(string val)
            {
                val = Type.CleanChars(val);

                var c = new Domain.Models.InfectionCriteria(_iRule, val);
                _DataContext.Insert(c);
                return this;
            }

            public L1Rule SetInstructions(string val)
            {
                val = Type.CleanChars(val);
                _iRule.InstructionsText = val;
                _DataContext.Update(_iRule);
                return this;
            }


            public L1Rule AddRule(int minCriteria)
            {
                return _pRuleSet.AddRule(minCriteria);
            }

            public L2RuleSet AddL2RuleSet(int minRules)
            {
                return _pRuleSet.AddL2RuleSet(minRules);
            }

            public Condition AddCondition(string name)
            {
                return _pRuleSet.AddCondition(name);
            }
        }


        public class L2Rule
        {
            public IStatelessDataContext _DataContext;
            public L2RuleSet _pRuleSet;
            public InfectionCriteriaRule _iRule;

            public L2Rule AddCriteria(string val)
            {
                val = Type.CleanChars(val);

                var c = new Domain.Models.InfectionCriteria(_iRule, val);
                _DataContext.Insert(c);
                return this;
            }

            public L2Rule SetInstructions(string val)
            {
                val = Type.CleanChars(val);
                _iRule.InstructionsText = val;
                _DataContext.Update(_iRule);
                return this;
            }


            public L2Rule AddRule(int minCriteria)
            {
                return _pRuleSet.AddRule(minCriteria);
            }

            public L3RuleSet AddL3RuleSet(int minRules)
            {
                return _pRuleSet.AddL3RuleSet(minRules);
            }

            public L2RuleSet AddL2RuleSet(int minRules)
            {
                return _pRuleSet.AddL2RuleSet(minRules);
            }

            public Condition AddCondition(string name)
            {
                return _pRuleSet.AddCondition(name);
            }
        }

        public class L3Rule
        {
            public IStatelessDataContext _DataContext;
            public L3RuleSet _pRuleSet;
            public InfectionCriteriaRule _iRule;

            public L3Rule AddCriteria(string val)
            {
                val = Type.CleanChars(val);

                var c = new Domain.Models.InfectionCriteria(_iRule, val);
                _DataContext.Insert(c);
                return this;
            }

            public L3Rule SetInstructions(string val)
            {
                val = Type.CleanChars(val);
                _iRule.InstructionsText = val;
                _DataContext.Update(_iRule);
                return this;
            }


            public L3Rule AddRule(int minCriteria)
            {
                return _pRuleSet.AddRule(minCriteria);
            }


            public L2RuleSet AddL2RuleSet(int minRules)
            {
                return _pRuleSet.AddL2RuleSet(minRules);
            }

            public L3RuleSet AddL3RuleSet(int minRules)
            {
                return _pRuleSet.AddL3RuleSet(minRules);
            }

            public Condition AddCondition(string name)
            {
                return _pRuleSet.AddCondition(name);
            }
        }
}
