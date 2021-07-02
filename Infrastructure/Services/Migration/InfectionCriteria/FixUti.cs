using System.Linq;
using System.Text;
using System.Collections.Generic;

using IQI.Intuition.Domain.Models;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Persistence.NHibernate;
using RedArrow.Framework.Logging;
using SnyderIS.sCore.Persistence;

namespace IQI.Intuition.Infrastructure.Services.Migration.InfectionCriteria
{
    public class FixUti
    {
        private IStatelessDataContext _DataContext;


        public FixUti(IStatelessDataContext dataContext)
        {
            _DataContext = dataContext;
        }

        public void Run(string[] args)
        {
            proc(int.Parse(args[1]));

        }

        private void proc(int id)
        {
            var type = _DataContext.CreateQuery<InfectionSite>()
                .FilterBy(x => x.Type.Definition.Id == id && x.Name == "UTI without catheter")
                .FetchFirst();

            var criteria = new List<IQI.Intuition.Domain.Models.InfectionCriteria>();
            GetAll(type.RuleSet.Id, criteria);

            /* Identify criteria */

            var cA1 = criteria.Where(x => x.Name.StartsWith("Acute dysuria or acute pain, swelling, or tenderness of the testes, epididymis, or prostate")).First();
            var cB1 = criteria.Where(x => x.Name.StartsWith("Acute costovertebral angle pain or tenderness <span style=\"font-size:9px;font-weight:bold;\">(with fever or leukocytosis)</span>")).First();
            var cB2 = criteria.Where(x => x.Name.StartsWith("Suprapubic pain <span style=\"font-size:9px;font-weight:bold;\">(with fever or leukocytosis)</span>")).First();
            var cB3 = criteria.Where(x => x.Name.StartsWith("Gross hematuria <span style=\"font-size:9px;font-weight:bold;\">(with fever or leukocytosis)</span>")).First();
            var cB4 = criteria.Where(x => x.Name.StartsWith("New or marked increase in incontinence <span style=\"font-size:9px;font-weight:bold;\">(with fever or leukocytosis)</span>")).First();
            var cB5 = criteria.Where(x => x.Name.StartsWith("New or marked increase in urgency <span style=\"font-size:9px;font-weight:bold;\">(with fever or leukocytosis)</span>")).First();
            var cB6 = criteria.Where(x => x.Name.StartsWith("New or marked increase in frequency <span style=\"font-size:9px;font-weight:bold;\">(with fever or leukocytosis)</span>")).First();

            var cC1 = criteria.Where(x => x.Name.StartsWith("Suprapubic pain <span style=\"font-size:9px;font-weight:bold;\">(without fever or leukocytosis)</span>")).First();
            var cC2 = criteria.Where(x => x.Name.StartsWith("Gross hematuria <span style=\"font-size:9px;font-weight:bold;\">(without fever or leukocytosis)</span>")).First();
            var cC3 = criteria.Where(x => x.Name.StartsWith("New or marked increase in incontinence <span style=\"font-size:9px;font-weight:bold;\">(without fever or leukocytosis)</span>")).First();
            var cC4 = criteria.Where(x => x.Name.StartsWith("New or marked increase in urgency <span style=\"font-size:9px;font-weight:bold;\">(without fever or leukocytosis)</span>")).First();
            var cC5 = criteria.Where(x => x.Name.StartsWith("New or marked increase in frequency <span style=\"font-size:9px;font-weight:bold;\">(without fever or leukocytosis)</span>")).First();

            var c2C1 = criteria.Where(x => x.Name.StartsWith("At least 10<sup>5</sup> cfu/mL of no more than 2 species of microorganisms in a voided urine sample")).First();
            var c2C2 = criteria.Where(x => x.Name.StartsWith("At least 10<sup>2</sup> cfu/mL of any number of organisms in a specimen collected by in-and-out catheter")).First();

            var parentRuleset = new InfectionCriteriaRuleSet();
            parentRuleset.MinimumRuleCount = 2;
            parentRuleset.InstructionsText = "For residents without an indwelling catheter (both criteria 1 and 2 must be present)";
            _DataContext.Insert(parentRuleset);

            type.RuleSet = parentRuleset;
            _DataContext.Update(type);

            var ruleSet1 = new InfectionCriteriaRuleSet();
            ruleSet1.ParentCriteriaRuleSet = parentRuleset;
            ruleSet1.InstructionsText = "1. At least 1 of the following sign or symptom subcriteria";
            ruleSet1.MinimumRuleCount = 1;
            _DataContext.Insert(ruleSet1);


            var rule1A = new InfectionCriteriaRule(ruleSet1);
            rule1A.InstructionsText = "";
            rule1A.MinimumCriteriaCount = 1;
            _DataContext.Insert(rule1A);
            cA1.Rule = rule1A;
            cA1.Name = "a. " + cA1.Name;
            _DataContext.Update(cA1);


            var rule1B = new InfectionCriteriaRule(ruleSet1);
            rule1B.InstructionsText = "b. Fever or leukocytosis and at least 1 of the following localizing urinary tract subcriteria";
            rule1B.MinimumCriteriaCount = 1;
            _DataContext.Insert(rule1B);
            cB1.Rule = rule1B;
            _DataContext.Update(cB1);
            cB2.Rule = rule1B;
            _DataContext.Update(cB2);
            cB3.Rule = rule1B;
            _DataContext.Update(cB3);
            cB4.Rule = rule1B;
            _DataContext.Update(cB4);
            cB5.Rule = rule1B;
            _DataContext.Update(cB5);
            cB6.Rule = rule1B;
            _DataContext.Update(cB6);

            var rule1C = new InfectionCriteriaRule(ruleSet1);
            rule1C.InstructionsText = "c. In the absence of fever or leukocytosis, then 2 or more of the following localizing urinary tract subcriteria";
            rule1C.MinimumCriteriaCount = 2;
            _DataContext.Insert(rule1C);
            cC1.Rule = rule1C;
            _DataContext.Update(cC1);
            cC2.Rule = rule1C;
            _DataContext.Update(cC2);
            cC3.Rule = rule1C;
            _DataContext.Update(cC3);
            cC4.Rule = rule1C;
            _DataContext.Update(cC4);
            cC5.Rule = rule1C;



            var ruleSet2 = new InfectionCriteriaRuleSet();
            ruleSet2.ParentCriteriaRuleSet = parentRuleset;
            ruleSet2.InstructionsText = "2. One of the following microbiologic subcriteria";
            ruleSet2.MinimumRuleCount = 1;
            _DataContext.Insert(ruleSet2);

            var rule2 = new InfectionCriteriaRule(ruleSet2);
            rule2.InstructionsText = "";
            rule2.MinimumCriteriaCount = 1;
            _DataContext.Insert(rule2);
            c2C1.Rule = rule2;
            c2C1.Name = "a. " + c2C1.Name;
            _DataContext.Update(c2C1);

            c2C2.Rule = rule2;
            c2C2.Name = "b. " + c2C2.Name;
            _DataContext.Update(c2C2);


        }


        private void GetAll(int rulSetId, List<IQI.Intuition.Domain.Models.InfectionCriteria> list)
        {
            var childRules = _DataContext.CreateQuery<InfectionCriteriaRule>()
                .FilterBy(x => x.RuleSet.Id == rulSetId)
                .FetchAll();

            foreach(var rule in childRules)
            {
                var childCriteria = _DataContext.CreateQuery<IQI.Intuition.Domain.Models.InfectionCriteria>()
                    .FilterBy(x => x.Rule.Id == rule.Id)
                    .FetchAll();
                
                foreach(var c in childCriteria)
                {
                    list.Add(c);
                }
            }

            var childRuleSets = _DataContext.CreateQuery<InfectionCriteriaRuleSet>()
                .FilterBy(x => x.ParentCriteriaRuleSet.Id == rulSetId)
                .FetchAll();

            foreach(var r in childRuleSets)
            {
                GetAll(r.Id, list);
            }
        }
    }
}
