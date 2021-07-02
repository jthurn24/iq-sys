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
    public class StatefulConfiguration : AutoMappedSqlServerConfiguration
    {
        public StatefulConfiguration(object[] listeners)
            : base(typeof(Account)) 
        {
            EnableAuditTracking();
            IsolationLevel(System.Data.IsolationLevel.ReadCommitted);

            this.AddAutoMappingConvention(new FileFieldConvention());

            if (listeners != null && listeners.Length > 0)
            {
                foreach (var listener in listeners)
                {
                    this.AddListener(listener);
                }
            }


        }
    }
}