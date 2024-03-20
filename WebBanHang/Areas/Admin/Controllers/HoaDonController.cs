using WebBanHang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebBanHang.Areas.Admin.Controllers
{
    public class HoaDonController : Controller
    {
        // GET: Admin/HoaDon
        dbShopQuanAoDataContext data = new dbShopQuanAoDataContext();
        // GET: HoaDon
        public ActionResult Index()
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return Redirect("~/Admin/Login");
            }
            var hoadons = data.HoaDons.OrderBy(h => h.MaHD);
            return View(hoadons);
        }
        public ActionResult Details(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return Redirect("~/Admin/Login");
            }

            var hoadon = (from s in data.HoaDons
                          where s.MaHD == id
                          select s).FirstOrDefault();
            var list = data.ChiTietHoaDons.Where(s => s.MaHD == hoadon.MaHD).ToList();
            var khachhang = (from s in data.KhachHangs
                             where s.MaKH == hoadon.MaKH
                             select s).FirstOrDefault();
            ViewBag.name = khachhang.HoTen;
            ViewBag.address = khachhang.DiaChi;
            ViewBag.phone = khachhang.SoDienThoai;
            return View(list);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return Redirect("~/Admin/Login");
            }
            var hoadon = (from s in data.HoaDons
                          where s.MaHD == id
                          select s).FirstOrDefault();
            return View(hoadon);
        }
        [HttpPost]
        public ActionResult Edit(int id, HoaDon hoadon)
        {
            hoadon = (from s in data.HoaDons
                      where s.MaHD == id
                      select s).FirstOrDefault();
            UpdateModel(hoadon);
            data.SubmitChanges();
            ViewBag.ThongBao = "SUCCESS";
            return View();
        }
        [HttpDelete]
        public ActionResult Delete(int id, HoaDon hoadons)
        {
            hoadons = (from s in data.HoaDons
                       where s.MaHD == id
                       select s).FirstOrDefault();
            ChiTietHoaDon chitiethoadon = (from s in data.ChiTietHoaDons where s.MaHD == hoadons.MaHD select s).FirstOrDefault();
            if (hoadons == null)
            {
                return HttpNotFound();
            }
            else
            {
                try
                {
                    if (chitiethoadon != null)
                    {

                        var list = data.ChiTietHoaDons.Where(s => s.MaHD == hoadons.MaHD).ToList();
                        data.ChiTietHoaDons.DeleteAllOnSubmit(list);
                        data.SubmitChanges();
                    }
                    data.HoaDons.DeleteOnSubmit(hoadons);
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