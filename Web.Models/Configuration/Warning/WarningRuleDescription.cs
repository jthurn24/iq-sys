using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.Configuration.Warning
{
    public class WarningRuleDescription
    {
        public string RuleType { get; set; }
        public IDictionary<string, string> Arguments { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
    }
}
