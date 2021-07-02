using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Domain.Repositories;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Logging;
using RedArrow.Framework.Extensions.Formatting;
using IQI.Intuition.Domain.Models;
using SnyderIS.sCore.Persistence;
using System.Text.RegularExpressions;
using SnyderIS.sCore.Console;
using System.Xml;
using SnyderIS.sCore.Extensions.Common;
using System.Globalization;

namespace IQI.Intuition.Infrastructure.Services.Importing.Drugs
{
    public class ImportDailyMed : IConsoleService
    {
        private ILog _Log;
        private IStatelessDataContext _DataContext;

        public ImportDailyMed(
            IStatelessDataContext dataContext,
            ILog log
            )
        {
            _DataContext = dataContext;
            _Log = log;
        }

        public void Run(string[] args)
        {
            var fileName = args[1];

            
            if (System.IO.Directory.Exists("c:\\Temp\\DailyMed"))
            {
                System.IO.Directory.Delete("c:\\Temp\\DailyMed", true);
            }

            System.IO.Directory.CreateDirectory("c:\\Temp\\DailyMed");

            System.Console.WriteLine("Extracting file");

            using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(fileName))
            {
                foreach (Ionic.Zip.ZipEntry e in zip)
                {
                    e.Extract("c:\\Temp\\DailyMed");
                }
            }
            
            

            if (System.IO.Directory.Exists("c:\\Temp\\DailyMed\\prescription\\"))
            {
                System.Console.WriteLine("Processing prescriptions");

                foreach (var file in System.IO.Directory.GetFiles("c:\\Temp\\DailyMed\\prescription\\"))
                {
                    if (file.ToLower().EndsWith(".zip"))
                    {
                        System.Console.WriteLine("Processing {0}", file);
                        ProcessChildZip(file);
                        System.IO.File.Delete(file);
                    }
                }
            }

        }

        public void ProcessChildZip(string path)
        {
            if (System.IO.Directory.Exists("c:\\Temp\\DailyMed\\Working"))
            {
                System.IO.Directory.Delete("c:\\Temp\\DailyMed\\Working", true);
            }

            System.IO.Directory.CreateDirectory("c:\\Temp\\DailyMed\\Working");

            using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(path))
            {
                foreach (Ionic.Zip.ZipEntry e in zip)
                {
                    e.Extract("c:\\Temp\\DailyMed\\Working");
                }
            }

            foreach (var file in System.IO.Directory.GetFiles("c:\\Temp\\DailyMed\\Working\\"))
            {
                if (file.ToLower().EndsWith(".xml"))
                {
                    System.Console.WriteLine("Reading file {0}", file);
                    ProcessXml(file, System.IO.Path.GetFileNameWithoutExtension(path));
                }
            }

        }


        public void ProcessXml(string path, string identifier)
        {
            var xml = new System.Xml.XmlDocument();
            xml.Load(path);

            var nsManager = new XmlNamespaceManager(xml.NameTable);

            string xmlns = xml.DocumentElement.Attributes["xmlns"].Value;

            nsManager.AddNamespace("x",xmlns);

            var sections = new List<Section>();
            var products = new List<string>();

            var componentsNodes = xml.SelectNodes("/x:document/x:component/x:structuredBody/x:component", nsManager);
            var productNodes = xml.SelectNodes("/x:document/x:component/x:structuredBody/x:component/x:section/x:subject/x:manufacturedProduct/x:manufacturedProduct", nsManager);
            var medicineNodes = xml.SelectNodes("/x:document/x:component/x:structuredBody/x:component/x:section/x:subject/x:manufacturedProduct/x:manufacturedMedicine", nsManager);


            foreach (System.Xml.XmlNode productNode in productNodes)
            {
                LoadProduct(productNode, products,nsManager);
            }

            foreach (System.Xml.XmlNode productNode in medicineNodes)
            {
                LoadProduct(productNode, products, nsManager);
            }

            foreach (System.Xml.XmlNode componentNode in componentsNodes)
            {
                LoadComponent(componentNode, sections,nsManager);
            }


            foreach (var product in products)
            {
                var dProduct = _DataContext.CreateQuery<Drug>().FilterBy(x => x.Name == product).FetchAll().FirstOrDefault();

                if (dProduct == null)
                {
                    dProduct = new Drug();
                    dProduct.Name = product;
                    dProduct.DrugType = Domain.Enumerations.DrugType.Prescription;
                    _DataContext.Insert(dProduct);
                }

                dProduct.SourceIdentifier = identifier;
                _DataContext.Update(dProduct);

                foreach (var dsection in _DataContext.CreateQuery<DrugSection>().FilterBy(x => x.Drug.Id == dProduct.Id).FetchAll())
                {
                    _DataContext.Delete(dsection);
                }

                foreach (var section in sections)
                {
                    var dsection = new DrugSection();
                    dsection.Drug = dProduct;
                    dsection.SectionName = section.Name;
                    dsection.SectionTitle = section.Title;
                    dsection.SectionText = section.Content;
                    _DataContext.Insert(dsection);
                }

            }
        }

        private void LoadComponent(System.Xml.XmlNode node, List<Section> sections, XmlNamespaceManager nsManager)
        {
            var sectionNode = node.SelectSingleNode("x:section", nsManager);

            if (sectionNode == null)
            {
                return;
            }

            var codeNode = sectionNode.SelectSingleNode("x:code", nsManager);
            var titleNode = sectionNode.SelectSingleNode("x:title", nsManager);
            var excerptNode = sectionNode.SelectSingleNode("x:excerpt", nsManager);

            var textNodes = new List<System.Xml.XmlNode>();

            var directTextNode = sectionNode.SelectSingleNode("x:text", nsManager);

            if (directTextNode != null)
            {
                textNodes.Add(directTextNode);
            }

            if (excerptNode != null)
            {
                var highlightNode = excerptNode.SelectSingleNode("x:highlight", nsManager);

                if (highlightNode != null)
                {
                    var highlighTextNode = highlightNode.SelectSingleNode("x:text", nsManager);
                    textNodes.Add(highlighTextNode);
                }
            }

            /* IF we have text nodes somewhere in here, add this section */

            if (textNodes.Count() > 0)
            {
                var section = new Section();
                var ti = new CultureInfo("en-US", false).TextInfo;


                if (codeNode != null && codeNode.Attributes["displayName"] != null)
                {
                    section.Name = codeNode.Attributes["displayName"].Value;
                }
                else
                {
                    section.Name = string.Empty;
                }

                if (titleNode != null)
                {
                    section.Title = ti.ToTitleCase(titleNode.InnerText.ToLower());
                }
                else
                {
                    section.Title = ti.ToTitleCase(section.Name.Replace("SECTION",string.Empty).ToLower());
                }

                section.Content = string.Empty;

                foreach (var tnode in textNodes)
                {
                    section.Content = string.Concat(section.Content, tnode.InnerXml); 
                }

                if (section.Content != null && section.Content != string.Empty)
                {
                    sections.Add(section);
                }
            }

            /* Handle child components */

            var componentsNodes = sectionNode.SelectNodes("x:component", nsManager);

            if (componentsNodes.Count > 0)
            {
                foreach (System.Xml.XmlNode childNode in componentsNodes)
                {
                    LoadComponent(childNode, sections, nsManager);
                }
            }
          
        }

        private void LoadProduct(System.Xml.XmlNode node, List<String> products, XmlNamespaceManager nsManager)
        {
            var nameNode = node.SelectSingleNode("x:name",nsManager);

            if (nameNode != null)
            {
                var ti = new CultureInfo("en-US", false).TextInfo;
                var name = ti.ToTitleCase(nameNode.InnerText.ToLower()).Trim();

                if (products.Contains(name) == false)
                {
                    products.Add(name);
                }
            }
        }


        public class Section
        {
            public string Name { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }
        }

    }
}
