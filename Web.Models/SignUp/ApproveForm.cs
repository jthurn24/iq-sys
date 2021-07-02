using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQI.Intuition.Web.Models.SignUp
{
    public class ApproveForm
    {
        public virtual string Password { get; set; }
        public virtual string ConfirmPassword { get; set; }
        public virtual string MaxBeds { get; set; }
        public virtual string State { get; set; }
        public virtual string Code { get; set; }
        public virtual string Link { get; set; }
        public virtual bool Complete { get; set; }
        public virtual string FacilityName { get; set; }
    }
}
