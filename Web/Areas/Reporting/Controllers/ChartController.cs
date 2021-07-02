using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.Security;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Reporting.Graphics;
using System.Drawing;
using SnyderIS.sCore.Persistence;

namespace IQI.Intuition.Web.Areas.Reporting.Controllers
{
     
    public class ChartController : Controller
    {
        protected IDocumentStore _Store;

        public ChartController(
            IActionContext actionContext,
            IDocumentStore store)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            _Store = store;
        }

        protected virtual IActionContext ActionContext { get; private set; }

        [AnonymousAccess]
        public ActionResult RenderFloorMap(string data)
        {
            var stream = FloorMapChart.GenerateImage(data, _Store);
            return File(stream, "image/jpeg");
        }

        [AnonymousAccess]
        public ActionResult RenderSmartMap(string data)
        {
            var stream = SmartFloorMap.GenerateImage(data, _Store);
            return File(stream, "image/jpeg");
        }

        [AnonymousAccess]
        public ActionResult RenderColumnChart(string data, string options, int? width, int? height)
        {
            var stream = ColumnChart.GenerateImage(data,options,width,height);
            return File(stream, "image/jpeg");
        }

        [AnonymousAccess]
        public ActionResult RenderSeriesColumnChart(string data, string options, int? width, int? height)
        {
            var stream = SeriesColumnChart.GenerateImage(data, options, width, height);
            return File(stream, "image/jpeg");
        }

        [AnonymousAccess]
        public ActionResult RenderPieChart(string data, int? width, int? height)
        {
            var stream = PieChart.GenerateImage(data,width,height);
            return File(stream, "image/jpeg");
        }

        [AnonymousAccess]
        public ActionResult RenderLineChart(string data, string options, int? width, int? height)
        {
            var stream = SeriesLineChart.GenerateImage(data, options, width, height);
            return File(stream, "image/jpeg");
        }

        [AnonymousAccess]
        public ActionResult RenderBodyGraph(string data)
        {
            var stream = BodyGraph.GenerateImage(data, Server.MapPath("/Content/images/body.bmp"));
            return File(stream, "image/jpeg");
        }

        [AnonymousAccess]
        public ActionResult RenderVerticalText(string data)
        {
            var stream = VerticalTextLabel.RenderLabel(data);
            return File(stream, "image/png");
        }
    }
}
