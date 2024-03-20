using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHang.Models;

namespace WebBanHang.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        dbShopQuanAoDataContext data = new dbShopQuanAoDataContext();
        // GET: Admin/Login
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            // Gán các giá trị người dùng nhập liệu cho các biến 
            var tendn = collection["TenDangNhap"];
            var matkhau = collection["MatKhau"];
            //Gán giá trị cho đối tượng được tạo mới (ad)                      
            WebBanHang.Models.Admin admin = data.Admins.SingleOrDefault(n => n.TaiKhoanAdmin == tendn && n.MatKhau == matkhau);
            if (admin != null)
            {
                // ViewBag.Thongbao = "Chúc mừng đăng nhập thành công";
                Session["Taikhoanadmin"] = admin;
                return RedirectToAction("Index", "QuanTriVien");
            }
            else
                ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";

            return View();
        }
        public ActionResult Logout()
        {
            Session["Taikhoanadmin"] = null;
            return RedirectToAction("Index");
        }
    }
}