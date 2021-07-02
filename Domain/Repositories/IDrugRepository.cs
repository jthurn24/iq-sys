using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Domain.Repositories
{
    public interface IDrugRepository
    {
        Drug Get(int id);
        IEnumerable<Drug> Find(string searchFor, string startsWith);
    }
}
