﻿using System;
using System.Web.Mvc;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using System.Collections;
using IQI.Intuition.Web.Attributes;
using System.Collections.Generic;
using RedArrow.Framework.Mvc.Security;
using IQI.Intuition.Web.Extensions;
using IQI.Intuition.Web.Models.Resource;
using Trirand.Web.Mvc;

namespace IQI.Intuition.Web.Areas.Administration.Controllers
{
    [AnonymousAccess, RequiresSystemUserAttribute(Domain.Enumerations.SystemUserRole.Admin)]
    public class ResourceController : Controller
    {
        public ResourceController(
			IActionContext actionContext, 
			IModelMapper modelMapper,
            IResourceRepository resourceRepository)
		{
			ActionContext = actionContext.ThrowIfNullArgument("actionContext");
			ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            ResourceRepository = resourceRepository.ThrowIfNullArgument("resourceRepository");
		}

		protected virtual IActionContext ActionContext { get; private set; }

		protected virtual IModelMapper ModelMapper { get; private set; }

        protected virtual IResourceRepository ResourceRepository { get; private set; }

        public ActionResult Index()
        {
            int selectedNode = -1;

            if (Session["SelectedNode"] != null)
            {
                selectedNode = (int)Session["SelectedNode"];
            }

            return View(selectedNode);
        }

        public ActionResult FolderList()
        {
            var tree = new JQTreeView();
            var nodes = new List<JQTreeNode>();

            foreach (var resourceFolder in ResourceRepository.GetRoot())
            {
                nodes.Add(MapFolder(resourceFolder,1));
            }

            return tree.DataBind(nodes);
        }

        private JQTreeNode MapFolder(ResourceFolder domain, int level)
        {
            var node = new JQTreeNode();
            node.ImageUrl = "/content/images/folder.png";
            node.Text = domain.Name;

            if (level < 2)
            {
                node.Expanded = true;
            }
  
            node.Value = domain.Id.ToString();

            foreach (var child in domain.Children.OrderBy(x => x.Name))
            {
                node.Nodes.Add(MapFolder(child, level++));
            }

            return node;
        }

        public ActionResult AddFolder(string name, int parent)
        {
            

            var resourceFolder = new ResourceFolder();
            resourceFolder.Parent = ResourceRepository.GetFolder(parent);
            resourceFolder.Name = name;
            ResourceRepository.Add(resourceFolder);

            Session["SelectedNode"] = resourceFolder.Id;

            return RedirectToAction("Index");
        }

        public ActionResult AddLink(string name, string description, int parent, string link)
        {
            Session["SelectedNode"] = parent;

            var resource = new Resource();
            resource.ResourceType = Domain.Enumerations.ResourceType.Link;
            resource.Folder = ResourceRepository.GetFolder(parent);
            resource.Link = link;
            resource.Name = name;
            resource.Description = description;
            resource.CreatedOn = DateTime.Today;
            ResourceRepository.Add(resource);

            return RedirectToAction("Index");
        }

        public ActionResult AddFile(string name, string description, int parent, System.Web.HttpPostedFileBase file)
        {
            Session["SelectedNode"] = parent;

            var resource = new Resource();
            resource.Name = name;
            resource.Description = description;
            resource.ResourceType = Domain.Enumerations.ResourceType.File;
            resource.Folder = ResourceRepository.GetFolder(parent);
            resource.FileExtension = System.IO.Path.GetExtension(file.FileName);
            resource.CreatedOn = DateTime.Today;
            ResourceRepository.Add(resource);

            var path = string.Concat(Server.MapPath("/Content/Resources/"), resource.Id);
            file.SaveAs(path);

            return RedirectToAction("Index");

        }

        public ActionResult DeleteResource(int id)
        {
            var resource = ResourceRepository.Get(id);

            Session["SelectedNode"] = resource.Folder.Id;

            ResourceRepository.Delete(resource);
            return RedirectToAction("Index");
        }

        public ActionResult DownloadResource(int id)
        {
            var resource = ResourceRepository.Get(id);
            var path = string.Concat(Server.MapPath("/Content/Resources/"), resource.Id);
            return File(path,"application/octet-stream",string.Concat(resource.Name,".",resource.FileExtension));
        }

        public ActionResult LoadFolder(int id)
        {
            var domain = ResourceRepository.GetFolder(id);
            var model = new ResourceEntryList();
            model.Entries = new List<ResourceEntry>();
            model.FolderId = id;
            model.Path = domain.GetFullPath();

            foreach (var resource in domain.Resources.OrderBy(x => x.Name))
            {
                var entry = new ResourceEntry();
                entry.Description = resource.Description;
                entry.Id = resource.Id;
                entry.Name = resource.Name;
                entry.ResourceType = resource.ResourceType;
                entry.SuggestedBy = string.Empty;
                entry.CreatedOn = resource.CreatedOn.Value.ToString("MM/dd/yy");

                if (resource.ResourceType == Domain.Enumerations.ResourceType.Link)
                {
                    entry.Link = resource.Link;
                }
                else
                {
                    entry.Link = Url.Action("DownloadResource", new { id = resource.Id });
                }

                model.Entries.Add(entry);
            }

            return PartialView(model);
            
        }


    }
}