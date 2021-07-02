using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Domain;

namespace IQI.Intuition.Web.Models.Navigation
{
    public class NavigationItem
    {
        public string Text { get; set; }
        public string Link { get; set; }
        public string Image { get; set; }
        public bool IsSeparator { get; set; }
        public List<NavigationItem> SubItems { get; set; }
        private IActionContext ActionContext { get; set; }
        private bool RequiresSubItems { get; set; }

        public NavigationItem(IActionContext actionContext)
        {
            SubItems = new List<NavigationItem>();
            ActionContext = actionContext;
        }

        public NavigationItem Add(string text, string link)
        {
            SubItems.Add(new NavigationItem(ActionContext) { Link = link, Text = text });
            return this;
        }

        public NavigationItem Add(string text, string link, Enumerations.KnownProductType productType)
        {
            if (ActionContext.CurrentFacility.HasProduct(productType) == false)
            { return this; }

            SubItems.Add(new NavigationItem(ActionContext) { Link = link, Text = text });
            return this;
        }

        public NavigationItem Add(string text, string link, Enumerations.KnownPermision permission )
        {
            if (ActionContext.CurrentUser.HasPermission(permission) == false)
            { return this; }
        
            SubItems.Add(new NavigationItem(ActionContext) { Link = link, Text = text });
            return this;
        }

        public NavigationItem AddSeparator(string text, string image)
        {
            SubItems.Add(new NavigationItem(ActionContext) { Link = "#", Text = text, Image = image, IsSeparator = true });
            return this;
        }

        public NavigationItem Add(string text, string link, Enumerations.KnownProductType productType, Enumerations.KnownPermision permission)
        {
            if (ActionContext.CurrentFacility.HasProduct(productType) == false)
            { return this; }

            if (ActionContext.CurrentUser.HasPermission(permission) == false)
            { return this; }

            SubItems.Add(new NavigationItem(ActionContext) { Link = link, Text = text });
            return this;
        }

        public NavigationItem AddForSystemUser(string text, string link)
        {
            if (ActionContext.CurrentUser.SystemUser == false)
            { return this; }

            SubItems.Add(new NavigationItem(ActionContext) { Link = link, Text = text });
            return this;
        }

        public NavigationItem AddForMultiFacilityUser(string text, string link)
        {
            if (ActionContext.CurrentUser.Facilities.Count() > 1 || ActionContext.CurrentUser.SystemUser)
            {
                SubItems.Add(new NavigationItem(ActionContext) { Link = link, Text = text });
            }

            return this;
        }

        public NavigationItem SetText(string text)
        {
            Text = text;
            return this;
        }

        public NavigationItem SetLink(string link)
        {
            Link = link;
            return this;
        }

        public NavigationItem RequireSubItems()
        {
            this.RequiresSubItems = true;
            return this;
        }

        public NavigationItem AddToList(List<NavigationItem> items)
        {
            if (this.RequiresSubItems && this.SubItems.Count() < 1)
            {
                return this;
            }

            items.Add(this);
            return this;
        }

    }
}
