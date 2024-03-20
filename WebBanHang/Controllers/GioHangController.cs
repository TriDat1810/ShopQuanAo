using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class GioHangController : Controller
    {
        dbShopQuanAoDataContext data = new dbShopQuanAoDataContext();
        // GET: GioHang
        public ActionResult Index()
        {
            return View();
        }
        public List<GioHangViewModels> LayGioHang()
        {
            List<GioHangViewModels> lstGioHang = Session["GioHang"] as List<GioHangViewModels>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<GioHangViewModels>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }

        public ActionResult ThemGioHang(int masp, int mas, string strURL)
        {

            //Lay ra Session gio hang
            List<GioHangViewModels> lstGiohang = LayGioHang();
            //Kiem tra sách này tồn tại trong Session["Giohang"] chưa?
            GioHangViewModels sanpham = lstGiohang.Find(n => n.MaSP == masp && n.MaS == mas);

            if (sanpham == null)
            {
                sanpham = new GioHangViewModels(masp, mas);
                lstGiohang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.SoLuong++;
                return Redirect(strURL);
            }
        }
        //Tong so luong
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHangViewModels> lstGiohang = Session["GioHang"] as List<GioHangViewModels>;
            if (lstGiohang != null)
            {
                iTongSoLuong = lstGiohang.Sum(n => n.SoLuong);
            }
            return iTongSoLuong;
        }
        //Tinh tong tien
        private double TongTien()
        {
            double iTongTien = 0;
            List<GioHangViewModels> lstGiohang = Session["GioHang"] as List<GioHangViewModels>;
            if (lstGiohang != null)
            {
                iTongTien = lstGiohang.Sum(n => n.ThanhTien);
            }
            return iTongTien;
        }

        //Xay dung trang Gio hang
        public ActionResult GioHang()
        {
            List<GioHangViewModels> lstGiohang = LayGioHang();
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "ShopQuanAo");
            }
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(lstGiohang);
        }
        //Tao Partial view de hien thi thong tin gio hang
        public ActionResult GiohangPartial()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return PartialView();
        }
        //Cap nhat Giỏ hàng
        public ActionResult CapnhatGiohang(int masp, FormCollection f)
        {

            //Lay gio hang tu Session
            List<GioHangViewModels> lstGiohang = LayGioHang();
            //Kiem tra sach da co trong Session["Giohang"]
            GioHangViewModels sanpham = lstGiohang.SingleOrDefault(n => n.MaSP == masp);
            //Neu ton tai thi cho sua Soluong
            if (sanpham != null)
            {
                sanpham.SoLuong = int.Parse(f["txtSoluong"].ToString());
            }
            return RedirectToAction("Giohang");
        }
        //Xoa Giohang
        public ActionResult XoaGiohang(int masp)
        {
            //Lay gio hang tu Session
            List<GioHangViewModels> lstGiohang = LayGioHang();
            //Kiem tra sach da co trong Session["Giohang"]
            GioHangViewModels sanpham = lstGiohang.SingleOrDefault(n => n.MaSP == masp);
            //Neu ton tai thi cho sua Soluong
            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.MaSP == masp);
                return RedirectToAction("GioHang");

            }
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "ShopQuanAo");
            }
            return RedirectToAction("GioHang");
        }
        //Xoa tat ca thong tin trong Gio hang
        public ActionResult XoaTatcaGiohang()
        {
            //Lay gio hang tu Session
            List<GioHangViewModels> lstGiohang = LayGioHang();
            lstGiohang.Clear();
            return RedirectToAction("Index", "ShopQuanAo");
        }


        [HttpGet]
        public ActionResult DatHang()
        {
            //Kiem tra dang nhap
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }
            if (Session["Giohang"] == null)
            {
                return RedirectToAction("Index", "ShopQuanAo");
            }

            //Lay gio hang tu Session
            List<GioHangViewModels> lstGiohang = LayGioHang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();

            return View(lstGiohang);
        }

        //Xay dung chuc nang Dathang
        [HttpPost]
        public ActionResult DatHang(FormCollection collection)
        {
            //Them Don hang
            HoaDon ddh = new HoaDon();
            KhachHang kh = (KhachHang)Session["Taikhoan"];
            List<GioHangViewModels> gh = LayGioHang();
            ddh.MaKH = kh.MaKH;
            ddh.DiaChiGiaoHang = kh.DiaChi;
            ddh.NgayLapHD = DateTime.Now;
            ddh.TrangThai = false;
            ddh.DaThanhToan = false;
            ddh.GhiChu = collection["GhiChu"];
            data.HoaDons.InsertOnSubmit(ddh);
            data.SubmitChanges();
            //Them chi tiet don hang            
            foreach (var item in gh)
            {
                ChiTietHoaDon ctdh = new ChiTietHoaDon();
                ctdh.MaHD = ddh.MaHD;
                ctdh.MaSP = item.MaSP;
                ctdh.TenSP = (item.MaSP + " - " + item.TenSP);
                ctdh.MaS = item.MaS;
                ctdh.SoLuong = item.SoLuong;
                ctdh.DonGia = (int)item.DonGia;
                data.ChiTietHoaDons.InsertOnSubmit(ctdh);
            }
            GuiEmailThongTinHoaDon(kh.Email, ddh, gh);
            data.SubmitChanges();
            Session["Giohang"] = null;
            return RedirectToAction("Xacnhandonhang", "Giohang");
        }

        private void GuiEmailThongTinHoaDon(string emailAddress, HoaDon hoaDon, List<GioHangViewModels> gioHang)
        {
            string subject = "Xác nhận đơn hàng từ cửa hàng của bạn";
            string body = $"Cảm ơn bạn đã đặt hàng! Dưới đây là thông tin đơn hàng của bạn:\n\n";

            // Thêm thông tin hóa đơn
            body += $"Mã đơn hàng: {hoaDon.MaHD}\n";
            body += $"Ngày đặt hàng: {hoaDon.NgayLapHD}\n";
            // Thêm thông tin chi tiết hóa đơn
            body += "\nChi tiết đơn hàng:\n";
            foreach (var item in gioHang)
            {
                body += $"{item.TenSP} - Số lượng: {item.SoLuong} - Đơn giá: {item.DonGia}\n";
            }

            // Thêm thông tin khách hàng
            body += $"\nThông tin khách hàng:\n";
            body += $"Tên khách hàng: {hoaDon.KhachHang.HoTen}\n";
            body += $"Địa chỉ giao hàng: {hoaDon.DiaChiGiaoHang}\n";
            body += $"Ghi chú: {hoaDon.GhiChu}\n";

            // Gửi email
            GuiEmail(emailAddress, subject, body);
        }

        private void GuiEmail(string to, string subject, string body)
        {
            string smtpServer = "smtp.gmail.com"; // Có thể cần thay đổi tùy vào nhà cung cấp email của bạn
            string smtpUsername = "kt78139@gmail.com"; // Thay bằng địa chỉ email của bạn
            string smtpPassword = "pbnf yvii lodt iwyu"; // Thay bằng mật khẩu email của bạn
            int smtpPort = 587; // Có thể cần thay đổi tùy vào nhà cung cấp email của bạn

            using (SmtpClient client = new SmtpClient(smtpServer, smtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                MailAddress fromAddress = new MailAddress(smtpUsername, "Shop");

                MailMessage mailMessage = new MailMessage(smtpUsername, to, subject, body);
                mailMessage.IsBodyHtml = false;

                client.Send(mailMessage);
            }
        }


        public ActionResult Xacnhandonhang()
        {
            return View();
        }
    }
}