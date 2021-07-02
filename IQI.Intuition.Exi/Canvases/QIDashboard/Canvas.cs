using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Implementation.Canvas.XmlSerializable;
using SnyderIS.sCore.Exi.Interfaces.Canvas;
using IQI.Intuition.Reporting.Repositories;

namespace IQI.Intuition.Exi.Canvases.QIDashboard
{
    public class Canvas : SnyderIS.sCore.Exi.Implementation.Canvas.XmlSerializable.Canvas
    {
        private IUserRepository _UserRepository { get; set; }
        private Guid _UserId { get; set; }

        public Canvas(Guid id, IUserRepository userRepository)
        {
            _UserRepository = userRepository;
            _UserId = id;

            var data = _UserRepository.GetOrCreateDashboardData(id);

            if (data.Dashboards == null)
            {
                var section = this.CreateSection(2, false, "Default");
                this.SetDefaultSection(section.Identifier);
            }
            else
            {
                LoadPrivate(data.Dashboards.First());
            }
        }

        public override void UpdatePrivate(string data)
        {
            var d = _UserRepository.GetOrCreateDashboardData(this._UserId);

            if (d.Dashboards == null)
            {
                d.Dashboards = new List<string>();
            }

            d.Dashboards.Clear();
            d.Dashboards.Add(data);
            _UserRepository.Update(d);
        }

        public override void UpdateShared(string data, Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
