using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class LogEntry 
    {
        public virtual Guid ErrorId { get; set; }
        public virtual string Application { get; set; }
        public virtual string Host { get; set; }
        public virtual string Type { get; set; }
        public virtual string Source { get; set; }
        public virtual string Message { get; set; }
        public virtual string User { get; set; }
        public virtual int StatusCode { get; set; }
        public virtual DateTime TimeUtc { get; set; }
        public virtual int Sequence { get; set; }
        public virtual string AllXml { get; set; }
    }
}
