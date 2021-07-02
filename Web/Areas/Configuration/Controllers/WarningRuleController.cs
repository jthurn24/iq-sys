using System;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Web.Models.Configuration.Warning;

namespace IQI.Intuition.Web.Areas.Configuration.Controllers
{
    public class WarningRuleController : Controller
    {
        public WarningRuleController(
            IActionContext actionContext,
            IModelMapper modelMapper,
            IWarningRepository warningRepository,
            IDataContext dataContext)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            WarningRepository = warningRepository.ThrowIfNullArgument("WarningRepository");
            DataContext = dataContext.ThrowIfNullArgument("DataContext");
        }

        protected virtual IActionContext ActionContext { get; private set; }

        protected virtual IModelMapper ModelMapper { get; private set; }

        protected virtual IWarningRepository WarningRepository { get; private set; }

        protected virtual IDataContext DataContext { get; private set; }

        [HttpGet]
        public ActionResult Index()
        {
            var model = new List<WarningRuleDescription>();
           
            var rules = WarningRepository.GetForFacility(ActionContext.CurrentFacility.Id);

            foreach (var rule in rules)
            {
                var service = IQI.Intuition.Infrastructure.Services.Warnings.BaseService.Activate(rule);

                model.Add(new WarningRuleDescription() { 
                    RuleType = service.DescribeRuleType(rule,DataContext),
                    Arguments = service.DescribeArguments(rule,DataContext),
                    Description = rule.Description,
                    Title = rule.Title
                });
            }

            return View(model);
        }
    }
}
