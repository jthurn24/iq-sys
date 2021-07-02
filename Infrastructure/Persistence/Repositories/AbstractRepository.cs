using System;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Persistence;

namespace IQI.Intuition.Infrastructure.Persistence.Repositories
{
    public abstract class AbstractRepository<T>
    {
        public AbstractRepository(T dataContext)
        {
            this.DataContext = dataContext;
        }

        protected T DataContext { get; set; }
    }
}
