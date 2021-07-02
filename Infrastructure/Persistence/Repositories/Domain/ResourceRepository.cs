using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Domain;
using SnyderIS.sCore.Encryption;

namespace IQI.Intuition.Infrastructure.Persistence.Repositories.Domain
{
    public class ResourceRepository : AbstractRepository<IDataContext>, IResourceRepository
    {
        public ResourceRepository(IDataContext dataContext)
            : base(dataContext) { }

        public void Add(Resource resource)
        {
            DataContext.TrackChanges(resource);
        }

        public void Add(ResourceFolder resource)
        {
            DataContext.TrackChanges(resource);
        }

        public void Delete(Resource resource)
        {
            DataContext.Delete(resource);
        }

        public Resource Get(int id)
        {
            return DataContext.Fetch<Resource>(id);
        }

        public ResourceFolder GetFolder(int id)
        {
            return DataContext.Fetch<ResourceFolder>(id);
        }

        public IEnumerable<ResourceFolder> GetRoot()
        {
            return DataContext.CreateQuery<ResourceFolder>()
                .FilterBy(x => x.Parent == null)
                .FetchAll();
        }

        public IEnumerable<Resource> GetRecent(int top)
        {
            return DataContext.CreateQuery<Resource>()
                .SortBy(x => x.CreatedOn).Descending()
                .FetchFirst(top);
        }

    }
}
