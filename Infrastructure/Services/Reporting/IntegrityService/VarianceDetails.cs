using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;
using SnyderIS.sCore.Extensions.Common;
using System.Reflection;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;

namespace IQI.Intuition.Infrastructure.Services.Reporting.IntegrityService
{
    public class VarianceDetails
    {
        public decimal DomainValue { get; set; }
        public decimal ReportingValue { get; set; }

        public IList<Entity> DomainEntities = new List<Entity>();
        public Entity ReportingEntity;


        public void SetReportingEntity(object src)
        {
            ReportingEntity = new Entity();
            ReportingEntity.ClassName = src.GetType().Name;
            ReportingEntity.ReadProperties(src, true);
        }

        public void AddDomainEntities(IEnumerable<object> src)
        {
            if (src != null)
            {
                foreach (var i in src)
                {
                    AddDomainEntity(i);
                }
            }
        }

        public void AddDomainEntity(object src)
        {
            var e = new Entity();
            e.ClassName = src.GetType().Name.SplitPascalCase();
            e.ReadProperties(src,false);
            DomainEntities.Add(e);
        }

        public class Entity
        {
            public string ClassName { get; set; }
            public IList<Attribute> Attributes = new List<Attribute>();

            public void ReadProperties(object src, bool children)
            {
                var cType = src.GetType();
                PropertyInfo[] properties = cType.GetProperties();
                foreach (PropertyInfo pi in properties)
                {
                    var a = new Attribute();
                    a.Name = pi.Name;

                    try
                    {
                        if (pi.PropertyType.Implements<RedArrow.Framework.ObjectModel.IUniqueIdentifier>())
                        {
                            var o = (RedArrow.Framework.ObjectModel.IUniqueIdentifier)cType.GetProperty(pi.Name).GetValue(src, null);
                            a.Value = o.Id.ToString();
                            Attributes.Add(a);
                        }
                        else if (
                            pi.PropertyType == typeof(string)
                            ||
                            pi.PropertyType == typeof(DateTime)
                            ||
                            pi.PropertyType == typeof(DateTime?)
                            ||
                            pi.PropertyType == typeof(int)
                            ||
                            pi.PropertyType == typeof(Int16)
                            ||
                            pi.PropertyType == typeof(Int32)
                            ||
                            pi.PropertyType == typeof(Int64)
                            ||
                            pi.PropertyType == typeof(int?)
                            ||
                            pi.PropertyType == typeof(Int16?)
                            ||
                            pi.PropertyType == typeof(Int32?)
                            ||
                            pi.PropertyType == typeof(Int64?)
                            ||
                            pi.PropertyType == typeof(Decimal)
                            ||
                            pi.PropertyType == typeof(Double)
                            ||
                            pi.PropertyType == typeof(Guid)
                            ||
                            pi.PropertyType == typeof(Guid?)
                            )
                        {
                            a.Value = cType.GetProperty(pi.Name).GetValue(src, null).ToStringSafely();
                            Attributes.Add(a);
                        }
                        else if (pi.PropertyType.Implements<System.Collections.IEnumerable>() == false)
                        {
                            if (children)
                            {
                                ReadProperties(cType.GetProperty(pi.Name).GetValue(src,null), true);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        a.Value = ex.Message;
                        Attributes.Add(a);
                    }

                }
            }
        }

        public class Attribute
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }


}
