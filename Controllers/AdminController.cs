using FileSite.Services;
using Microsoft.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FileSite.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            EnsureAuthenticated();
            var fileService = new FileService();
            return View(fileService.ListFiles());
        }

        [HttpPost]
        public ActionResult Delete(string file)
        {
            EnsureAuthenticated();
            var fileService = new FileService();
            fileService.Delete(file);
            return RedirectToAction("Index");
        }

        private void EnsureAuthenticated()
        {
            if (TempData.ContainsKey("hasLoggedIn"))
                return;
            
            // See if they've supplied credentials
            string authHeader = Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic"))
            {
                // Parse username and password out of the HTTP headers
                authHeader = authHeader.Substring("Basic".Length).Trim();
                byte[] authHeaderBytes = Convert.FromBase64String(authHeader);
                authHeader = Encoding.UTF7.GetString(authHeaderBytes);
                string pass = authHeader.Split(':')[1];

                if (pass == CloudConfigurationManager.GetSetting("AdminPassword"))
                {
                    TempData["hasLoggedIn"] = false;
                    return;
                }
            }
            
            Response.ClearContent();
            Response.StatusCode = 401;
            Response.AddHeader("WWW-Authenticate", "Basic");
            Response.End();
        }
    }
}