using WebBanHang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebBanHang.Areas.Admin.Controllers
{
    public class DanhMucController : Controller
    {
        // GET: Admin/DanhMuc
        dbShopQuanAoDataContext data = new dbShopQuanAoDataContext();
        // GET: Category
        public ActionResult Index()
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return Redirect("~/Admin/Login");
            }
            var loaisp = data.LoaiSanPhams.OrderBy(n => n.MaL);
            return View(loaisp);
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
        public ActionResult Create(LoaiSanPham loaisanphams)
        {

            data.LoaiSanPhams.InsertOnSubmit(loaisanphams);
            data.SubmitChanges();
            ViewBag.ThongBao = "Thêm danh mục thành công";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return Redirect("~/Admin/Login");
            }
            var loaisp = (from s in data.LoaiSanPhams
                          where s.MaL == id
                          select s).FirstOrDefault();
            if (loaisp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(loaisp);
        }
        [HttpPost]
        public ActionResult Edit(int id, LoaiSanPham loaisp)
        {
            loaisp = (from s in data.LoaiSanPhams
                      where s.MaL == id
                      select s).FirstOrDefault();
            UpdateModel(loaisp);
            data.SubmitChanges();
            ViewBag.ThongBao = "Cập nhật thành công";
            return View();
        }
        [HttpDelete]
        public ActionResult Delete(int id, LoaiSanPham loaisp)
        {
            loaisp = (from s in data.LoaiSanPhams
                      where s.MaL == id
                      select s).FirstOrDefault();
            SanPham sanpham = (from s in data.SanPhams where s.MaL == id select s).FirstOrDefault();
            if (sanpham != null)
            {
                return HttpNotFound();
            }
            else
            {
                try
                {
                    data.LoaiSanPhams.DeleteOnSubmit(loaisp);
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