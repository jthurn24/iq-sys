using System;
using System.Linq.Expressions;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using System.Collections.Generic;

namespace IQI.Intuition.Domain.Repositories
{
    public interface IResourceRepository
    {
        void Add(Resource resource);
        void Add(ResourceFolder resource);
        void Delete(Resource resource);
        Resource Get(int id);
        ResourceFolder GetFolder(int id);
        IEnumerable<ResourceFolder> GetRoot();

        IEnumerable<Resource> GetRecent(int top);
    }
}
