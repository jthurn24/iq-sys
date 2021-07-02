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
    public class MockInfections : IConsoleService
    {
        private ILog _Log;
        private IStatelessDataContext _DataContext;

        public MockInfections(
            IStatelessDataContext dataContext,
            ILog log
            )
        {
            _DataContext = dataContext;
            _Log = log;
        }

        #region IConsoleService Members

        public void Run(string[] args)
        {
            if (_DataContext.CreateQuery<SystemSetting>().FilterBy(x => x.SettingKey == "IsProduction" && x.SettingValue == "0").FetchAll().Count() < 1)
            {
                System.Console.WriteLine("Unable to run this utility in production environments");
            }

            Random random = new Random();

            var patients = _DataContext.CreateQuery<Patient>()
                .FetchAll();

            var infectionTypes = _DataContext.CreateQuery<InfectionType>().FilterBy(x => x.IsHidden == false).FetchAll();
            var infectionSites = _DataContext.CreateQuery<InfectionSite>().FetchAll();


            foreach (var patient in patients)
            {
                int chanceOfInfection = random.Next(0, 100);

                if (chanceOfInfection > 50)
                {

                    int chanceOfInfectionType = random.Next(0, infectionTypes.Count() - 1);
                    var infectionType = infectionTypes.ToArray<InfectionType>()[chanceOfInfection];

                    var typeSites = infectionSites.Where( x => x.Type.Id == infectionType.Id);

                    int chanceOfSite = random.Next(0,typeSites.Count() -1);
                    var infectionsite = typeSites.ToArray<InfectionSite>()[chanceOfSite];

                    var criterias = _DataContext.CreateQuery<InfectionCriteriaRule>()
                        .FilterBy(x => x.RuleSet.Id == infectionsite.RuleSet.Id)
                        .FetchAll();

                    var selectedCriteria = new List<int>();


                    foreach (var criteria in criterias)
                    {
                        int chanceOfCriteria = random.Next(0, 50);

                        if (chanceOfCriteria > 0)
                        {
                            selectedCriteria.Add(criteria.Id);
                        }
                    }

                    

                }

            }

        }

        #endregion
    }
}
