using WebBanHang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebBanHang.Areas.Admin.Controllers
{
    public class KhachHangController : Controller
    {
        // GET: Admin/KhachHang
        dbShopQuanAoDataContext data = new dbShopQuanAoDataContext();
        // GET: KhachHang
        public ActionResult Index()
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return Redirect("~/Admin/Login");
            }
            var khachhang = data.KhachHangs.OrderBy(n => n.MaKH);
            return View(khachhang);
        }
        public ActionResult Edit(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return Redirect("~/Admin/Login");
            }
            var khachhang = (from s in data.KhachHangs
                             where s.MaKH == id
                             select s).FirstOrDefault();
            if (khachhang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(khachhang);
        }
        [HttpPost]
        public ActionResult Edit(int id, KhachHang khachhang)
        {
            khachhang = (from s in data.KhachHangs where s.MaKH == id select s).FirstOrDefault();
            UpdateModel(khachhang);
            data.SubmitChanges();
            return View();
        }
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            KhachHang khachhang = (from s in data.KhachHangs
                                   where s.MaKH == id
                                   select s).FirstOrDefault();
            var listbl = data.BinhLuans.Where(s => s.MaKH == id).ToList();
            var listhd = data.HoaDons.Where(s => s.MaKH == id).ToList();
            try
            {
                foreach (var item in listhd.ToList())
                {
                    var chitiethoadon = data.ChiTietHoaDons.Where(s => s.MaHD == item.MaHD).ToList();
                    data.ChiTietHoaDons.DeleteAllOnSubmit(chitiethoadon);
                    data.SubmitChanges();
                }
                foreach (var item in listhd.ToList())
                {
                    data.HoaDons.DeleteAllOnSubmit(listhd);
                    data.SubmitChanges();
                }
                foreach (var item in listbl.ToList())
                {
                    data.BinhLuans.DeleteAllOnSubmit(listbl);
                    data.SubmitChanges();
                }
                data.KhachHangs.DeleteOnSubmit(khachhang);
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