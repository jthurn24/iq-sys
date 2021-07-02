using System;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using System.Reflection;
using System.Linq.Expressions;

namespace IQI.Intuition.Web.Models.Extensions
{
    public static class MappingExtensions
    {
        public static IProperty<TModel, DateTime?> AtMostToday<TModel>(
            this IProperty<TModel, DateTime?> property, 
            string propertyDescription) where TModel : class
        {
            return property.AtMost(DateTime.Today)
                .ErrorMessage("{0} date cannot be in the future".FormatWith(propertyDescription));
        }

        public static IProperty<TModel, DateTime?> AtMostToday<TModel>(
            this IProperty<TModel, DateTime?> property) where TModel : class
        {
            return property.AtMost(DateTime.Today)
                .ErrorMessage(name => "{0} cannot be in the future".FormatWith(name));
        }

        public static IProperty<TModel, DateTime?> RequiredWhen<TModel>(
            this IProperty<TModel, DateTime?> property, 
            Func<TModel, bool> predicate,
            string propertyDescription) where TModel : class
        {
            return property.Required().When((model, value) => predicate(model))
                .ErrorMessage("Please specify {0} date".FormatWith(propertyDescription));
        }

        public static IProperty<TModel, string> RequiredWhen<TModel>(
            this IProperty<TModel, string> property,
            Func<TModel, bool> predicate) where TModel : class
        {
            return property.Required().When((model, value) => predicate(model))
                .ErrorMessage(name => "Please specify {0}".FormatWith(name));
        }

        public static void WriteDate<TModel, TDomainModel>(
            this ModelMap<TModel, TDomainModel> map,
            TDomainModel domain,
            Expression<Func<TDomainModel, DateTime?>> targetFunc, DateTime? val)
            where TModel : class
            where TDomainModel : class
        {
            var prop = (PropertyInfo)((MemberExpression)targetFunc.Body).Member;

            if (val.HasValue == false)
            {
                prop.SetValue(domain, null, null);
                return;
            }

            var target = targetFunc.Compile().Invoke(domain);

            if (target.HasValue == false)
            {
                target = DateTime.Today;
            }

            target = new DateTime(
                val.Value.Year,
                val.Value.Month,
                val.Value.Day,
                target.Value.Hour,
                target.Value.Minute,
                0);

            prop.SetValue(domain, target, null);
            return;
        }

        public static void WriteMinute<TModel, TDomainModel>(
            this ModelMap<TModel, TDomainModel> map ,
            TDomainModel domain, 
            Expression<Func<TDomainModel, DateTime?>> targetFunc, int? minutes)
            where TModel : class
            where TDomainModel : class
        {
            var target = targetFunc.Compile().Invoke(domain);

            if (minutes.HasValue == false || target.HasValue == false)
            {
                return;
            }

            target = new DateTime(
                target.Value.Year,
                target.Value.Month,
                target.Value.Day,
                target.Value.Hour,
                minutes.Value,
                0);

            var prop = (PropertyInfo)((MemberExpression)targetFunc.Body).Member;
            prop.SetValue(domain, target, null);
        }


        public static void WriteHour<TModel, TDomainModel>(
            this ModelMap<TModel, TDomainModel> map,
            TDomainModel domain,
            Expression<Func<TDomainModel, DateTime?>> targetFunc, int? hour)
                    where TModel : class
                    where TDomainModel : class
                {

                    var target = targetFunc.Compile().Invoke(domain);

                    if (hour.HasValue == false || target.HasValue == false)
                    {
                        return;
                    }

                    target = new DateTime(
                        target.Value.Year,
                        target.Value.Month,
                        target.Value.Day,
                        hour.Value,
                        target.Value.Minute,
                        0);

                    var prop = (PropertyInfo)((MemberExpression)targetFunc.Body).Member;
                    prop.SetValue(domain, target, null);
                }

    }
}
