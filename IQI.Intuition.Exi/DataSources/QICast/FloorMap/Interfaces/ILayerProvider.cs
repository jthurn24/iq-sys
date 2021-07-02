using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Exi.DataSources.QICast.FloorMap.Interfaces
{
    public interface ILayerProvider
    {
       Domain.Enumerations.KnownProductType ProductType { get; }
       void Initialize(Reporting.Models.Dimensions.FloorMap map);
       Reporting.Graphics.SmartFloorMap.Layer GetLayer();
       string GetTitle();
       string GetDescription();
       int GetListWeight();
    }
}
