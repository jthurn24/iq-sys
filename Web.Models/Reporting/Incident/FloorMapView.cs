using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Graphics;
using System.Web.Mvc;
using IQI.Intuition.Reporting.Models.Cubes;
using IQI.Intuition.Reporting.Models.Dimensions;
using System.Drawing;
using System.Collections.Generic;


namespace IQI.Intuition.Web.Models.Reporting.Incident
{
    public class FloorMapView
    {
        public String StartDate { get; set; }
        public String EndDate { get; set; }


        public FloorMapChart FloorMapChart { get; set; }

        public Guid? FloorMap { get; set; }
        public IEnumerable<SelectListItem> FloorMapOptions { get; set; }

        public Dictionary<Guid, Color> IncidentGroupColors { get; set; }
        public Dictionary<Guid, string> IncidentGroupNames { get; set; }


        public void MapChart(
            IEnumerable<FloorMapRoomIncidentType.EntityEntry> results,
            Guid floorMapGuid,
            IEnumerable<FloorMapRoom> rooms,
            IEnumerable<IncidentTypeGroup> incidentGroups)
        {

            IncidentGroupColors = new System.Collections.Generic.Dictionary<Guid, Color>();
            IncidentGroupNames = new System.Collections.Generic.Dictionary<Guid, string>();
            foreach (var type in incidentGroups.OrderBy(x => x.Name))
            {
                IncidentGroupColors[type.Id] = System.Drawing.ColorTranslator.FromHtml(type.Color);
                IncidentGroupNames[type.Id] = type.Name;
            }

            var chart = new FloorMapChart(floorMapGuid);

            int counter = 0;

            foreach (var room in rooms)
            {

                int width = 20;

                foreach (var type in incidentGroups.OrderBy(x => x.Name))
                {
                    var data = results
                        .Where(x => x.FloorMapRoom.Id == room.Id 
                            && (x.IncidentTypeGroups.Contains(type)));

                    if (data.Count() > 0)
                    {
                        var circle = new FloorMapChart.Circle();
                        circle.Coordinates = room.Coordinates;
                        circle.Index = counter;
                        circle.ShadingColor = IncidentGroupColors[type.Id];
                        circle.ShadingOpacity = 250;
                        circle.Width = width;
                        circle.Index = counter;
                        chart.Circles.Add(circle);
                        width = width + 10;
                        circle.DataUrl =
                            string.Concat("/Incident/ListByRoom/?roomGuid="
                            , room.Room.Id,
                            "&startDate=",
                            this.StartDate,
                            "&endDate=",
                            this.EndDate);

                        counter++;
                    }


                }
            }

            FloorMapChart = chart;

        }
    }
}
