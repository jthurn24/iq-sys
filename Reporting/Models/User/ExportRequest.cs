using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Reporting.Models.User
{
    public class ExportRequest : BaseReportingEntity
    {
        public enum ExportRequestStatus : int
        {
            New = 0,
            Waiting = 1,
            Pending = 2,
            Completed =3,
            Error =4,
            Running = 5
        }

        public enum ExportRequestFormat : int
        {
            Pdf = 0
        }

        public ExportRequest(int accountId, ExportRequestFormat format)
        {
            this.AccountId = accountId;
            this.CreatedOn = DateTime.Now;
            this.Status = ExportRequestStatus.New;
            this.ExportPaths = new List<ExportRequestPath>();
            this.Format = format;

        }

        public virtual ExportRequestStatus Status { get; set; }
        public virtual ExportRequestFormat Format { get; set; }
        public virtual string EmailTo { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual IList<ExportRequestPath> ExportPaths { get; set; }
        public virtual byte[] OutputFile { get; set; }
        public virtual int AccountId { get; set; }
        public virtual string ReturnPath { get; set; }
        

        public class ExportRequestPath 
        {
            public virtual string Path { get; set; }
            public virtual bool? Landscape { get; set; }
            public virtual Guid Id { get; set; }
        }
    }
}
