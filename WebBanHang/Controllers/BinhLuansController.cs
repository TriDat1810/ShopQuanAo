using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class BinhLuansController : Controller
    {
        // GET: BinhLuan
        dbShopQuanAoDataContext data = new dbShopQuanAoDataContext();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Create(int id)
        {
            SanPham sanphams = data.SanPhams.SingleOrDefault(n => n.MaSP == id);
            // Giữ lại MaSP cho binhluans => HttpPost
            var binhluans = new BinhLuan();
            binhluans.MaSP = sanphams.MaSP;
            return View(binhluans);
        }
        [HttpPost]
        public ActionResult Create(BinhLuan binhluans)
        {

            var product = new dbShopQuanAoDataContext();
            KhachHang kh = (KhachHang)Session["Taikhoan"];
            binhluans.MaKH = kh.MaKH;
            DateTime date = DateTime.Now;
            binhluans.NgayBL = date;
            data.BinhLuans.InsertOnSubmit(binhluans);
            data.SubmitChanges();
            return RedirectToAction("Details", "ShopQuanAo", new { id = binhluans.MaSP });
        }
    }
}