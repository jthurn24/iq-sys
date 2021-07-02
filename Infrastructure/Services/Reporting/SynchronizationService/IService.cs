using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Domain.Models;
using Dimensions = IQI.Intuition.Reporting.Models.Dimensions;
using Cubes = IQI.Intuition.Reporting.Models.Cubes;
using Facts = IQI.Intuition.Reporting.Models.Facts;
using IQI.Intuition.Reporting.Repositories;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Logging;
using SnyderIS.sCore.Persistence;
using IQI.Intuition.Reporting.Containers;

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService
{
    public interface IService
    {
        void Run(DateTime scanStartDate,
            Domain.Models.Facility facility,
            DataDimensions dimensions,
            IDimensionBuilderRepository dimensionBuilderRepository,
            IDimensionRepository dimensionRepository,
            ICubeBuilderRepository cubeBuilderRepository,
            IStatelessDataContext dataContext,
            IFactBuilderRepository factBuilderRespository,
            ILog log,
            IDocumentStore store);
    }
}
