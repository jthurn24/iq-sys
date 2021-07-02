using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Graphics;
using RedArrow.Framework.Extensions.Formatting;

namespace IQI.Intuition.Exi.Models.QICast
{
    public class FloorMapView
    {
        public SmartFloorMap FloorMap { get; set; }
        public IList<Group> Groups { get; set; }



        public class Group
        {
            public Domain.Enumerations.KnownProductType ProductType { get; set; }
            public IList<Layer> Layers { get; set; }

            public string Name
            {
                get
                {
                    return System.Enum.GetName(typeof(Domain.Enumerations.KnownProductType), this.ProductType).SplitPascalCase();
                }
            }
        }

        public class Layer
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public Guid LayerId { get; set; }
            public int Count { get; set; }
        }
    }
}
