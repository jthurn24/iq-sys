using System;
using System;
using System.Web.Mvc;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Drug;
using System.Collections;
using IQI.Intuition.Web.Attributes;
using System.Collections.Generic;


namespace IQI.Intuition.Web.Controllers
{
    [RequiresActiveAccount]
    public class DrugController : Controller
    {
        public DrugController(
            IActionContext actionContext, 
            IModelMapper modelMapper, 
            IInfectionRepository infectionRepository,
            IDrugRepository drugRepository)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            DrugRepository = drugRepository.ThrowIfNullArgument("drugRepository");
        }

        protected virtual IActionContext ActionContext { get; private set; }

        protected virtual IModelMapper ModelMapper { get; private set; }

        protected virtual IDrugRepository DrugRepository { get; private set; }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DrugSearchCenter()
        {
            return PartialView();
        }

        public ActionResult SearchNames(string startsWith, string searchFor)
        {
            var results = DrugRepository.Find(searchFor, startsWith);

            var model = new List<NameSearchResult>();

            foreach (var result in results)
            {
                model.Add(new NameSearchResult() { Id = result.Id, Name = result.Name });
            }

            return PartialView(model);
        }

        public ActionResult GetSections(int id)
        {
            var domain = DrugRepository.Get(id);
            var model = new SectionView();
            model.DrugName = domain.Name;
            model.Sections = new List<Section>();



            foreach (var section in domain.Sections)
            {
                var content = section.SectionText;
                content = content.Replace("<content styleCode=\"bold\">", "<span style=\"font-weight:bold;\">");
                content = content.Replace("<content>", "<span>");
                content = content.Replace("</content>", "</span>");
                content = content.Replace("</paragraph>", "</p>");
                content = content.Replace("<paragraph", "<p");

                model.Sections.Add(new Section() { Content = content, Id = section.Id, Title = section.SectionTitle });
            }



            return PartialView(model);
        }

    }
}
