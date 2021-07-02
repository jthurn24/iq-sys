using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class InfectionCriteriaRule : AuditTrackingEntity<InfectionCriteriaRule>
    {
        protected InfectionCriteriaRule() { }

        public InfectionCriteriaRule(InfectionCriteriaRuleSet ruleSet)
        {
            RuleSet = ruleSet.ThrowIfNullArgument("ruleSet");

            // Initialize collections
            _CriteriaOptions = new List<InfectionCriteria>();
        }

        public virtual InfectionCriteriaRuleSet RuleSet { get;  set; }

        public virtual string InstructionsText { get; set; }

        public virtual int MinimumCriteriaCount { get; set; }

        private IList<InfectionCriteria> _CriteriaOptions;
        public virtual IEnumerable<InfectionCriteria> CriteriaOptions
        {
            get
            {
                return _CriteriaOptions;
            }
        }

        public virtual void AddOption(InfectionCriteria option)
        {
            _CriteriaOptions.Add(option);
        }

        public virtual bool IsSatisfied(IEnumerable<InfectionCriteria> criteria)
        {
            return MinimumCriteriaCount <= CriteriaOptions.Intersect(criteria).Count();
        }

        public virtual void ChangeRuleSet(InfectionCriteriaRuleSet set)
        {
            RuleSet = set;
        }
    }
}
