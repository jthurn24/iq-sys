using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using RedArrow.Framework.Extensions.Common;
using IQI.Intuition.Domain.Utilities;
using System.Linq.Expressions;

namespace IQI.Intuition.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString HorizontalCheckBoxFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, IEnumerable<SelectListItem> items)
        {
            return helper.EditorFor(expression, "HorizontalCheckBoxList", new  { SelectListItems =  items  });
        }

        public static MvcHtmlString SmartDropDownList(this HtmlHelper helper, string name, IEnumerable<SelectListItem> listItems)
        {
            return helper.SmartDropDownList(name, listItems, null);
        }

        public static MvcHtmlString SmartDropDownList(this HtmlHelper helper, string name, IEnumerable<SelectListItem> listItems, object htmlAttributes)
        {
            if (!helper.ViewData.ModelMetadata.ShowForEdit)
            {
                return MvcHtmlString.Empty;
            }

            if (!helper.ViewData.ModelMetadata.IsReadOnly)
            {
                // Adding the model to ViewData so that the matching item in the list is selected automatically
                helper.ViewData[helper.ViewData.TemplateInfo.HtmlFieldPrefix] = helper.ViewData.Model;

                var sl = new SelectList(listItems, "Value","Text", helper.ViewData.Model);

                return helper.DropDownList(name, sl, htmlAttributes);
            }

            string value = helper.ViewData.Model.ToStringSafely();
            var selectedItem = listItems.FirstOrDefault(x => x.Value == value);
            string selectedItemText = "";

            if (selectedItem != null)
            {
                selectedItemText = selectedItem.Text;
            }

            string displayFormatString = helper.ViewData.ModelMetadata.DisplayFormatString;

            if (displayFormatString.IsNotNullOrWhiteSpace())
            {
                selectedItemText = displayFormatString.FormatWith(selectedItemText);
            }

            return MvcHtmlString.Create(selectedItemText + helper.Hidden(name, value));
        }

        public static void RenderExportPanel(this HtmlHelper helper)
        {
            RenderExportPanel(helper, false, false);
        }

        public static void RenderExportPanel(this HtmlHelper helper, bool disabledEmailExport, bool disableComposite)
        {
            string path = UrlEncodeHelper.UrlEncode(helper.ViewContext.RequestContext.HttpContext.Request.Url.AbsoluteUri);
            bool landscape = false;


            if (helper.ViewData["ExportPath"] != null)
            {
                path = helper.ViewData["ExportPath"].ToString();
            }

            if (helper.ViewData["ExportLandscape"] != null)
            {
                landscape = Boolean.Parse(helper.ViewData["ExportLandscape"].ToString());
            }

            helper.RenderAction("RenderExportPanel", 
                new { 
                    Controller = "Export", 
                    Area = "", 
                    path = path, 
                    title = helper.ViewData["Layout_ContentTitle"], 
                    disabledEmailExport = disabledEmailExport, 
                    disableComposite = disableComposite,
                    landscape = landscape
                });
        }

        public static MvcHtmlString SmartLink(this HtmlHelper helper, string destination, string text, string prompt)
        {
            var tagBuilder = new TagBuilder("a");
            tagBuilder.Attributes.Add("data-url", destination);
            tagBuilder.Attributes.Add("href", "#");
            tagBuilder.Attributes.Add("data-prompt", prompt);
            tagBuilder.AddCssClass("link");
            tagBuilder.SetInnerText(text);
            return new MvcHtmlString(tagBuilder.ToString());
        }

        public static MvcHtmlString SmartLink(this HtmlHelper helper, string destination, string text)
        {
            var tagBuilder = new TagBuilder("a");
            tagBuilder.Attributes.Add("data-url", destination);
            tagBuilder.Attributes.Add("href", "#");
            tagBuilder.AddCssClass("link");
            tagBuilder.SetInnerText(text);
            return new MvcHtmlString(tagBuilder.ToString());
        }

        public static MvcHtmlString SmartLink(this HtmlHelper helper, string destination, string text, object htmlAttributes)
        {
            var tagBuilder = new TagBuilder("a");
            tagBuilder.Attributes.Add("data-url", destination);
            tagBuilder.Attributes.Add("href", "#");
            tagBuilder.AddCssClass("link");
            tagBuilder.MergeAttributes(new System.Web.Routing.RouteValueDictionary(htmlAttributes));
            tagBuilder.SetInnerText(text);
            return new MvcHtmlString(tagBuilder.ToString());
        }

        public static MvcHtmlString SmartButton(this HtmlHelper helper, string destination, string text, string prompt)
        {
            var tagBuilder = new TagBuilder("input");
            tagBuilder.Attributes.Add("type", "button");
            tagBuilder.Attributes.Add("value", text);
            tagBuilder.Attributes.Add("data-url", destination);
            tagBuilder.Attributes.Add("data-prompt", prompt);
            tagBuilder.AddCssClass("link");
            return new MvcHtmlString(tagBuilder.ToString());
        }

        public static MvcHtmlString SmartButton(this HtmlHelper helper, string destination, string text, string prompt, object htmlAttributes)
        {
            var tagBuilder = new TagBuilder("input");
            tagBuilder.Attributes.Add("type", "button");
            tagBuilder.Attributes.Add("value", text);
            tagBuilder.Attributes.Add("data-url", destination);
            tagBuilder.Attributes.Add("data-prompt", prompt);
            tagBuilder.MergeAttributes(new System.Web.Routing.RouteValueDictionary(htmlAttributes));
            tagBuilder.AddCssClass("link");
            return new MvcHtmlString(tagBuilder.ToString());
        }

        public static MvcHtmlString SmartButton(this HtmlHelper helper, string destination, string text, object htmlAttributes)
        {
            var tagBuilder = new TagBuilder("input");
            tagBuilder.Attributes.Add("type", "button");
            tagBuilder.Attributes.Add("value", text);
            tagBuilder.Attributes.Add("data-url", destination);
            tagBuilder.MergeAttributes(new System.Web.Routing.RouteValueDictionary(htmlAttributes));
            tagBuilder.AddCssClass("link");
            return new MvcHtmlString(tagBuilder.ToString());
        }

        public static MvcHtmlString SmartButton(this HtmlHelper helper, string destination, string text)
        {
            var tagBuilder = new TagBuilder("input");
            tagBuilder.Attributes.Add("type", "button");
            tagBuilder.Attributes.Add("value", text);
            tagBuilder.Attributes.Add("data-url", destination);
            tagBuilder.AddCssClass("link");
            return new MvcHtmlString(tagBuilder.ToString());
        }

        public static MvcHtmlString SmartSubmit(this HtmlHelper helper, string text, string prompt)
        {
            var tagBuilder = new TagBuilder("input");
            tagBuilder.Attributes.Add("value", text);
            tagBuilder.Attributes.Add("type", "button");
            tagBuilder.Attributes.Add("data-submit", "true");
            tagBuilder.Attributes.Add("data-prompt", prompt);
            tagBuilder.AddCssClass("link");
            return new MvcHtmlString(tagBuilder.ToString());
        }

        public static MvcHtmlString SmartSubmit(this HtmlHelper helper, string text, object htmlAttributes)
        {
            var tagBuilder = new TagBuilder("input");
            tagBuilder.Attributes.Add("value", text);
            tagBuilder.Attributes.Add("type", "button");
            tagBuilder.Attributes.Add("data-submit", "true");
            tagBuilder.MergeAttributes(new System.Web.Routing.RouteValueDictionary(htmlAttributes));
            tagBuilder.AddCssClass("link");
            return new MvcHtmlString(tagBuilder.ToString());
        }

        public static MvcHtmlString SmartSubmit(this HtmlHelper helper, string text, string prompt, object htmlAttributes)
        {
            var tagBuilder = new TagBuilder("input");
            tagBuilder.Attributes.Add("value", text);
            tagBuilder.Attributes.Add("type", "button");
            tagBuilder.Attributes.Add("data-submit", "true");
            tagBuilder.Attributes.Add("data-prompt", prompt);
            tagBuilder.MergeAttributes(new System.Web.Routing.RouteValueDictionary(htmlAttributes));
            tagBuilder.AddCssClass("link");
            return new MvcHtmlString(tagBuilder.ToString());
        }

        public static MvcHtmlString SmartSubmit(this HtmlHelper helper, string text)
        {
            var tagBuilder = new TagBuilder("input");
            tagBuilder.Attributes.Add("value", text);
            tagBuilder.Attributes.Add("type", "button");
            tagBuilder.Attributes.Add("data-submit", "true");
            tagBuilder.AddCssClass("link");
            return new MvcHtmlString(tagBuilder.ToString());
        }

        public static MvcHtmlString SmartCancel(this HtmlHelper helper, string text)
        {
            var tagBuilder = new TagBuilder("button");
            tagBuilder.Attributes.Add("name", "cancel");
            tagBuilder.Attributes.Add("value", "true");
            tagBuilder.Attributes.Add("type", "submit");
            tagBuilder.SetInnerText(text);
            return new MvcHtmlString(tagBuilder.ToString());
        }

        public static MvcHtmlString SmartCancel(this HtmlHelper helper, string text, object htmlAttributes)
        {
            var tagBuilder = new TagBuilder("button");
            tagBuilder.Attributes.Add("name", "cancel");
            tagBuilder.Attributes.Add("value", "true");
            tagBuilder.Attributes.Add("type", "submit");
            tagBuilder.SetInnerText(text);
            tagBuilder.MergeAttributes(new System.Web.Routing.RouteValueDictionary(htmlAttributes));
            return new MvcHtmlString(tagBuilder.ToString());
        }

        public static string RenderAuditLink(this HtmlHelper helper, IQI.Intuition.Web.Models.Audit.AuditRequest auditRequest)
        {
            return RenderAuditLink(helper, auditRequest, 1);
        }

        public static string RenderAuditLink(this HtmlHelper helper, IQI.Intuition.Web.Models.Audit.AuditRequest auditRequest, int page)
        {
            var builder = new System.Text.StringBuilder();
            var urlHelper = new UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);

            builder.Append(urlHelper.Action("ViewAudit","Audit", new { area = "" }));

            builder.Append("?PatientId=");
            builder.Append(auditRequest.PatientId);
            builder.Append("&ComponentId=");
            builder.Append(auditRequest.ComponentId);
            builder.Append("&Page=");
            builder.Append(page);
            builder.Append("&AuditDescription=");
            builder.Append(UrlEncodeHelper.UrlEncode(auditRequest.AuditDescription));


            if (auditRequest.Types != null && auditRequest.Types.Count() > 0)
            {
                builder.Append("&Types=");
                builder.Append(auditRequest.Types.ToDelimitedString(','));
            }

            

            return builder.ToString();
        }

        public static MvcHtmlString RenderAnnotation(this HtmlHelper helper,
            IQI.Intuition.Reporting.Models.AnnotatedEntry e, int val)
        {

            if (val == 0)
            {
                return new MvcHtmlString(val.ToString());
            }

            return RenderAnnotation(helper, e, val.ToString());
        }

        public static MvcHtmlString RenderAnnotation(this HtmlHelper helper, 
            IQI.Intuition.Reporting.Models.AnnotatedEntry e, decimal val)
        {

            if (val == 0)
            {
                return new MvcHtmlString(val.ToString("#0.00"));
            }

            return RenderAnnotation(helper, e, val.ToString("#0.00"));
        }

        public static MvcHtmlString RenderAnnotation(this HtmlHelper helper,
        IQI.Intuition.Reporting.Models.AnnotatedEntry e, string val)
        {

            var urlHelper = new UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);

            var url = string.Concat(
                urlHelper.Action(e.ViewAction, "Annotation", new { area = "Reporting" }),
                "?guids=",
                e.Components.ToDelimitedString(','));

            var link = string.Format("<span class=\"annotationLink\" onclick=\"clickAnnotations(this,'{0}')\">{1}</span>",
                url, val);

            return new MvcHtmlString(link);
        }

    }
}