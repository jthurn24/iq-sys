using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.Renderer;
using SnyderIS.sCore.Exi.Mvc.Renderers;
using System.Linq.Expressions;
using SnyderIS.sCore.Exi.Interfaces.DataSource;

namespace IQI.Intuition.Exi.Renderers.QICast
{
    public class NoteView : MvcRenderer<Exi.Models.QICast.Note>
    {
        public override string RenderHtml(IDataSourceResult<Exi.Models.QICast.Note> data)
        {
            var note = data.Metrics.First();
            return this.RenderPartialView("~/Views/Shared/Exi/QICast/Note.cshtml", note);
        }

        public override string Description
        {
            get { throw new NotImplementedException(); }
        }

        public override string Name
        {
            get { throw new NotImplementedException(); }
        }
    }
}
