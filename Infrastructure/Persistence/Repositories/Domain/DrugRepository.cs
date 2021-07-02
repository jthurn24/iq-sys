using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Domain;

namespace IQI.Intuition.Infrastructure.Persistence.Repositories.Domain
{
    public class DrugRepository : AbstractRepository<IDataContext>, IDrugRepository
    {
        public DrugRepository(IDataContext dataContext)
            : base(dataContext) { }

        public Drug Get(int id)
        {
            return DataContext.Fetch<Drug>(id);
        }

        public IEnumerable<Drug> Find(string searchFor, string startsWith)
        {
            var q =  DataContext.CreateQuery<Drug>();

            if (searchFor.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.Name.Contains(searchFor));
            }

            if (startsWith.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.Name.StartsWith(startsWith));
            }

            return q.SortBy(x => x.Name).FetchAll();
        }
    }   
}
