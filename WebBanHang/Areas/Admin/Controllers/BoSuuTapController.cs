using WebBanHang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebBanHang.Areas.Admin.Controllers
{
    public class BoSuuTapController : Controller
    {
        // GET: Admin/BoSuuTap
        dbShopQuanAoDataContext data = new dbShopQuanAoDataContext();
        // GET: BoSuuTap
        public ActionResult Index()
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return Redirect("~/Admin/Login");
            }
            var bosuutaps = data.BoSuuTaps.OrderBy(n => n.MaBST);
            return View(bosuutaps);
        }
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return Redirect("~/Admin/Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Create(BoSuuTap bosuutaps)
        {
            data.BoSuuTaps.InsertOnSubmit(bosuutaps);
            if (String.IsNullOrEmpty(bosuutaps.TenBST))
            {
                ViewBag.ThongBao = "Chưa nhập";
            }
            else
            {
                try
                {
                    data.SubmitChanges();
                    ViewBag.ThongBao = "SUCCESS";
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ViewBag.ThongBao = "Lỗi nhập size(vd: xxl... không lớn hơn 3 chữ cái)";
                }
            }
            return this.Create();

        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return Redirect("~/Admin/Login");
            }
            var bosuutaps = (from s in data.BoSuuTaps
                             where s.MaBST == id
                             select s).FirstOrDefault();
            if (bosuutaps == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(bosuutaps);
        }
        [HttpPost]
        public ActionResult Edit(int id, BoSuuTap bosuutap)
        {
            bosuutap = (from s in data.BoSuuTaps
                        where s.MaBST == id
                        select s).FirstOrDefault();
            UpdateModel(bosuutap);
            if (String.IsNullOrEmpty(bosuutap.TenBST))
            {
                ViewBag.ThongBao = "Chưa nhập";
            }
            else
            {
                try
                {
                    data.SubmitChanges();
                    ViewBag.ThongBao = "SUCCESS";
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ViewBag.ThongBao = "Lỗi nhập size(vd: xxl... không lớn hơn 3 chữ cái)";
                }
            }
            return View();
        }
        [HttpDelete]
        public ActionResult Delete(int id, BoSuuTap bosuutaps)
        {
            bosuutaps = (from s in data.BoSuuTaps
                         where s.MaBST == id
                         select s).FirstOrDefault();
            SanPham sanpham = (from s in data.SanPhams where s.MaBST == id select s).FirstOrDefault();
            if (sanpham != null)
            {
                return HttpNotFound();
            }
            else
            {
                try
                {
                    data.BoSuuTaps.DeleteOnSubmit(bosuutaps);
                    data.SubmitChanges();
                    return RedirectToAction("Index");

                }
                catch
                {
                    return View();
                }
            }
        }
    }
}