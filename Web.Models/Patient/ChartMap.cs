using System;
using System.Linq;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;

namespace IQI.Intuition.Web.Models.Patient
{
    public class ChartMap : ReadOnlyModelMap<Chart, Domain.Models.Patient>
    {
        public ChartMap()
        {
            ForProperty(model => model.MDName)
                .Read(domain => domain.MDName);

            ForProperty(model => model.Warnings)
                .Read(domain => domain.Warnings
                    .Select(warning => warning.Title)
                    .Take(2)
                    .ToArray());


            ForProperty(model => model.Flags)
                .Read(domain => domain.PatientFlags
                    .Select(flag => flag.Name)
                    .ToArray());

            ForProperty(model => model.StatusChanges)
                .Read(domain => domain.StatusChanges
                    .OrderByDescending(d => d.StatusChangedAt)
                    .Select( 
                    x => new ChartStatusChange() 
                    { 
                        Status = Enum.GetName(typeof(Domain.Enumerations.PatientStatus),x.Status),
                        StatusOn = x.StatusChangedAt.ToString("MM/dd/yy"),
                        Id = x.Id.ToString()
                    }));


            ForProperty(model => model.RoomChanges)
                .Read(domain => domain.RoomChanges
                    .OrderByDescending(d => d.RoomChangedAt)
                    .Select(
                    x => new ChartRoomChange()
                    {
                        Room = string.Concat(x.Room.Wing.Floor.Name," - ", x.Room.Wing.Name, " - ",x.Room.Name),
                        ChangeOn = x.RoomChangedAt.ToString("MM/dd/yy"),
                        Id = x.Id.ToString()
                    }));
        }
    }
}
