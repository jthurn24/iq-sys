using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;
using IQI.Intuition.Domain.Services.Psychotropic;

namespace IQI.Intuition.Domain.Models
{
    public class PsychotropicFrequency : Entity<PsychotropicFrequency>
    {
        public virtual string Name { get; set; }
        public virtual string DosageFrequencyDefinitionTypeName { get; set; }


        public virtual IDosageFrequencyDefinition GetFrequencyDefinition()
        {
            var type = Type.GetType(this.DosageFrequencyDefinitionTypeName, true);
            return (IDosageFrequencyDefinition)Activator.CreateInstance(type);
        }
    }
}
