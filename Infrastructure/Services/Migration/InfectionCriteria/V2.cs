using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Persistence.NHibernate;
using RedArrow.Framework.Logging;
using SnyderIS.sCore.Persistence;


namespace IQI.Intuition.Infrastructure.Services.Migration.InfectionCriteria
{
    public class V2 : IConsoleService
    {

        private IStatelessDataContext _DataContext;

        private InfectionType _SkinSoftTissueType;
        private InfectionSite _SkinCellulitisCondition;
        private InfectionSite _SkinScabies;
        private InfectionSite _SkinFungal;
        private InfectionSite _SkinOral;
        private InfectionSite _SkinHerpesSimplex;
        private InfectionSite _SkinHerpesZoster;
        private InfectionSite _SkinConjunctivitis;

        private InfectionSite _GastroNorovirus;
        private InfectionSite _GastroClostridium;

        public V2(IStatelessDataContext dataContext)
        {
            _DataContext = dataContext;
        }

        public void Run(string[] args)
        {

            /* Setup new types and conditions */

            //Skin, Soft Tissue and Mucosal Infection
            _SkinSoftTissueType = _DataContext.CreateQuery<InfectionType>()
                .FilterBy(x => x.Name == "Skin, Soft Tissue and Mucosal Infection")
                .FetchAll().FirstOrDefault();

            if (_SkinSoftTissueType == null)
            {
                _SkinSoftTissueType = new InfectionType("Skin, Soft Tissue and Mucosal Infection");
                _SkinSoftTissueType.SortOrder = 30;
                _DataContext.Insert(_SkinSoftTissueType);

                _SkinCellulitisCondition = new InfectionSite("Cellulitis, soft tissue, or wound infection", _SkinSoftTissueType);
                _SkinScabies = new InfectionSite("Scabies", _SkinSoftTissueType);
                _SkinFungal = new InfectionSite("Fungal skin infection", _SkinSoftTissueType);
                _SkinOral = new InfectionSite("Oral candidiasis", _SkinSoftTissueType);
                _SkinHerpesSimplex = new InfectionSite("Herpes simplex infection", _SkinSoftTissueType);
                _SkinHerpesZoster = new InfectionSite("Herpes zoster infection", _SkinSoftTissueType);
                _SkinConjunctivitis = new InfectionSite("Conjunctivitis", _SkinSoftTissueType);

                _DataContext.Insert(_SkinCellulitisCondition);
                _DataContext.Insert(_SkinScabies);
                _DataContext.Insert(_SkinFungal);
                _DataContext.Insert(_SkinOral);
                _DataContext.Insert(_SkinHerpesSimplex);
                _DataContext.Insert(_SkinHerpesZoster);
                _DataContext.Insert(_SkinConjunctivitis);
            }
            else
            {
                _SkinCellulitisCondition = _DataContext.CreateQuery<InfectionSite>()
                    .FilterBy(x => x.Type.Id == _SkinSoftTissueType.Id && x.Name == "Cellulitis, soft tissue, or wound infection")
                    .FetchAll().FirstOrDefault();

                _SkinScabies = _DataContext.CreateQuery<InfectionSite>()
                    .FilterBy(x => x.Type.Id == _SkinSoftTissueType.Id && x.Name == "Scabies")
                    .FetchAll().FirstOrDefault();

                _SkinFungal = _DataContext.CreateQuery<InfectionSite>()
                    .FilterBy(x => x.Type.Id == _SkinSoftTissueType.Id && x.Name == "Fungal skin infection")
                    .FetchAll().FirstOrDefault();

                _SkinOral = _DataContext.CreateQuery<InfectionSite>()
                    .FilterBy(x => x.Type.Id == _SkinSoftTissueType.Id && x.Name == "Oral candidiasis")
                    .FetchAll().FirstOrDefault();

                _SkinHerpesSimplex = _DataContext.CreateQuery<InfectionSite>()
                    .FilterBy(x => x.Type.Id == _SkinSoftTissueType.Id && x.Name == "Herpes simplex infection")
                    .FetchAll().FirstOrDefault();

                _SkinHerpesZoster = _DataContext.CreateQuery<InfectionSite>()
                    .FilterBy(x => x.Type.Id == _SkinSoftTissueType.Id && x.Name == "Herpes zoster infection")
                    .FetchAll().FirstOrDefault();

                _SkinConjunctivitis = _DataContext.CreateQuery<InfectionSite>()
                    .FilterBy(x => x.Type.Id == _SkinSoftTissueType.Id && x.Name == "Conjunctivitis")
                    .FetchAll().FirstOrDefault();
            }

            _GastroNorovirus = _DataContext.CreateQuery<InfectionSite>()
                .FilterBy(x => x.Type.Id == 810 && x.Name == "Norovirus Gastroenteritis")
                .FetchAll().FirstOrDefault();

            if (_GastroNorovirus == null)
            {
                _GastroNorovirus = new InfectionSite("Norovirus Gastroenteritis", _DataContext.Fetch<InfectionType>(810));
                _DataContext.Insert(_GastroNorovirus);
            }

            _GastroClostridium = _DataContext.CreateQuery<InfectionSite>()
                .FilterBy(x => x.Type.Id == 810 && x.Name == "Clostridium difficile Infection")
                .FetchAll().FirstOrDefault();

            if (_GastroClostridium == null)
            {
                _GastroClostridium = new InfectionSite("Clostridium difficile Infection", _DataContext.Fetch<InfectionType>(810));
                _DataContext.Insert(_GastroClostridium);
            }

            /* Misc Cleanup */

            var coldType = _DataContext.Fetch<InfectionSite>(909);
            coldType.Name = "Common Cold Syndrome or Pharyngitis";
            _DataContext.Update(coldType);

            var respType = _DataContext.Fetch<InfectionSite>(912);
            respType.Name = "Lower respiratory tract (bronchitis or tracheobronchitis)";
            _DataContext.Update(respType);

            _SkinOral.Name = "Fungal Oral or Perioral";
            _DataContext.Update(_SkinOral);

            /* Hide old types */

            var sType = _DataContext.Fetch<InfectionType>(812);
            sType.IsHidden = true;
            _DataContext.Update(sType);

            var eType = _DataContext.Fetch<InfectionType>(811);
            eType.IsHidden = true;
            _DataContext.Update(eType);

            var gType = _DataContext.Fetch<InfectionType>(810);
            gType.Name = "Gastrointestinal Infection";
            _DataContext.Update(gType);

            /* Build new criteria */


            A1();
            A2();
            A3();
            A4();
            A5();
            A6();
            A7();
            A10();
            A11();
            A12();
            A13();
            A14();
            A15();
            A16();
            A17();
            A18();
        }


        private void stub()
        {

            var rulesset = new InfectionCriteriaRuleSet();
            rulesset.CommentsText = "";
            rulesset.MinimumRuleCount = 2;
            _DataContext.Insert(rulesset);

            addCriteria(rulesset, 1, string.Empty,
                "",
                "",
                "",
                "");

            addCriteria(rulesset, 1, "AND",
                "");

        }

        //Type:  Urinary Tract Infection Condition:  UTI with Catheter	
        private void A1()
        {

            var rulesset = new InfectionCriteriaRuleSet();
            rulesset.CommentsText = "Recent catheter trauma, catheter obstruction, or new onset hematuria are useful localizing signs that are consistent with UTI but are not necessary for diagnosis.";
            rulesset.CommentsText += "<br>";
            rulesset.CommentsText += "<br>";
            rulesset.CommentsText += "Urinary catheter specimens for culture should be ";			
            rulesset.CommentsText += "collected following replacement of the catheter (if ";
            rulesset.CommentsText += "current catheter has been in place for >14 days).	";	
            
            rulesset.MinimumRuleCount = 2;
            _DataContext.Insert(rulesset);

            addCriteria(rulesset, 1, "Must have at least 1 of the following sign or symptom subcriteria",
                "Fever, rigors, or new-onset hypotension, with no alternate site of infection",
                "Either acute change in mental status or acute functional decline, with no alternate diagnosis and leukocytosis",
                "New-onset suprapubic pain or costovertebral angle pain	or tenderness",
                "Purulent discharge from around the catheter or acute pain, swelling, or tenderness of the testes, epididymis, or prostate");

            addCriteria(rulesset, 1, "AND",
                "Urinary catheter specimen culture with at least 10<sup>5</sup> cfu/mL of any organism(s)");

            assign(rulesset, 914);
        }

        //Type:  Respiratory Tract Infection			Condition:  Common Cold Syndrome or Pharyngitis		
        private void A2()
        {

            var rulesset = new InfectionCriteriaRuleSet();
            rulesset.CommentsText = "Fever may or may not be present. Symptoms must be new and not attributable to allergies.";

            rulesset.MinimumRuleCount = 1;
            _DataContext.Insert(rulesset);

            addCriteria(rulesset, 2, "Must have 2 of the following criteria:",
                "Runny nose or sneezing",
                "Stuffy nose (ie, congestion)",
                "Sore throat or hoarseness or difficulty in swallowing",
                "Dry cough",
                "Swollen or tender glands in the neck (cervical	lymphadenopathy)");

            assign(rulesset, 909);
        }

        // Type:  Respiratory Tract Infection			Condition:  Influenza-like Illness		
        private void A3()
        {

            var rulesset = new InfectionCriteriaRuleSet();
            rulesset.CommentsText = "If criteria for influenza-like illness and another upper or";
            rulesset.CommentsText += " lower RTI are met at the same time, only the diagnosis";
            rulesset.CommentsText += " of influenza-like illness should be recorded. Because of";
            rulesset.CommentsText += " increasing uncertainty surrounding the timing of the";
            rulesset.CommentsText += " start of influenza season, the peak of influenza activity,";
            rulesset.CommentsText += " and the length of the season, “seasonality” is no longer";
            rulesset.CommentsText += " a criterion to define influenza-like illness.";

            rulesset.MinimumRuleCount = 2;
            _DataContext.Insert(rulesset);

            addCriteria(rulesset, 1, "Must have:",
                "Fever");

            addCriteria(rulesset, 3, "AND At least 3 of the following influenza-like illness subcriteria",
                "Chills",
                "New headache or eye pain",
                "Myalgias or body aches",
                "Malaise or loss of appetite",
                "Sore throat",
                "New or increased dry cough");

            assign(rulesset, 910);
        }

        // Type:  Respiratory Tract Infection			Condition:  Pneumonia	
        private void A4()
        {

            var rulesset = new InfectionCriteriaRuleSet();
            rulesset.CommentsText = "For both pneumonia and lower RTI, the presence of";
            rulesset.CommentsText += " underlying conditions that could mimic the";
            rulesset.CommentsText += " presentation of a RTI (eg, congestive heart failure or";
            rulesset.CommentsText += " interstitial lung diseases) should be excluded by a";
            rulesset.CommentsText += " review of clinical records and an assessment of";
            rulesset.CommentsText += " presenting symptoms and signs.";

            rulesset.MinimumRuleCount = 3;
            _DataContext.Insert(rulesset);

            addCriteria(rulesset, 1, "Must have:",
                "Interpretation of a chest radiograph as demonstrating pneumonia or the presence of a new infiltrate");

            addCriteria(rulesset, 1, "AND At least 1 of the following respiratory subcriteria",
                "New or increased cough",
                "New or increased sputum production",
                "O<sub>2</sub> saturation < 94% on room air or a reduction in O<sub>2</sub> saturation of > 3% from baseline",
                "New or changed lung examination abnormalities",
                "Pleuritic chest pain",
                "Respiratory rate of > 25 breaths/min");

            addCriteria(rulesset, 1, "AND At least 1 of the Constitutional Criteria",
                "Fever",
                "Leukocytosis",
                "Acute change in mental status from baseline",
                "Acute Functional Decline");

            assign(rulesset, 911);

        }

        // Type:  Respiratory Tract Infection			Condition:  Lower respiratory tract (bronchitis or tracheobronchitis)						
        private void A5()
        {

            var rulesset = new InfectionCriteriaRuleSet();
            rulesset.CommentsText = "For both pneumonia and lower RTI, the presence of";
            rulesset.CommentsText += " underlying conditions that could mimic the";
            rulesset.CommentsText += " presentation of a RTI (eg, congestive heart failure or";
            rulesset.CommentsText += " interstitial lung diseases) should be excluded by a";
            rulesset.CommentsText += " review of clinical records and an assessment of";
            rulesset.CommentsText += " presenting symptoms and signs.";
            rulesset.MinimumRuleCount = 3;
            _DataContext.Insert(rulesset);

            addCriteria(rulesset, 1, "Must have:",
                "Chest radiograph not performed or negative results for	pneumonia or new infiltrate");

            addCriteria(rulesset, 2, "AND At least 2 of the following respiratory subcriteria",
                "New or increased cough",
                "New or increased sputum production",
                "O<sub>2</sub> saturation <94% on room air or a reduction in O<sub>2</sub> saturation of > 3% from baseline",
                "New or changed lung examination abnormalities",
                "Pleuritic chest pain",
                "Respiratory rate of ≥25 breaths/min");

            addCriteria(rulesset, 1, "AND At least 1 of the Constitutional Criteria",
                "Fever",
                "Leukocytosis",
                "Acute change in mental status from baseline",
                "Acute Functional Decline");

            assign(rulesset, 912);

        }

        // Type:  Skin, Soft Tissue and Mucosal Infection					Condition:  Cellulitis, soft tissue, or wound infection					
        private void A6()
        {

            var rulesset = new InfectionCriteriaRuleSet();
            rulesset.CommentsText = "Presence of organisms cultured from the surface (eg,";
            rulesset.CommentsText += " superficial swab sample) of a wound is not sufficient";
            rulesset.CommentsText += " evidence that the wound is infected. More than 1";
            rulesset.CommentsText += " resident with streptococcal skin infection from the";
            rulesset.CommentsText += " same serogroup (eg, A, B, C, G) in a long-term care";
            rulesset.CommentsText += " facility (LTCF) may indicate an outbreak.";

            rulesset.MinimumRuleCount = 1;
            _DataContext.Insert(rulesset);

            addCriteria(rulesset, 1, "Must have at least 1 of the following criteria:",
                "Pus present at a wound, skin, or soft tissue site");

            addCriteria(rulesset, 4, "OR New or increasing presence of at least 4 of the following sign or symptom subcriteria",
                "Heat at the affected site",
                "Redness at the affected site",
                "Swelling at the affected site",
                "Tenderness or pain at the affected site",
                "Serous drainage at the affected site",
                "One constitutional criterion");

            assign(rulesset, _SkinCellulitisCondition.Id);
        }

        //Type:  Skin, Soft Tissue and Mucosal Infection					Condition:  Scabies	
        private void A7()
        {

            var rulesset = new InfectionCriteriaRuleSet();
            rulesset.CommentsText = "An epidemiologic linkage to a case can be considered if";
            rulesset.CommentsText += " there is evidence of geographic proximity in the facility,";
            rulesset.CommentsText += " temporal relationship to the onset of symptoms, or";
            rulesset.CommentsText += " evidence of common source of exposure (ie, shared";
            rulesset.CommentsText += " caregiver). Care must be taken to rule out rashes due";
            rulesset.CommentsText += " to skin irritation, allergic reactions, eczema, and other";
            rulesset.CommentsText += " noninfectious skin conditions";

            rulesset.MinimumRuleCount = 2;
            _DataContext.Insert(rulesset);

            addCriteria(rulesset, 1, "Must have:",
                "A maculopapular and/or itching rash");

            addCriteria(rulesset, 1, "AND At least 1 of the following scabies subcriteria",
                "Physician diagnosis",
                "Laboratory confirmation (scraping or biopsy)",
                "Epidemiologic linkage to a case of scabies with laboratory confirmation");

            assign(rulesset, _SkinScabies.Id);
        }

        //Type:  Skin, Soft Tissue and Mucosal Infection					Condition:  Conjunctivitis		
        private void A10()
        {

            var rulesset = new InfectionCriteriaRuleSet();
            rulesset.CommentsText = "Conjunctivitis symptoms (“pink eye”) should not be due to allergic reaction or trauma.";
            rulesset.MinimumRuleCount = 1;
            _DataContext.Insert(rulesset);

            addCriteria(rulesset, 1, "Must have 1 of of the following criteria:",
                "Pus appearing from 1 or both eyes, present for at least 24 hours",
                "New or increased conjunctival erythema, with or without itching",
                "New or increased conjunctival pain, present for at least 24 hours");

            assign(rulesset, _SkinConjunctivitis.Id);
        }

        //Type:  Gastrointestinal (GI) Infections					Condition:  Gastroenteritis	
        private void A11()
        {

            var rulesset = new InfectionCriteriaRuleSet();
            rulesset.CommentsText = "Care must be taken to exclude noninfectious causes of";
            rulesset.CommentsText += " symptoms. For instance, new medications may cause";
            rulesset.CommentsText += " diarrhea, nausea, or vomiting; initiation of new enteral";
            rulesset.CommentsText += " feeding may be associated with diarrhea; and nausea or";
            rulesset.CommentsText += " vomiting may be associated with gallbladder disease.";
            rulesset.CommentsText += " Presence of new GI symptoms in a single resident may";
            rulesset.CommentsText += " prompt enhanced surveillance for additional cases. In";
            rulesset.CommentsText += " the presence of an outbreak, stool specimens should be";
            rulesset.CommentsText += " sent to confirm the presence of norovirus or other";
            rulesset.CommentsText += " pathogens (eg, rotavirus or E. coli O157 : H7).";

            rulesset.MinimumRuleCount = 1;
            _DataContext.Insert(rulesset);

            addCriteria(rulesset, 1, "Must have 1 of the following criteria:",
                "Diarrhea: 3 or more liquid or watery stools above what is normal for the resident within a 24 hour period",
                "Vomiting: 2 or more episodes in a 24 hour period");

            addCriteria(rulesset, 2, "OR Both of the following sign or symptom subcriteria",
                "A stool specimen testing positive for a pathogen (eg, Salmonella, Shigella, Escherichia coli O157 : H7, Campylobacter species, rotavirus)",
                "At least 1 of the following GI subcriteria	(Nausea,Vomiting, Abdominal pain or tenderness,Diarrhea)");

            assign(rulesset, 915);
        }

        //Type:  Gastrointestinal (GI) Infections					Condition:  Norovirus Gastroenteritis			
        private void A12()
        {

            var rulesset = new InfectionCriteriaRuleSet();
            rulesset.CommentsText = "In the absence of laboratory confirmation, an outbreak (2";
            rulesset.CommentsText += " or more cases occurring in a long-term care facility";
            rulesset.CommentsText += " [LTCF]) of acute gastroenteritis due to norovirus";
            rulesset.CommentsText += " infection may be assumed to be present if all of the";
            rulesset.CommentsText += " following criteria are present (\"Kaplan Criteria\"): (a)";
            rulesset.CommentsText += " vomiting in more than half of affected persons; (b) a";
            rulesset.CommentsText += " mean (or median) incubation period of 24–48 hours; (c) a";
            rulesset.CommentsText += " mean (or median) duration of illness of 12–60 hours; and";
            rulesset.CommentsText += " (d) no bacterial pathogen is identified in stool culture.";

            rulesset.MinimumRuleCount = 2;
            _DataContext.Insert(rulesset);

            addCriteria(rulesset, 1, "Must have at least 1 of the following GI subcriteria",
                "Diarrhea: 3 or more liquid or watery stools above what is normal for the resident within a 24 hour period",
                "Vomiting: 2 or more episodes of in a 24 hour period");

            addCriteria(rulesset, 1, "AND",
                "A stool specimen for which norovirus is positively detected by electron microscopy, enzyme immunoassay, or	molecular diagnostic testing such as polymerase chain reaction (PCR)");

            var condition = _DataContext.CreateQuery<InfectionSite>()
                .FilterBy(x => x.Type.Id == 810)
                .FilterBy(x => x.Description == "Norovirus Gastroenteritis").FetchAll().FirstOrDefault();

            assign(rulesset, _GastroNorovirus.Id);

        }

        //Type:  Gastrointestinal (GI) Infections					Condition:  Clostridium difficile Infection			
        private void A13()
        {

            var rulesset = new InfectionCriteriaRuleSet();
            rulesset.CommentsText = "A “primary episode” of C. difficile infection is defined as";
            rulesset.CommentsText += " one that has occurred without any previous history of";
            rulesset.CommentsText += " C. difficile infection or that has occurred > 8 weeks after";
            rulesset.CommentsText += " the onset of a previous episode of C. difficile infection.";
            rulesset.CommentsText += " A “recurrent episode” of C. difficile infection is defined";
            rulesset.CommentsText += " as an episode of C. difficile infection that occurs 8 weeks";
            rulesset.CommentsText += " or sooner after the onset of a previous episode,";
            rulesset.CommentsText += " provided that the symptoms from the earlier (previous)";
            rulesset.CommentsText += " episode have resolved. Individuals previously infected";
            rulesset.CommentsText += " with C. difficile may continue to remain colonized even";
            rulesset.CommentsText += " after symptoms resolve. In the setting of an outbreak";
            rulesset.CommentsText += " of GI infection, individuals could have positive test";
            rulesset.CommentsText += " results for presence of C. difficile toxin because of";
            rulesset.CommentsText += " ongoing colonization and also be coinfected with";
            rulesset.CommentsText += " another pathogen. It is important that other";
            rulesset.CommentsText += " surveillance criteria be used to differentiate infections";
            rulesset.CommentsText += " in this situation.";

            rulesset.MinimumRuleCount = 2;
            _DataContext.Insert(rulesset);

            addCriteria(rulesset, 1, "Must have at least 1 of the following GI subcriteria",
                "Diarrhea: 3 or more liquid or watery stools above what	is normal for the resident within a 24 hour period",
                "Presence of toxic megacolon (abnormal dilatation of the large bowel, documented radiologically)");

            addCriteria(rulesset, 1, "AND One of the following diagnostic subcriteria",
                "A stool sample yields a positive laboratory test result for C. difficile toxin A or B, or a toxin-producing C.	difficile organism is identified from a stool sample culture or by a molecular diagnostic test such as PCR",
                "Pseudomembranous colitis is identified during endoscopic examination or surgery or in histopathologic examination of a biopsy specimen");


            assign(rulesset, _GastroClostridium.Id);
        }

        //Type:  Urinary Tract Infection			Condition:  UTI without Catheter		
        private void A14()
        {

            var rulesset = new InfectionCriteriaRuleSet();
            rulesset.CommentsText = "UTI should be diagnosed when there are localizing ";
            rulesset.CommentsText += "genitourinary signs and symptoms and a positive urine ";
            rulesset.CommentsText += "culture result. A diagnosis of UTI can be made without ";
            rulesset.CommentsText += "localizing symptoms if a blood culture isolate is the ";
            rulesset.CommentsText += "same as the organism isolated from the urine and there ";
            rulesset.CommentsText += "is no alternate site of infection. In the absence of a ";
            rulesset.CommentsText += "clear alternate source of infection, fever or rigors with ";
            rulesset.CommentsText += "a positive urine culture result in the noncatheterized ";
            rulesset.CommentsText += "resident or acute confusion in the catheterized resident ";
            rulesset.CommentsText += "will often be treated as UTI. However, evidence ";
            rulesset.CommentsText += "suggests that most of these episodes are likely not due ";
            rulesset.CommentsText += "to infection of a urinary source. ";
            rulesset.CommentsText += "<br>";
            rulesset.CommentsText += "<br>";
            rulesset.CommentsText += "Urine specimens for culture should be processed as soon ";			
            rulesset.CommentsText += "as possible, preferably within 1–2 hours. If urine specimens ";			
            rulesset.CommentsText += "cannot be processed within 30 min of collection, they ";				
            rulesset.CommentsText += "should be refrigerated. Refrigerated specimens should ";	
            rulesset.CommentsText += "be cultured within 24 hours. ";	


            rulesset.MinimumRuleCount = 2;
            _DataContext.Insert(rulesset);

            addCriteria(rulesset, 1, "Must have at least 1 of the following sign or symptom subcriteria:",
                "Acute dysuria or acute pain, swelling, or tenderness of the testes, epididymis, or prostate");

            addCriteria(rulesset, 1, "Fever or leukocytosis and at least 1 of the following localizing urinary tract subcriteria:",
                "Acute costovertebral angle pain or tenderness <span style=\"font-size:9px;font-weight:bold;\">(with fever or leukocytosis)</span>",
                "Suprapubic pain <span style=\"font-size:9px;font-weight:bold;\">(with fever or leukocytosis)</span>",
                "Gross hematuria <span style=\"font-size:9px;font-weight:bold;\">(with fever or leukocytosis)</span>",
                "New or marked increase in incontinence <span style=\"font-size:9px;font-weight:bold;\">(with fever or leukocytosis)</span>",
                "New or marked increase in urgency <span style=\"font-size:9px;font-weight:bold;\">(with fever or leukocytosis)</span>",
                "New or marked increase in frequency <span style=\"font-size:9px;font-weight:bold;\">(with fever or leukocytosis)</span>");

            addCriteria(rulesset, 2, "In the absence of fever or leukocytosis, then 2 or more of the following localizing urinary tract subcriteria:",
                "Suprapubic pain <span style=\"font-size:9px;font-weight:bold;\">(without fever or leukocytosis)</span>",
                "Gross hematuria <span style=\"font-size:9px;font-weight:bold;\">(without fever or leukocytosis)</span>",
                "New or marked increase in incontinence <span style=\"font-size:9px;font-weight:bold;\">(without fever or leukocytosis)</span>",
                "New or marked increase in urgency <span style=\"font-size:9px;font-weight:bold;\">(without fever or leukocytosis)</span>",
                "New or marked increase in frequency <span style=\"font-size:9px;font-weight:bold;\">(without fever or leukocytosis)</span>");

            addCriteria(rulesset, 1, "AND One of the following microbiologic subcriteria:",
            "At least 10<sup>5</sup> cfu/mL of no more than 2 species of microorganisms in a voided urine sample",
            "At least 10<sup>2</sup> cfu/mL of any number of organisms in a specimen collected by in-and-out catheter");

            assign(rulesset,913);
            
        }

        //Type: Type:  Skin, Soft Tissue and Mucosal Infection	Condition:  Oral candidiasis			
        private void A15()
        {

            var rulesset = new InfectionCriteriaRuleSet();
            rulesset.CommentsText = "Mucocutaneous Candida infections are usually due to";
            rulesset.CommentsText += " underlying clinical conditions such as poorly controlled";
            rulesset.CommentsText += " diabetes or severe immunosuppression. Although they";
            rulesset.CommentsText += " are not transmissible infections in the healthcare";
            rulesset.CommentsText += " setting, they can be a marker for increased antibiotic";
            rulesset.CommentsText += " exposure.";
            rulesset.MinimumRuleCount = 2;
            _DataContext.Insert(rulesset);

            addCriteria(rulesset, 1, "Must have:",
                "Presence of raised white patches on inflamed mucosa or	plaques on oral mucosa");

            addCriteria(rulesset, 1, "AND",
                "Diagnosis by a medical or dental provider");

            assign(rulesset, _SkinOral.Id);
        }

        //Type: Type:  Skin, Soft Tissue and Mucosal Infection	Condition:   Fungal skin infection		
        private void A16()
        {

            var rulesset = new InfectionCriteriaRuleSet();
            rulesset.CommentsText = "Dermatophytes have been known to cause occasional";
            rulesset.CommentsText += " infections and rare outbreaks in the LTCF setting";

            rulesset.MinimumRuleCount = 2;
            _DataContext.Insert(rulesset);

            addCriteria(rulesset, 1, "Must have:",
                "Characteristic rash or lesions");

            addCriteria(rulesset, 1, "AND",
            "Either a diagnosis by a medical provider or a laboratoryconfirmed fungal pathogen from a scraping or a	medical biopsy");

            assign(rulesset, _SkinFungal.Id);
        }


        //Type:  Skin, Soft Tissue and Mucosal Infection	Condition:   Herpes simplex infection
        private void A17()
        {

            var rulesset = new InfectionCriteriaRuleSet();
            rulesset.CommentsText = "Reactivation of herpes simplex (\"cold sores\") or herpes";
            rulesset.CommentsText += " zoster (\"shingles\") is not considered a healthcare associated (HAI)";
            rulesset.CommentsText += " infection. Primary herpesvirus skin infections";
            rulesset.CommentsText += " are very uncommon in a LTCF except in pediatric";
            rulesset.CommentsText += " populations, where it should be considered healthcare";
            rulesset.CommentsText += " associated.";

            rulesset.MinimumRuleCount = 2;
            _DataContext.Insert(rulesset);

            addCriteria(rulesset, 1, "Must have:",
                "A vesicular rash");

            addCriteria(rulesset, 1, "AND",
                "Either physician diagnosis or laboratory confirmation");

            assign(rulesset, _SkinHerpesSimplex.Id);
        }


        //Type:  Skin, Soft Tissue and Mucosal Infection	Condition:   Herpes zoster infection
        private void A18()
        {

            var rulesset = new InfectionCriteriaRuleSet();
            rulesset.CommentsText = "Reactivation of herpes simplex (\"cold sores\") or herpes";
            rulesset.CommentsText += " zoster (\"shingles\") is not considered a healthcare associated (HAI)";
            rulesset.CommentsText += " infection. Primary herpesvirus skin infections";
            rulesset.CommentsText += " are very uncommon in a LTCF except in pediatric";
            rulesset.CommentsText += " populations, where it should be considered healthcare";
            rulesset.CommentsText += " associated.";

            rulesset.MinimumRuleCount = 2;
            _DataContext.Insert(rulesset);

            addCriteria(rulesset, 1, "Must have:",
                "A vesicular rash");

            addCriteria(rulesset, 1, "AND",
                "Either physician diagnosis or laboratory confirmation");

            assign(rulesset, _SkinHerpesZoster.Id);
        }


        private void addCriteria(
            InfectionCriteriaRuleSet ruleSet,
            int minimumCount,
            string instructions,
            params string[] criterias)
        {
            var rule = new InfectionCriteriaRule(ruleSet);
            rule.MinimumCriteriaCount = minimumCount;
            rule.InstructionsText = instructions;
            _DataContext.Insert(rule);

            foreach (var criteria in criterias)
            {
                _DataContext.Insert(
                    new Domain.Models.InfectionCriteria(rule,
                        criteria));
            }

        }

        private void assign(InfectionCriteriaRuleSet rulset, int conditionId)
        {
            var condition = _DataContext.Fetch<InfectionSite>(conditionId);
            condition.RuleSet = rulset;
            _DataContext.Update(condition);
        }

    }
}
