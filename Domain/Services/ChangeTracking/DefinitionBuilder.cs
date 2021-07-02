using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using SnyderIS.sCore.Extensions.Common;
using System.Reflection;
using RedArrow.Framework.Extensions.Formatting;

namespace IQI.Intuition.Domain.Services.ChangeTracking
{
    public class DefinitionBuilder<T> 
    {
        private Definition _Definition;

        public DefinitionBuilder()
        {
            _Definition = new Definition();
            _Definition.ComparisonOverrides = new Dictionary<string, Expression<Func<object, string>>>();
            _Definition.AuditCreateFlag = Enumerations.AuditEntryType.Unknown;
            _Definition.AuditEditFlag = Enumerations.AuditEntryType.Unknown;
            _Definition.AuditRemoveFlag = Enumerations.AuditEntryType.Unknown;
            _Definition.EntityDescription = x => typeof(T).Name.SplitPascalCase();
        }


        public DefinitionBuilder<T> Ignore(Expression<Func<T, object>> property)
        {
            var name = property.GetPropertyName<T>();
            _Definition.ComparisonOverrides[name] = x => String.Empty;
            return this;
        }

        public DefinitionBuilder<T> Override<TT>(Expression<Func<T, TT>> property, Expression<Func<TT, string>> o)
        {
            var name = property.GetPropertyName<T,TT>();

            Expression<Func<object, string>> exp = u =>
            ( o.Compile().Invoke((TT)u) );

            _Definition.ComparisonOverrides[name] = exp;
            return this;
        }

        public DefinitionBuilder<T> OverrideCollection(Expression<Func<T,object>> property, Expression<Func<IEnumerable<object>, string>> o)
        {
            var name = property.GetPropertyName<T>();

            Expression<Func<object, string>> exp = u =>
            (o.Compile().Invoke((IEnumerable<object>)u));

            _Definition.ComparisonOverrides[name] = exp;
            return this;
        }

        public IChangeTrackingDefinition GetDefinition()
        {
            return _Definition;
        }

        public DefinitionBuilder<T> RemoveExpression(Expression<Func<T, bool>> eval)
        {

            Expression<Func<object, bool>> exp = u =>
            (eval.Compile().Invoke((T)u));

            _Definition.RemoveExpression = exp;

            return this;
        }

        public DefinitionBuilder<T> CreateFlag(Domain.Enumerations.AuditEntryType flag)
        {
            _Definition.AuditCreateFlag = flag;
            return this;
        }

        public DefinitionBuilder<T> EditFlag(Domain.Enumerations.AuditEntryType flag)
        {
            _Definition.AuditEditFlag = flag;
            return this;
        }

        public DefinitionBuilder<T> RemoveFlag(Domain.Enumerations.AuditEntryType flag)
        {
            _Definition.AuditRemoveFlag = flag;
            return this;
        }

        public DefinitionBuilder<T> PatientGuid(Expression<Func<T, Guid>> e)
        {

            Expression<Func<object, Guid>> exp = u =>
            (e.Compile().Invoke((T)u));

            _Definition.AuditTargetPatient = exp;

            return this;

        }


        public DefinitionBuilder<T> ComponentGuid(Expression<Func<T, Guid>> e)
        {

            Expression<Func<object, Guid>> exp = u =>
            (e.Compile().Invoke((T)u));

            _Definition.AuditTargetComponent = exp;

            return this;

        }


        public DefinitionBuilder<T> Description(Expression<Func<T, string>> e)
        {

            Expression<Func<object, string>> exp = u =>
            (e.Compile().Invoke((T)u));

            _Definition.EntityDescription = exp;

            return this;

        }

        public class Definition : IChangeTrackingDefinition
        {

            public Expression<Func<object, Guid>> AuditTargetComponent { get; set;}

            public Expression<Func<object, Guid>> AuditTargetPatient { get; set;}

            public Enumerations.AuditEntryType AuditCreateFlag { get; set;}

            public Enumerations.AuditEntryType AuditEditFlag { get; set;}

            public Enumerations.AuditEntryType AuditRemoveFlag { get; set;}

            public IDictionary<string, Expression<Func<object, string>>> ComparisonOverrides { get; set;}

            public Expression<Func<object, string>> EntityDescription { get; set;}

            public Expression<Func<object,bool>> RemoveExpression { get; set; }

        }

 
 
    }
}
