using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class InfectionCriteriaRuleSet : AuditTrackingEntity<InfectionCriteriaRuleSet>
    {
        public InfectionCriteriaRuleSet()
        {
            // Initialize collections
            _CriteriaRules = new List<InfectionCriteriaRule>();
        }

        public virtual string CommentsText { get; set; }

        public virtual string InstructionsText { get; set; }

        public virtual int MinimumRuleCount { get; set; }

        public virtual InfectionCriteriaRuleSet ParentCriteriaRuleSet { get; set; }

        public virtual IEnumerable<InfectionCriteriaRuleSet> ChildCriteriaRuleSets { get; set; }

        private IList<InfectionCriteriaRule> _CriteriaRules;
        public virtual IEnumerable<InfectionCriteriaRule> CriteriaRules
        {
            get
            {
                return _CriteriaRules;
            }
        }

        //public virtual void AddRule(InfectionCriteriaRule rule)
        //{
        //    _CriteriaRules.Add(rule);
        //}

        //public virtual bool IsSatisfied(IEnumerable<InfectionCriteria> criteria)
        //{
        //    return MinimumRuleCount <= CriteriaRules.Count(rule => rule.IsSatisfied(criteria));
        //}
    }
}
