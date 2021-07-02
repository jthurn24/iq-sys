using System;
using System.Linq.Expressions;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using System.Collections.Generic;


namespace IQI.Intuition.Domain.Repositories
{
    public interface ITreatmentRepository
    {
        IEnumerable<Treatment> AllTreatments { get; }
        IEnumerable<TreatmentType> AllTreatmentTypes { get; }
    }
}
