using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Domain.Repositories;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Logging;
using IQI.Intuition.Domain.Models;
using SnyderIS.sCore.Persistence;

namespace IQI.Intuition.Infrastructure.Services.Utilities
{
    public class InfectionManagement : IConsoleService
    {
        private ILog _Log;
        private IStatelessDataContext _DataContext;

        public InfectionManagement(
            IStatelessDataContext dataContext,
            ILog log
            )
        {
            _DataContext = dataContext;
            _Log = log;
        }

        public void Run(string[] args)
        {
            if (args[1] == "CloneSite")
            {
                CloneSite(args);
            }
        }

        private void CloneSite(string[] args)
        {
            int siteID = Convert.ToInt32(args[2]);
            string name = args[3];

            var oldSite = _DataContext.Fetch<InfectionSite>(siteID);
            var newSite = new InfectionSite(name,oldSite.Type);
            newSite.Description = oldSite.Description;
            newSite.SupportingDetailsDescription = oldSite.SupportingDetailsDescription;
            _DataContext.Insert(newSite);

            var oldRuleSet = _DataContext.Fetch<InfectionCriteriaRuleSet>(oldSite.RuleSet.Id);
            var newRuleSet = new InfectionCriteriaRuleSet();
            newRuleSet.CommentsText = oldRuleSet.CommentsText;
            newRuleSet.InstructionsText = oldRuleSet.InstructionsText;
            newRuleSet.MinimumRuleCount = oldRuleSet.MinimumRuleCount;
            _DataContext.Insert(newRuleSet);

            newSite.RuleSet = newRuleSet;
            _DataContext.Update(newSite);

            foreach (var oldRule in _DataContext.CreateQuery<InfectionCriteriaRule>().FilterBy(x => x.RuleSet.Id == oldRuleSet.Id).FetchAll())
            {
                var newRule = new InfectionCriteriaRule(newRuleSet);
                newRule.InstructionsText = oldRule.InstructionsText;
                newRule.MinimumCriteriaCount = oldRule.MinimumCriteriaCount;
                _DataContext.Insert(newRule);

                foreach(var oldCriteria in _DataContext.CreateQuery<InfectionCriteria>().FilterBy(x =>x.Rule.Id == oldRule.Id).FetchAll())
                {
                    var newCriteria = new InfectionCriteria(newRule, oldCriteria.Name);
                    _DataContext.Insert(newCriteria);
                }
            }


            _DataContext.Close();
        }
    }
}
