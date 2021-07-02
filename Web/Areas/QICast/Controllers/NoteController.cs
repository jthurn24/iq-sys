using System;
using System.Web.Mvc;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Administration.Account;
using System.Collections;
using IQI.Intuition.Web.Attributes;
using System.Collections.Generic;
using RedArrow.Framework.Mvc.Security;
using IQI.Intuition.Web.Extensions;
using IQI.Intuition.Web.Models.QICast;
using IQI.Intuition.Reporting.Repositories;
using System.IO;
using System.Web.Mvc;
using System.Web;
using System.Drawing;

namespace IQI.Intuition.Web.Areas.QICast.Controllers
{
    public class NoteController : Controller
    {

        private IUserRepository _UserRepository;
        private IActionContext _Context;
        private IDimensionRepository _DimensionRepository;

        public NoteController(IUserRepository userRepository,
            IActionContext context,
            IDimensionRepository dimensionRepository)
        {
            _UserRepository = userRepository;
            _Context = context;
            _DimensionRepository = dimensionRepository;
        }

        public ActionResult DisplayImage(Guid id)
        {
            var note = _UserRepository.GetOrCreateNote(id, _Context.CurrentFacility.Guid);
            MemoryStream ms = new MemoryStream(note.Image);
            return File(ms, "image/jpeg");
        }

        [HttpGet]
        public ActionResult EditText(Guid id)
        {
            var note = _UserRepository.GetOrCreateNote(id, _Context.CurrentFacility.Guid);
            return View(new NoteEdit() { Id = id, Content = note.Content, Referrer = Request.UrlReferrer.AbsoluteUri });
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditText(Guid id, string content, string referrer)
        {
            var note = _UserRepository.GetOrCreateNote(id, _Context.CurrentFacility.Guid);
            note.Content = content;
            _UserRepository.Update(note);
            return Redirect(referrer);
        }

        [HttpGet]
        public ActionResult UploadImage(Guid id)
        {
            var note = _UserRepository.GetOrCreateNote(id, _Context.CurrentFacility.Guid);
            return View(new ImageUpload() { Id = id, Referrer = Request.UrlReferrer.AbsoluteUri });
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult UploadImage(ImageUpload model)
        {
            HttpPostedFileBase file = Request.Files["FileUpload"];
            if (file.ContentLength > 0)
            {
                var note = _UserRepository.GetOrCreateNote(model.Id, _Context.CurrentFacility.Guid);
                var i = System.Drawing.Image.FromStream(file.InputStream);


                double resizeFactor = 1;

                if (i.Width > 500|| i.Height > 900)
                {
                    double widthFactor = Convert.ToDouble(i.Width) / 500;
                    double heightFactor = Convert.ToDouble(i.Height) / 900;
                    resizeFactor = Math.Max(widthFactor, heightFactor);

                }
                int width = Convert.ToInt32(i.Width / resizeFactor);
                int height = Convert.ToInt32(i.Height / resizeFactor);
                Bitmap newImage = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(newImage);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(i, 0, 0, newImage.Width, newImage.Height);


                MemoryStream ms = new MemoryStream();
                newImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                note.Image = ms.ToArray();

                _UserRepository.Update(note);


            }

            return Redirect(model.Referrer);
        }


    }
}
