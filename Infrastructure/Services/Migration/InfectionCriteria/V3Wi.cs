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
    public class V3WI : IConsoleService
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

        public V3WI(IStatelessDataContext dataContext)
        {
            _DataContext = dataContext;
        }

        public void Run(string[] args)
        {
            var sourceTypes = _DataContext.CreateQuery<InfectionType>()
                .FilterBy(x => x.Definition.Id == 1)
                .FilterBy(x => x.IsHidden == false)
                .FetchAll();

            var newDef = _DataContext.CreateQuery<InfectionDefinition>()
                .FilterBy(x => x.Id == 3).FetchFirst();

            foreach (var st in sourceTypes)
            {
                CreateType(st, newDef);
            }

        }

        private void CreateType(InfectionType src, InfectionDefinition def)
        {
            var newType = new InfectionType(src.Name);
            newType.Definition = def;
            newType.ShortName = src.ShortName;
            newType.SortOrder = src.SortOrder;
            newType.UsedForEmployees = src.UsedForEmployees;

            _DataContext.Insert(newType);

            var srcSites = _DataContext.CreateQuery<InfectionSite>()
                .FilterBy(x => x.Type.Id == src.Id)
                .FetchAll();

            foreach (var srcSite in srcSites)
            {
                CreateSite(srcSite, newType);
            }

        }

        private void CreateSite(InfectionSite src, InfectionType newType)
        {

            var newSite = new InfectionSite(src.Name, newType);
            newSite.Description = src.Description;
            newSite.SupportingDetailsDescription = src.SupportingDetailsDescription;
            newSite.CriteriaAvailabilityRule = src.CriteriaAvailabilityRule;
            _DataContext.Insert(newSite);

            var srcRuleSet = _DataContext.CreateQuery<InfectionCriteriaRuleSet>()
                .FilterBy(x => x.Id == src.RuleSet.Id)
                .FetchFirst();

            if (src.Name == "Influenza-like Illness")
            {
                newSite.RuleSet = CloneILIRuleSet(srcRuleSet, null);
                newSite.Name = "Acute Respiratory Infection ";
            }
            else
            {
                newSite.RuleSet = CloneRuleSet(srcRuleSet, null);
            }


            _DataContext.Update(newSite);

            var srcSupportingDetails = _DataContext.CreateQuery<InfectionSiteSupportingDetail>()
                .FilterBy(x => x.InfectionSite.Id == src.Id)
                .FilterBy( x=> x.Name != "")
                .FetchAll();

            foreach (var srcSupportingDetail in srcSupportingDetails)
            {
                var newDetail = new InfectionSiteSupportingDetail(newSite, srcSupportingDetail.Name);
                _DataContext.Insert(newDetail);
            }
                
    
        }


        /**************** Special change for ILI *********************/
        private InfectionCriteriaRuleSet CloneILIRuleSet(InfectionCriteriaRuleSet srcSet, InfectionCriteriaRuleSet parentSet)
        {

            var newRuleSet = new InfectionCriteriaRuleSet();
            newRuleSet.CommentsText = "";
            newRuleSet.InstructionsText = srcSet.InstructionsText;
            newRuleSet.MinimumRuleCount = 1;
            newRuleSet.ParentCriteriaRuleSet = parentSet;
            _DataContext.Insert(newRuleSet);


            var newRule = new InfectionCriteriaRule(newRuleSet);
            newRule.InstructionsText = "At least 2 of the following ARI subcriteria";
            newRule.MinimumCriteriaCount = 2;
            _DataContext.Insert(newRule);

            var srcRules = _DataContext.CreateQuery<InfectionCriteriaRule>()
                .FilterBy(x => x.RuleSet.Id == srcSet.Id)
                .FetchAll();



            var c1 = new Domain.Models.InfectionCriteria(newRule, "* Fever (Fever may be difficult to determine among elderly residents. Therefore, the definition of fever used for ARI may be defined as temperature two degrees (2°) Fahrenheit above the established baseline for that resident)");
            _DataContext.Insert(c1);

            var c2 = new Domain.Models.InfectionCriteria(newRule, "Cough (New or Worsening, productive or non-productive)");
            _DataContext.Insert(c2);

            var c3 = new Domain.Models.InfectionCriteria(newRule, "Rhinorrhea (runny nose) or nasal congestion");
            _DataContext.Insert(c3);

            var c4 = new Domain.Models.InfectionCriteria(newRule, "Sore throat");
            _DataContext.Insert(c4);

            var c5 = new Domain.Models.InfectionCriteria(newRule, "Myalgia (muscle aches) great than the resident’s norm.");
            _DataContext.Insert(c5);

            return newRuleSet;
        }

        private InfectionCriteriaRuleSet CloneRuleSet(InfectionCriteriaRuleSet srcSet, InfectionCriteriaRuleSet parentSet)
        {
            var newRuleSet = new InfectionCriteriaRuleSet();
            newRuleSet.CommentsText = srcSet.CommentsText;
            newRuleSet.InstructionsText = srcSet.InstructionsText;
            newRuleSet.MinimumRuleCount = srcSet.MinimumRuleCount;
            newRuleSet.ParentCriteriaRuleSet = parentSet;
            _DataContext.Insert(newRuleSet);


            var srcRules = _DataContext.CreateQuery<InfectionCriteriaRule>()
                .FilterBy(x => x.RuleSet.Id == srcSet.Id)
                .FetchAll();

            foreach (var srcRule in srcRules)
            {
                CloneRule(srcRule, newRuleSet);
            }


            /* Child rulesets */

            var srcChildRuleSets = _DataContext.CreateQuery<InfectionCriteriaRuleSet>()
                .FilterBy(x => x.ParentCriteriaRuleSet != null && x.ParentCriteriaRuleSet.Id == srcSet.Id)
                .FetchAll();

            foreach (var srcChildRuleSet in srcChildRuleSets)
            {
                CloneRuleSet(srcChildRuleSet, newRuleSet);
            }
     

            return newRuleSet;
        }

        private void CloneRule(InfectionCriteriaRule srcRule, InfectionCriteriaRuleSet newRuleSet)
        {
            var newRule = new InfectionCriteriaRule(newRuleSet);
            newRule.MinimumCriteriaCount = srcRule.MinimumCriteriaCount;
            newRule.InstructionsText = srcRule.InstructionsText;
            newRule.MinimumCriteriaCount = srcRule.MinimumCriteriaCount;
            




            _DataContext.Insert(newRule);

            var srcCriterias = _DataContext.CreateQuery<Domain.Models.InfectionCriteria>()
            .FilterBy(x => x.Rule.Id == srcRule.Id)
            .FetchAll();

            foreach (var srcCriteria in srcCriterias)
            {
                CloneCriteria(srcCriteria, newRule);
            }
        }

        private void CloneCriteria(Domain.Models.InfectionCriteria srcCrit, InfectionCriteriaRule newRule)
        {
            var newCrit = new Domain.Models.InfectionCriteria(newRule, srcCrit.Name);
            _DataContext.Insert(newCrit);
        }

     

    }
}
