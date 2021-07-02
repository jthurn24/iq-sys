using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Graphics;
using System.Web.Mvc;
using IQI.Intuition.Reporting.Models.Cubes;
using IQI.Intuition.Reporting.Models.Dimensions;
using System.Drawing;

namespace IQI.Intuition.Web.Models.Reporting.Infection
{
    public class FloorMapView
    {

        public String StartDate { get; set; }
        public String EndDate { get; set; }

        public int DisplayMode { get; set; }
        public IEnumerable<SelectListItem> DisplayModeOptions { get; set; }

        public Guid? FloorMap { get; set; }
        public IEnumerable<SelectListItem> FloorMapOptions { get; set; }

       
        public FloorMapChart FloorMapChart { get; set; }

        public Dictionary<Guid, Color> InfectionColors { get; set; }
        public Dictionary<Guid, string> InfectionNames { get; set; }


        public void MapChart(
            IEnumerable<FloorMapRoomInfectionType.EntityEntry> results, 
            Guid floorMapGuid, 
            IEnumerable<FloorMapRoom> rooms,
            IEnumerable<InfectionType> infectionTypes)
        {



            InfectionColors = new System.Collections.Generic.Dictionary<Guid, Color>();
            InfectionNames = new System.Collections.Generic.Dictionary<Guid, string>();
            foreach (var type in infectionTypes.OrderBy(x => x.SortOrder))
            {
                InfectionColors[type.Id] = System.Drawing.ColorTranslator.FromHtml(type.Color); 
                InfectionNames[type.Id] = type.Name;
            }

            var chart = new FloorMapChart(floorMapGuid);

            int counter = 0;

            foreach (var room in rooms)
            {
                
                int width = 20;

                foreach (var type in infectionTypes.OrderBy(x => x.SortOrder))
                {
                    var data = results.Where(x => x.FloorMapRoom.Id == room.Id && x.InfectionType.Id == type.Id);

                    if (data.Count() > 0)
                    {
                        var circle = new FloorMapChart.Circle();
                        circle.Coordinates = room.Coordinates;
                        circle.Index = counter;
                        circle.ShadingColor = InfectionColors[type.Id];
                        circle.ShadingOpacity = 250;
                        circle.Width = width;
                        circle.Index = counter;
                        chart.Circles.Add(circle);
                        width = width + 10;
                        circle.DataUrl =
                            string.Concat("/Infection/ListByRoom/?roomGuid="
                            , room.Room.Id,
                            "&startDate=",
                            this.StartDate,
                            "&endDate=",
                            this.EndDate,
                            "&displayMode=",
                            this.DisplayMode);

                        counter++;
                    }

                    
                }
            }
            
            FloorMapChart = chart;
            
        }
    }
}
