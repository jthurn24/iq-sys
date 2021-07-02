using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Attributes;

namespace IQI.Intuition.Reporting.Models
{
    public abstract class BaseReportingEntity : IEquatable<BaseReportingEntity>
    {
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Id { get; set; }

        

        public override bool Equals(object obj)
        {
            return (BaseReportingEntity)obj == this;
        }

  

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(BaseReportingEntity a, BaseReportingEntity b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Id == b.Id;
        }

        public static bool operator !=(BaseReportingEntity a, BaseReportingEntity b)
        {
            return !(a == b);
        }


        public bool Equals(BaseReportingEntity other)
        {
            return other == this;
        }

    }
}
