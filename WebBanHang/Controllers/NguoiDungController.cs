using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class NguoiDungController : Controller
    {
        dbShopQuanAoDataContext data = new dbShopQuanAoDataContext();
        // GET: NguoiDung
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Profile(KhachHang khachhang)
        {
            return View(khachhang);
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("index", "ShopQuanAo");
        }
        [HttpGet]
        private void SendVerificationEmail(string email, string activationCode)
        {
            var fromAddress = new MailAddress("kt78139@gmail.com", "Shop");
            var toAddress = new MailAddress(email, "Bạn");
            var subject = "Xác thực Email";
            var body = $"Xin chào,\n\nHãy nhấp vào liên kết sau để xác thực email của bạn:\n\n {Url.Action("ConfirmEmail", "NguoiDung", new { email = email, code = activationCode }, Request.Url.Scheme)}";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, "pbnf yvii lodt iwyu")
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }


        [HttpGet]
        public PartialViewResult DangKy()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult Dangky(FormCollection collection, KhachHang kh)
        {
            // Gán các giá tị người dùng nhập liệu cho các biến 
            var tendangnhap = collection["TenDangnhap"];
            var matkhau = collection["MatKhau"];
            var nhaplaimatkhau = collection["NhapLaiMatkhau"];
            var hoten = collection["HoTen"];
            var diachi = collection["Diachi"];
            var email = collection["Email"];
            var sodienthoai = collection["SoDienthoai"];
            if (String.IsNullOrEmpty(hoten))
            {
                ViewData["hoten"] = "Họ tên khách hàng không được để trống";
            }
            else if (String.IsNullOrEmpty(tendangnhap))
            {
                ViewData["tendangnhap"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["matkhau"] = "Phải nhập mật khẩu";
            }
            else if (String.IsNullOrEmpty(nhaplaimatkhau))
            {
                ViewData["nhaplaimatkhau"] = "Phải nhập lại mật khẩu";
            }

            if (String.IsNullOrEmpty(email))
            {
                ViewData["email"] = "Email không được bỏ trống";
            }

            if (String.IsNullOrEmpty(sodienthoai))
            {
                ViewData["sodienthoai"] = "Phải nhập điện thoai";
            }
            else
            {
                // Tạo mã xác thực
                kh.ActivationCode = Guid.NewGuid();

                // Gán giá trị cho đối tượng được tạo mới (kh)
                kh.TenDangNhap = tendangnhap;
                kh.MatKhau = matkhau;
                kh.HoTen = hoten;
                kh.DiaChi = diachi;
                kh.Email = email;
                kh.SoDienThoai = sodienthoai;
                kh.TrangThai = false;

                // Lưu vào cơ sở dữ liệu
                data.KhachHangs.InsertOnSubmit(kh);
                data.SubmitChanges();

                // Gửi email xác thực
                SendVerificationEmail(kh.Email, kh.ActivationCode.ToString());

                // Chuyển đến trang xác thực email
                return RedirectToAction("DangNhap");
            }

            return this.DangKy();
        }

        [HttpGet]
        public ActionResult ConfirmEmail(string email, string code)
        {
            // Kiểm tra email và code trong cơ sở dữ liệu
            var khachHang = data.KhachHangs.SingleOrDefault(kh => kh.Email == email && kh.ActivationCode.ToString() == code);

            if (khachHang != null)
            {
                // Cập nhật trạng thái xác thực
                khachHang.TrangThai = true;
                data.SubmitChanges();

                ViewBag.Message = "Xác thực email thành công! Bây giờ bạn có thể đăng nhập.";
            }
            else
            {
                ViewBag.Message = "Xác thực email không thành công.";
            }

            return View();
        }


        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Dangnhap(FormCollection collection)
        {
            // Gán các giá trị người dùng nhập liệu cho các biến 
            var tendangnhap = collection["tendangnhap"];
            var matkhau = collection["Matkhau"];
            if (String.IsNullOrEmpty(tendangnhap))
            {
                ViewData["tendangnhap"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["matkhau"] = "Phải nhập mật khẩu";
            }
            else
            {
                //Gán giá trị cho đối tượng được tạo mới (kh)
                KhachHang kh = data.KhachHangs.SingleOrDefault(n => n.TenDangNhap == tendangnhap && n.MatKhau == matkhau && n.TrangThai == true);
                if (kh != null)
                {
                    ViewBag.Thongbao = "Chúc mừng đăng nhập thành công";
                    Session["Taikhoan"] = kh;
                    return RedirectToAction("Index", "ShopQuanAo");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";

            }
            return RedirectToAction("DangNhap");
        }
        public ActionResult Edit(int id)
        {
            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (Session["Taikhoan"] == null)
            {
                // Lưu URL của trang chỉnh sửa vào session để sau khi đăng nhập thành công, người dùng có thể quay trở lại trang chỉnh sửa
                Session["ReturnUrl"] = Url.Action("Edit", "NguoiDung", new { id = id });
                return RedirectToAction("DangNhap"); // Chuyển hướng đến trang đăng nhập nếu chưa đăng nhập
            }

            // Lấy thông tin khách hàng cần chỉnh sửa từ cơ sở dữ liệu
            KhachHang khachhang = data.KhachHangs.SingleOrDefault(n => n.MaKH == id);

            // Kiểm tra xem khách hàng có tồn tại hay không
            if (khachhang == null)
            {
                return HttpNotFound(); // Trả về lỗi 404 nếu không tìm thấy khách hàng
            }

            return View(khachhang); // Trả về view để hiển thị form chỉnh sửa thông tin khách hàng
        }

        [HttpPost]
        public ActionResult Edit(KhachHang khachhang)
        {
            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (Session["Taikhoan"] == null)
            {
                return RedirectToAction("DangNhap"); // Chuyển hướng đến trang đăng nhập nếu chưa đăng nhập
            }

            // Kiểm tra tính hợp lệ của dữ liệu nhập vào
            if (ModelState.IsValid)
            {
                // Lưu các thay đổi vào cơ sở dữ liệu
                KhachHang khachhangUpdate = data.KhachHangs.SingleOrDefault(n => n.MaKH == khachhang.MaKH);
                if (khachhangUpdate != null)
                {
                    khachhangUpdate.HoTen = khachhang.HoTen;
                    khachhangUpdate.DiaChi = khachhang.DiaChi;
                    khachhangUpdate.Email = khachhang.Email;
                    khachhangUpdate.GioiTinh = khachhang.GioiTinh;
                    khachhangUpdate.SoDienThoai = khachhang.SoDienThoai;

                    data.SubmitChanges(); // Lưu các thay đổi vào cơ sở dữ liệu

                    ViewBag.Thongbao = "Cập nhật thông tin thành công";
                    Session["Taikhoan"] = khachhangUpdate;
                    // Chuyển hướng người dùng đến trang Profile hoặc trang chi tiết khách hàng để hiển thị thông tin đã cập nhật
                    return RedirectToAction("Profile", "NguoiDung");
                }
                else
                {
                    return HttpNotFound(); // Trả về lỗi 404 nếu không tìm thấy khách hàng
                }
            }

            return View(khachhang); // Trả về view để hiển thị form chỉnh sửa thông tin khách hàng (nếu có lỗi)
        }
        public ActionResult ChangePassword(int id)
        {
            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (Session["Taikhoan"] == null)
            {
                // Lưu URL của trang chỉnh sửa vào session để sau khi đăng nhập thành công, người dùng có thể quay trở lại trang chỉnh sửa
                Session["ReturnUrl"] = Url.Action("ChangePassword", "NguoiDung", new { id = id });
                return RedirectToAction("DangNhap"); // Chuyển hướng đến trang đăng nhập nếu chưa đăng nhập
            }

            // Lấy thông tin khách hàng cần chỉnh sửa từ cơ sở dữ liệu
            KhachHang khachhang = data.KhachHangs.SingleOrDefault(n => n.MaKH == id);

            // Kiểm tra xem khách hàng có tồn tại hay không
            if (khachhang == null)
            {
                return HttpNotFound(); // Trả về lỗi 404 nếu không tìm thấy khách hàng
            }

            return View(khachhang); // Trả về view để hiển thị form thay đổi mật khẩu
        }

        // POST: NguoiDung/ChangePassword
        [HttpPost]
        public ActionResult ChangePassword(KhachHang khachhang, string newPassword)
        {
            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (Session["Taikhoan"] == null)
            {
                return RedirectToAction("DangNhap"); // Chuyển hướng đến trang đăng nhập nếu chưa đăng nhập
            }

            // Kiểm tra tính hợp lệ của dữ liệu nhập vào
            if (ModelState.IsValid)
            {
                // Lưu các thay đổi vào cơ sở dữ liệu
                KhachHang khachhangUpdate = data.KhachHangs.SingleOrDefault(n => n.MaKH == khachhang.MaKH);
                if (khachhangUpdate != null)
                {
                    khachhangUpdate.MatKhau = newPassword;

                    data.SubmitChanges(); // Lưu các thay đổi vào cơ sở dữ liệu

                    ViewBag.Thongbao = "Cập nhật mật khẩu thành công";
                    Session["Taikhoan"] = khachhangUpdate;
                    // Chuyển hướng người dùng đến trang ChangePassword
                    return RedirectToAction("ChangePassword", new { id = khachhangUpdate.MaKH });
                }
                else
                {
                    return HttpNotFound(); // Trả về lỗi 404 nếu không tìm thấy khách hàng
                }
            }

            return View(khachhang); // Trả về view để hiển thị form thay đổi mật khẩu (nếu có lỗi)
        }
    }
}