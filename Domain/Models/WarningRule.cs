using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;


namespace IQI.Intuition.Domain.Models
{
    public class WarningRule : Entity<WarningRule>
    {
        public virtual Facility Facility { get; set; }
        public virtual string Description { get; set; }
        public virtual string Title { get; set; }
        public virtual string Arguments { get; set; }
        public virtual string TypeName { get; set; }
        public virtual string ItemTemplate { get; set; }

        public virtual Dictionary<string, string> ParsedArguments
        {
            get
            {
                var arguments = new Dictionary<string, string>();

                if (this.Arguments.IsNotNullOrEmpty())
                {
                    var args = this.Arguments.Split(",".ToCharArray());

                    foreach (string arg in args)
                    {
                        var pieces = arg.Split("=".ToCharArray());
                        arguments[pieces[0]] = pieces[1];
                    }
                }

                return arguments;
            }
        }

        public virtual string ParsedTitle
        {
            get
            {
                string title = this.Title;

                foreach (var key in this.ParsedArguments.Keys)
                {
                    title = title.Replace(string.Concat("{", key, "}"), this.ParsedArguments[key]);
                }

                return title;
            }
        }
    }
}
