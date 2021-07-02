using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RedArrow.Framework.Extensions.Common;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace IQI.Intuition.Infrastructure.Persistence.Mapping.Conventions
{
    public class FileFieldConvention : IPropertyConvention, IPropertyConventionAcceptance
    {
        public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
        {
            criteria.Expect(x => x.Name.EndsWith("FileData", StringComparison.InvariantCultureIgnoreCase));
        }

        public void Apply(IPropertyInstance instance)
        {
            instance.Length(1048576);
            instance.CustomType("BinaryBlob");
            instance.Nullable();

        }
    }
}
