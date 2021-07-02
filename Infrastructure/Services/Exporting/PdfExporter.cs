using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Models.User;
using System.Diagnostics;
using System.IO;
using RedArrow.Framework.Logging;
using SnyderIS.sCore.Persistence;
using IQI.Intuition.Reporting.Repositories;
using PdfSharp.Pdf;

namespace IQI.Intuition.Infrastructure.Services.Exporting
{
    
    public class PdfExporter
    {
        private ExportRequest _Request;
        
        private string _WkHtmlToPdfPath = System.Configuration.ConfigurationSettings.AppSettings["WkhtmkToPdfPath"];
        private string _OutputRootPath = System.Configuration.ConfigurationSettings.AppSettings["PdfOutputroot"];
        private ILog _Log;
        private IUserRepository _UserRepository;

        public PdfExporter(
            ExportRequest request,
            ILog log,
            IUserRepository userRepository)
        {
            _Request = request;
            _Log = log;
            _UserRepository = userRepository;
        }


        public void Export()
        {
            var exportedPaths = new List<string>();


            string outputFile = string.Concat(_OutputRootPath,"Export",_Request.Id,".pdf");

            int counter = 0;

            foreach (var path in _Request.ExportPaths)
            {
                counter++;
                var doc = BuildPdf(path);
                if (doc != null)
                {
                    exportedPaths.Add(doc);
                }

                System.Console.WriteLine(counter);
            }

            if (exportedPaths.Count() < 1)
            {
                _Request.Status = ExportRequest.ExportRequestStatus.Error;
            }
            else
            {
                var pdfDoc = new PdfSharp.Pdf.PdfDocument();

                foreach (string exportedPath in exportedPaths)
                {

                    var d = PdfSharp.Pdf.IO.PdfReader.Open(exportedPath, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
 
                    foreach (PdfPage p in d.Pages)
                    {
                        System.IO.File.Delete(exportedPath);
                        pdfDoc.AddPage(p);
                    }
                }

                int page = 0;
                foreach (PdfPage p in pdfDoc.Pages)
                {
                    page++;
                    var graphics = PdfSharp.Drawing.XGraphics.FromPdfPage(p, PdfSharp.Drawing.XGraphicsPdfPageOptions.Append);

                    PdfSharp.Drawing.XStringFormat right = new PdfSharp.Drawing.XStringFormat();
                    right.Alignment = PdfSharp.Drawing.XStringAlignment.Far;
                    right.LineAlignment = PdfSharp.Drawing.XLineAlignment.Near;

                    graphics.DrawString(string.Format("Page {0} of {1}", page, pdfDoc.Pages.Count), new PdfSharp.Drawing.XFont("Verdana", 8, PdfSharp.Drawing.XFontStyle.Bold), PdfSharp.Drawing.XBrushes.Black,
                        new PdfSharp.Drawing.XRect(20, p.Height - 20, p.Width - 40, 20),
                        PdfSharp.Drawing.XStringFormats.TopLeft);

                    graphics.DrawString("(c) 2012 IQI Systems LLC", new PdfSharp.Drawing.XFont("Verdana", 8, PdfSharp.Drawing.XFontStyle.Bold), PdfSharp.Drawing.XBrushes.Black,
                        new PdfSharp.Drawing.XRect(20, p.Height - 20, p.Width - 40, 20),
                        right);

                }

                var dataStream = new MemoryStream();

                pdfDoc.Save(dataStream);


                _Request.OutputFile = dataStream.ToArray();
                _Request.Status = ExportRequest.ExportRequestStatus.Completed;

                _Log.Info("Export Complete");
            }

            _UserRepository.Update(_Request);
        }


        private string BuildPdf(ExportRequest.ExportRequestPath requestPath)
        {
            System.Console.WriteLine(requestPath.Path);

            try
            {
                string outputFile = string.Concat(_OutputRootPath, requestPath.Id, ".pdf");

                string args;

                if (requestPath.Landscape == true)
                {
                    args = string.Format("  -O Landscape  \"{0}\" {1} ", requestPath.Path, outputFile);
                }
                else
                {
                    args = string.Format("\"{0}\" {1} ", requestPath.Path, outputFile);
                }
                
                //_Log.Info(String.Concat("Starting: ",_WkHtmlToPdfPath, " ", args));

                Process p = new Process();
                p.StartInfo = new ProcessStartInfo(_WkHtmlToPdfPath);
                p.StartInfo.Arguments = args;
                p.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(_WkHtmlToPdfPath);
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
                p.WaitForExit((int)TimeSpan.FromMinutes(2).TotalMilliseconds);

                //_Log.Info(String.Concat("Finished: ", _WkHtmlToPdfPath, " ", args));

                return outputFile;
            }
            catch (Exception ex)
            {
                this._Log.Error(ex);
                return null;
            }

        }

    }
}
