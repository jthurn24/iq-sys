using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using System.Linq.Expressions;
using RedArrow.Framework.Mvc.Extensions;
using RedArrow.Framework.Mvc.ModelMapper;
using System.Linq;
using RedArrow.Framework.Extensions.Common;

namespace IQI.Intuition.Web.Models.Extensions
{
    public static class TemplateExtensions
    {
        public static IProperty<TModel, TProperty> DropDownList<TModel, TProperty>(this IProperty<TModel, TProperty> property, Func<IEnumerable<SelectListItem>> itemsDelegate)
            where TModel : class
        {
            var propertyMetadata = property as IPropertyMetadata<TModel, TProperty>;
            propertyMetadata.SelectListItems(itemsDelegate);
            return propertyMetadata.Template(@"DropDownList");
        }

        public static IProperty<TModel, TProperty> DropDownList<TModel, TProperty>(this IProperty<TModel, TProperty> property)
            where TModel : class
        {
            var propertyMetadata = property as IPropertyMetadata<TModel, TProperty>;

            return propertyMetadata.Template(@"DropDownList");
        }

        public static IProperty<TModel, TProperty> HourDropDown<TModel, TProperty>(this IProperty<TModel, TProperty> property)
        where TModel : class
        {
            return property.HourDropDown(false);
        }

        public static IProperty<TModel, TProperty> HourDropDown<TModel, TProperty>(this IProperty<TModel, TProperty> property, bool allowNull)
            where TModel : class
        {
            var propertyMetadata = property as IPropertyMetadata<TModel, TProperty>;

            var list = new List<SelectListItem>() { 
                new SelectListItem() { Text = "0" , Value = "0" },
                new SelectListItem() { Text = "1" , Value = "1" },
                new SelectListItem() { Text = "2" , Value = "2" },
                new SelectListItem() { Text = "3" , Value = "3" },
                new SelectListItem() { Text = "4" , Value = "4" },
                new SelectListItem() { Text = "5" , Value = "5" },
                new SelectListItem() { Text = "6" , Value = "6" },
                new SelectListItem() { Text = "7" , Value = "7" },
                new SelectListItem() { Text = "8" , Value = "8" },
                new SelectListItem() { Text = "9" , Value = "9" },
                new SelectListItem() { Text = "10" , Value = "10" },
                new SelectListItem() { Text = "11" , Value = "11" },
                new SelectListItem() { Text = "12" , Value = "12" },
                new SelectListItem() { Text = "13" , Value = "13" },
                new SelectListItem() { Text = "14" , Value = "14" },
                new SelectListItem() { Text = "15" , Value = "15" },
                new SelectListItem() { Text = "16" , Value = "16" },
                new SelectListItem() { Text = "17" , Value = "17" },
                new SelectListItem() { Text = "18" , Value = "18" },
                new SelectListItem() { Text = "19" , Value = "19" },
                new SelectListItem() { Text = "20" , Value = "20" },
                new SelectListItem() { Text = "21" , Value = "21" },
                new SelectListItem() { Text = "22" , Value = "22" },
                new SelectListItem() { Text = "23" , Value = "23" }
            };

            if (allowNull == true)
            {
                list = list.Prepend(new SelectListItem() { Text = string.Empty, Value = string.Empty }).ToList();
            }

            propertyMetadata.SelectListItems(() => list);


            return propertyMetadata.Template(@"DropDownList");
        }

        public static IProperty<TModel, TProperty> MinuteDropDown<TModel, TProperty>(this IProperty<TModel, TProperty> property)
        where TModel : class
        {
            return property.MinuteDropDown(false);
        }

        public static IProperty<TModel, TProperty> MinuteDropDown<TModel, TProperty>(this IProperty<TModel, TProperty> property, bool allowNull)
            where TModel : class
        {
            var propertyMetadata = property as IPropertyMetadata<TModel, TProperty>;

            var list = new List<SelectListItem>() { 
                new SelectListItem() { Text = "00" , Value = "0" },
                new SelectListItem() { Text = "01" , Value = "1" },
                new SelectListItem() { Text = "02" , Value = "2" },
                new SelectListItem() { Text = "03" , Value = "3" },
                new SelectListItem() { Text = "04" , Value = "4" },
                new SelectListItem() { Text = "05" , Value = "5" },
                new SelectListItem() { Text = "06" , Value = "6" },
                new SelectListItem() { Text = "07" , Value = "7" },
                new SelectListItem() { Text = "08" , Value = "8" },
                new SelectListItem() { Text = "09" , Value = "9" },
                new SelectListItem() { Text = "10" , Value = "10" },
                new SelectListItem() { Text = "11" , Value = "11" },
                new SelectListItem() { Text = "12" , Value = "12" },
                new SelectListItem() { Text = "13" , Value = "13" },
                new SelectListItem() { Text = "14" , Value = "14" },
                new SelectListItem() { Text = "15" , Value = "15" },
                new SelectListItem() { Text = "16" , Value = "16" },
                new SelectListItem() { Text = "17" , Value = "17" },
                new SelectListItem() { Text = "18" , Value = "18" },
                new SelectListItem() { Text = "19" , Value = "19" },
                new SelectListItem() { Text = "20" , Value = "20" },
                new SelectListItem() { Text = "21" , Value = "21" },
                new SelectListItem() { Text = "22" , Value = "22" },
                new SelectListItem() { Text = "23" , Value = "23" },
                new SelectListItem() { Text = "24" , Value = "24" },
                new SelectListItem() { Text = "25" , Value = "25" },
                new SelectListItem() { Text = "26" , Value = "26" },
                new SelectListItem() { Text = "27" , Value = "27" },
                new SelectListItem() { Text = "28" , Value = "28" },
                new SelectListItem() { Text = "29" , Value = "29" },
                new SelectListItem() { Text = "30" , Value = "30" },
                new SelectListItem() { Text = "31" , Value = "31" },
                new SelectListItem() { Text = "32" , Value = "32" },
                new SelectListItem() { Text = "33" , Value = "33" },
                new SelectListItem() { Text = "34" , Value = "34" },
                new SelectListItem() { Text = "35" , Value = "35" },
                new SelectListItem() { Text = "36" , Value = "36" },
                new SelectListItem() { Text = "37" , Value = "37" },
                new SelectListItem() { Text = "38" , Value = "38" },
                new SelectListItem() { Text = "39" , Value = "39" },
                new SelectListItem() { Text = "40" , Value = "40" },
                new SelectListItem() { Text = "41" , Value = "41" },
                new SelectListItem() { Text = "42" , Value = "42" },
                new SelectListItem() { Text = "43" , Value = "43" },
                new SelectListItem() { Text = "44" , Value = "44" },
                new SelectListItem() { Text = "45" , Value = "45" },
                new SelectListItem() { Text = "46" , Value = "46" },
                new SelectListItem() { Text = "47" , Value = "47" },
                new SelectListItem() { Text = "48" , Value = "48" },
                new SelectListItem() { Text = "49" , Value = "49" },
                new SelectListItem() { Text = "50" , Value = "50" },
                new SelectListItem() { Text = "51" , Value = "51" },
                new SelectListItem() { Text = "52" , Value = "52" },
                new SelectListItem() { Text = "53" , Value = "53" },
                new SelectListItem() { Text = "54" , Value = "54" },
                new SelectListItem() { Text = "55" , Value = "55" },
                new SelectListItem() { Text = "56" , Value = "56" },
                new SelectListItem() { Text = "57" , Value = "57" },
                new SelectListItem() { Text = "58" , Value = "58" },
                new SelectListItem() { Text = "59" , Value = "59" }
            };


            if (allowNull == true)
            {
                list = list.Prepend(new SelectListItem() { Text = string.Empty, Value = string.Empty }).ToList();
            }

            propertyMetadata.SelectListItems(() => list
            );

            return propertyMetadata.Template(@"DropDownList");
        }

        public static IProperty<TModel, TProperty> CheckBoxQuestion<TModel, TProperty>(this IProperty<TModel, TProperty> property)
            where TModel : class
        {
            var propertyMetadata = property as IPropertyMetadata<TModel, TProperty>;

            return propertyMetadata.Template(@"CheckBoxQuestion");
        }

        public static IProperty<TModel, TProperty> CheckBoxList<TModel, TProperty>(this IProperty<TModel, TProperty> property, IEnumerable<SelectListItem> selectListItems)
            where TModel : class
        {
            var propertyMetadata = property as IPropertyMetadata<TModel, TProperty>;
            propertyMetadata.AdditionalMetadata("CheckBoxList.SelectListItems", selectListItems);

            return propertyMetadata.Template(@"CheckBoxList");
        }

        public static IProperty<TModel, TProperty> HorizontalCheckBoxList<TModel, TProperty>(this IProperty<TModel, TProperty> property, IEnumerable<SelectListItem> selectListItems)
        where TModel : class
        {
            var propertyMetadata = property as IPropertyMetadata<TModel, TProperty>;
            propertyMetadata.AdditionalMetadata("CheckBoxList.SelectListItems", selectListItems);

            return propertyMetadata.Template(@"HorizontalCheckBoxList");
        }


        public static IProperty<TModel, TProperty> EnumList<TModel, TProperty>(this IProperty<TModel, TProperty> property)
            where TModel : class
        {
            var propertyMetadata = property as IPropertyMetadata<TModel, TProperty>;

            return propertyMetadata.Template(@"Enum");
        }

        public static IProperty<TModel, TProperty> SubText<TModel, TProperty>(this IProperty<TModel, TProperty> property, string value)
        where TModel : class
        {
            var propertyMetadata = property as IPropertyMetadata<TModel, TProperty>;
            propertyMetadata.AdditionalMetadata("Display.SubText",value);
            return propertyMetadata;
        }

        public static IReadOnlyProperty<TModel, TProperty> SubText<TModel, TProperty>(this IReadOnlyProperty<TModel, TProperty> property, string value)
        where TModel : class
        {
            var propertyMetadata = property as IReadOnlyPropertyMetadata<TModel, TProperty>;
            propertyMetadata.AdditionalMetadata("Display.SubText", value);
            return propertyMetadata;
        }

    }
}