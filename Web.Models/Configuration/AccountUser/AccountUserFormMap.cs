using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.Extensions;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Web.Models.Extensions;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Domain.Repositories;

namespace IQI.Intuition.Web.Models.Configuration.AccountUser
{
    public class AccountUserFormMap : ModelMap<AccountUserForm, Domain.Models.AccountUser>
    {
        private IActionContext _ActionContext { get; set; }
        private ISystemRepository _SystemRepository { get; set; }

        public AccountUserFormMap(
            IActionContext actionContext,
            ISystemRepository systemRepository)
        {
            _ActionContext = actionContext;
            _SystemRepository = systemRepository;

            AutoFormatDisplayNames();

            ForProperty(model => model.Id)
                .Bind(domain => domain.Id)
                .Exclude(On.Create)
                .HiddenInput();

            ForProperty(model => model.Login)
                .Bind(domain => domain.Login)
                .Verify(ValidateLogin).ErrorMessage("Login name already exists")
                .Required();

            ForProperty(model => model.FirstName)
                .Bind(domain => domain.FirstName)
                .Required();

            ForProperty(model => model.LastName)
                .Bind(domain => domain.LastName)
                .Required();

            ForProperty(model => model.EmailAddress)
                .Bind(domain => domain.EmailAddress)
                .Required();

            ForProperty(model => model.CellPhone)
                .Bind(domain => domain.CellPhone)
                .Required();

            ForProperty(model => model.IsActive)
                .Bind(domain => domain.IsActive);

            ForProperty(model => model.Password)
                .Read(x => string.Empty)
                .Write(SetPassword)
                .Password()
                .Required(On.Create);

            ForProperty(model => model.SelectedFacilities)
                .Read(domain => domain.Facilities.Select(item => item.Id).ToList())
                .Write((domain, value) => domain
                    .AssignFacilities(ConvertFacilitySelections(value)))
                .Default(new List<int>())
                .CheckBoxList(GenerateFacilityItems());

            ForProperty(model => model.SelectedPermissions)
                .Read(domain => domain.Permissions.Select(item => item.Id).ToList())
                .Write((domain, value) => domain
                    .AssignPermissions(ConvertPermissionSelections(value)))
                .Default(new List<int>())
                .CheckBoxList(GeneratePermissionItems());

            ForProperty(model => model.SelectedWarningRules)
                .Read(domain => domain.Notifications.Select(item => item.WarningRule.Id).ToList())
                .Write((domain, value) => domain.Notifications = ConvertNotificationSelections(value, domain) )
                .Default(new List<int>())
                .CheckBoxList(GenerateNotificationItems());

            ForProperty(model => model.NotificationMethod)
                .Bind(domain => domain.NotificationMethod)
                .EnumList();

        }

        private bool ValidateLogin(AccountUserForm form, string value)
        {
            if (_ActionContext.CurrentAccount.Users.Where(x => x.Login == value && x.Id != form.Id).Count() > 0)
            {
                return false;
            }

            return true;
        }

        private IEnumerable<SelectListItem> GenerateFacilityItems()
        {
            return _ActionContext.CurrentAccount.Facilities
                .ToSelectListItems(
                    item => item.Name,
                    item => item.Id);
        }

        private IEnumerable<SelectListItem> GeneratePermissionItems()
        {
            return _SystemRepository.GetAllPermissions()
                .ToSelectListItems(
                    item => item.Name,
                    item => item.Id);
        }

        private IEnumerable<SelectListItem> GenerateNotificationItems()
        {


            return _ActionContext.CurrentFacility.WarningRules
                .ToSelectListItems(
                    item => item.ParsedTitle,
                    item => item.Id);
        }

        private void SetPassword(Domain.Models.AccountUser domain, string value)
        {
            if (value.IsNotNullOrEmpty())
            {
                domain.ChangePassword(value);
            }
        }

        private Facility[] ConvertFacilitySelections(IEnumerable<int> selections)
        {
            return _ActionContext.CurrentAccount.Facilities
                .Where(item => selections
                    .EmptyIfNull()
                    .Contains(item.Id))
                .ToArray();
        }

        private Permission[] ConvertPermissionSelections(IEnumerable<int> selections)
        {
            return _SystemRepository.GetAllPermissions()
                .Where(item => selections
                    .EmptyIfNull()
                    .Contains(item.Id))
                .ToArray();
        }

        private WarningRuleNotification[] ConvertNotificationSelections(IEnumerable<int> selections, Domain.Models.AccountUser user)
        {
            if(selections == null)
            {
                return  new WarningRuleNotification[0];
            }

            return selections.Select(x => new WarningRuleNotification()
            {
                WarningRule = _ActionContext.CurrentFacility.WarningRules.Where(xx => xx.Id == x).First(),
                AccountUser = user
            })
            .ToArray();
        }

    }
}
