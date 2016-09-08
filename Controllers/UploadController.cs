using FileSite.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FileSite.Controllers
{
    public class UploadController : Controller
    {
        // GET: Upload
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Save(HttpPostedFileBase file)
        {
            var fileService = new FileService();
            var name = Path.GetFileName(file.FileName);
            var uri = fileService.Upload(name, file.InputStream);

            return View("Success", uri);
        }
    }
}