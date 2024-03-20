using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class ThanhToanOnlineController : Controller
    {
        // GET: ThanhToanOnline
        dbShopQuanAoDataContext data = new dbShopQuanAoDataContext();
        public List<GioHangViewModels> layGioHang()
        {
            var lstGiohang = Session["GioHang"] as List<GioHangViewModels>;
            if (lstGiohang == null)
            {
                lstGiohang = new List<GioHangViewModels>();
                Session["GioHang"] = lstGiohang;
            }
            return lstGiohang;
        }
        private int TongSoLuong()
        {
            int sum = 0;
            var lstGioHang = Session["GioHang"] as List<GioHangViewModels>;
            if (lstGioHang != null)
            {
                sum += lstGioHang.Count;
            }
            return sum;
        }

        private double TongTien()
        {
            double sum = 0;
            var lstGioHang = Session["GioHang"] as List<GioHangViewModels>;
            if (lstGioHang != null)
            {
                sum += lstGioHang.Sum(p => p.ThanhTien);
            }
            return sum;
        }

        public ActionResult PayOnl()
        {
            if (Session["taikhoan"] == null)
            {
                return RedirectToAction("dangnhap", "NguoiDung");
            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public ActionResult PaymentWithPaypal()
        {
            APIContext apiContext = PaypalConfiguration.getAPIContext();
            try
            {

                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {

                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/ThanhToanOnline/PaymentWithPaypal?";

                    var guid = Convert.ToString((new Random()).Next(100000));

                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);

                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = lnk.href;
                        }

                    }
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {

                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

                    if (executedPayment.state.ToLower() != "approved")
                    {
                        throw new Exception();
                    }
                    else
                    {
                        HoaDon hd = new HoaDon();
                        KhachHang kh = (KhachHang)Session["Taikhoan"];
                        List<GioHangViewModels> gh = layGioHang();
                        hd.MaKH = kh.MaKH;
                        hd.NgayLapHD = DateTime.Now;
                        hd.DiaChiGiaoHang = kh.DiaChi;
                        hd.DaThanhToan = true;
                        data.HoaDons.InsertOnSubmit(hd);
                        data.SubmitChanges();

                        foreach (var item in gh)
                        {
                            ChiTietHoaDon ctBill = new ChiTietHoaDon();

                            ctBill.MaHD = hd.MaHD;
                            ctBill.MaSP = item.MaSP;
                            ctBill.MaS = item.MaS;
                            ctBill.TenSP = item.TenSP;
                            ctBill.DonGia = (int?)double.Parse(item.DonGia.ToString());
                            ctBill.SoLuong = item.SoLuong;
                            data.ChiTietHoaDons.InsertOnSubmit(ctBill);

                        }
                        GuiEmailThongTinHoaDon(kh.Email, hd, gh);
                        data.SubmitChanges();
                        Session["GioHang"] = null;
                    }

                }
            }
            catch (Exception ex)
            {
                ViewBag.Loi = ex.Message; ViewBag.Error = ex.StackTrace;
                return View("DatThatBai");
            }
           
            return RedirectToAction("DatThanhCong");
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

        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            //create itemlist and add item objects to it  
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };

            //Adding Item Details like name, currency, price etc
            if (Session["GioHang"] != null)
            {
                List<GioHangViewModels> cart = (List<GioHangViewModels>)Session["GioHang"];

                foreach (var item in cart)
                {
                    // calculate price based on quantity
                    decimal itemPrice = decimal.Parse(item.ThanhTien.ToString()) / item.SoLuong;

                    itemList.items.Add(new Item()
                    {
                        name = "Thanh toan",
                        currency = "USD",
                        quantity = "1",
                        price = itemPrice.ToString(),
                        sku = "sku"
                    });
                }

                var payer = new Payer()
                {
                    payment_method = "paypal"
                };

                // Configure Redirect Urls here with RedirectUrls object  
                var redirUrls = new RedirectUrls()
                {
                    cancel_url = redirectUrl + "&Cancel=true",
                    return_url = redirectUrl
                };

                // Adding Tax, shipping and Subtotal details  
                var subtotal = TongTien().ToString("0.00");
                var details = new Details()
                {
                    tax = "1.00",
                    shipping = "2.00",
                    subtotal = subtotal,
                };

                //Final amount with details  
                var amount = new Amount()
                {
                    currency = "USD",
                    total = (decimal.Parse(details.tax) + decimal.Parse(details.shipping) + decimal.Parse(details.subtotal)).ToString("0.00"),
                    details = details
                };

                var transactionList = new List<Transaction>();
                // Adding description about the transaction  
                transactionList.Add(new Transaction()
                {
                    description = "Transaction description",
                    invoice_number = Convert.ToString((new Random()).Next(100000)),
                    amount = amount,
                    item_list = itemList
                });

                this.payment = new Payment()
                {
                    intent = "sale",
                    payer = payer,
                    transactions = transactionList,
                    redirect_urls = redirUrls
                };
            }

            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }



        public ActionResult DatThanhCong()
        {
            return View();
        }

        public ActionResult DatThatBai()
        {
            return View();
        }
    }
}