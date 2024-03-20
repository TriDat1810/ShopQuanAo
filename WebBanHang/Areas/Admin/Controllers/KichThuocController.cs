using WebBanHang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebBanHang.Areas.Admin.Controllers
{
    public class KichThuocController : Controller
    {
        // GET: Admin/KichThuoc
        dbShopQuanAoDataContext data = new dbShopQuanAoDataContext();
        // GET: Size
        public ActionResult Index()
        {
            var sizes = data.Sizes.OrderBy(n => n.MaS);
            return View(sizes);
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
        public ActionResult Create(Size sizes)
        {

            data.Sizes.InsertOnSubmit(sizes);
            if (String.IsNullOrEmpty(sizes.TenS))
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
            var sizes = (from s in data.Sizes
                         where s.MaS == id
                         select s).FirstOrDefault();
            if (sizes == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sizes);
        }
        [HttpPost]
        public ActionResult Edit(int id, Size sizes)
        {
            sizes = (from s in data.Sizes
                     where s.MaS == id
                     select s).FirstOrDefault();
            UpdateModel(sizes);
            if (String.IsNullOrEmpty(sizes.TenS))
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
        public ActionResult Delete(int id, Size sizes)
        {
            sizes = (from s in data.Sizes
                     where s.MaS == id
                     select s).FirstOrDefault();
            SanPhamSize sanphamsizes = data.SanPhamSizes.SingleOrDefault(n => n.MaS == id);
            if (sanphamsizes != null)
            {
                data.SanPhamSizes.DeleteOnSubmit(sanphamsizes);
                data.SubmitChanges();
            }
            data.Sizes.DeleteOnSubmit(sizes);
            data.SubmitChanges();
            return RedirectToAction("Index");
        }
    }
}