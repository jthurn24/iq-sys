using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace IQI.Intuition.Domain.Services.ChangeTracking
{

    public interface ITrackChanges
    {
        IChangeTrackingDefinition GetChangeTrackingDefinition();
    }
}
