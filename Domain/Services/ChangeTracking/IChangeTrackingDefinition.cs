using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;


namespace IQI.Intuition.Domain.Services.ChangeTracking
{

    public interface IChangeTrackingDefinition
    {
        Expression<Func<object, Guid>> AuditTargetComponent { get; }
        Expression<Func<object, Guid>> AuditTargetPatient { get; }
        Domain.Enumerations.AuditEntryType AuditCreateFlag { get; }
        Domain.Enumerations.AuditEntryType AuditEditFlag { get; }
        Domain.Enumerations.AuditEntryType AuditRemoveFlag { get; }
        IDictionary<string, Expression<Func<object, string>>> ComparisonOverrides { get; }
        Expression<Func<object, string>> EntityDescription { get; }
        Expression<Func<object,bool>> RemoveExpression { get; }
    }
}
