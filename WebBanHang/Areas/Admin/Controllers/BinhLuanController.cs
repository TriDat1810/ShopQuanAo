using WebBanHang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebBanHang.Areas.Admin.Controllers
{
    public class BinhLuanController : Controller
    {
        // GET: Admin/BinhLuan
        dbShopQuanAoDataContext data = new dbShopQuanAoDataContext();
        // GET: Comment
        public ActionResult Index()
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return Redirect("~/Admin/Login");
            }
            var binhluans = data.BinhLuans.OrderBy(n => n.MaBL);
            return View(binhluans);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return Redirect("~/Admin/Login");
            }
            var binhluans = (from s in data.BinhLuans
                             where s.MaBL == id
                             select s).FirstOrDefault();
            if (binhluans == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(binhluans);
        }
        [HttpPost]
        public ActionResult Edit(int id, BinhLuan binhluans)
        {
            binhluans = (from s in data.BinhLuans where s.MaBL == id select s).FirstOrDefault();
            UpdateModel(binhluans);
            data.SubmitChanges();
            return View();
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return Redirect("~/Admin/Login");
            }
            var binhluans = (from s in data.BinhLuans
                             where s.MaBL == id
                             select s).FirstOrDefault();
            return View(binhluans);

        }
        [HttpDelete]
        public ActionResult Delete(int id, BinhLuan binhluans)
        {
            binhluans = (from s in data.BinhLuans
                         where s.MaBL == id
                         select s).FirstOrDefault();
            data.BinhLuans.DeleteOnSubmit(binhluans);
            data.SubmitChanges();
            return RedirectToAction("Index");
        }
    }
}