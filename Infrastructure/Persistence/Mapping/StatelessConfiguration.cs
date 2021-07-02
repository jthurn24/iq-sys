using System;
using FluentNHibernate.Automapping;
using FluentNHibernate.Conventions.Helpers;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Persistence.NHibernate;
using RedArrow.Framework.Persistence.NHibernate.Helpers;
using RedArrow.Framework.Persistence.NHibernate.Helpers.Mapping;
using RedArrow.Framework.Persistence.NHibernate.Helpers.Mapping.Conventions;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Infrastructure.Persistence.Mapping.Conventions;
using IQI.Intuition.Reporting.Models.Cubes;

namespace IQI.Intuition.Infrastructure.Persistence.Mapping
{
    public class StatelessConfiguration : AutoMappedSqlServerConfiguration
    {
        public StatelessConfiguration()
            : base(typeof(Account))
        {
            EnableAuditTracking();
            BatchSize(0);
            IsolationLevel(System.Data.IsolationLevel.ReadCommitted);

            this.AddAutoMappingConvention(new FileFieldConvention());




            
        }
    }
}