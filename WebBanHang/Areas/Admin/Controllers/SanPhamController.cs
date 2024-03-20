using WebBanHang.Areas.Admin.Models;
using WebBanHang.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebBanHang.Areas.Admin.Controllers
{
    public class SanPhamController : Controller
    {
        // GET: Admin/SanPham
        dbShopQuanAoDataContext data = new dbShopQuanAoDataContext();

        public ActionResult Index()
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return Redirect("~/Admin/Login");
            }
            return View(data.SanPhams.ToList().OrderBy(n => n.MaSP));
        }
        public ActionResult Details(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return Redirect("~/Admin/Login");
            }
            SanPham sanpham = data.SanPhams.SingleOrDefault(n => n.MaSP == id);
            return View(sanpham);
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return Redirect("~/Admin/Login");
            }
            // Khoi tao view Model
            // Lay size 

            //Lay ds tu tabke LoạiSảnPhẩm, sắp xep tang dan trheo ten Loại, chon lay gia tri MaL, hien thi thi TenLoai
            ViewBag.MaL = new SelectList(data.LoaiSanPhams.ToList().OrderBy(n => n.TenLoai), "MaL", "TenLoai");
            ViewBag.MaBST = new SelectList(data.BoSuuTaps.ToList().OrderBy(n => n.TenBST), "MaBST", "TenBST");
            var viewModel = new ThemSanPhamViewModel();

            return View(viewModel);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(ThemSanPhamViewModel viewModel, HttpPostedFileBase fileUpload, HttpPostedFileBase[] files)
        {

            //Dua du lieu vao dropdownload
            ViewBag.MaL = new SelectList(data.LoaiSanPhams.ToList().OrderBy(n => n.TenLoai), "MaL", "TenLoai");
            ViewBag.MaBST = new SelectList(data.BoSuuTaps.ToList().OrderBy(n => n.TenBST), "MaBST", "TenBST");
            // Tao san pham moi lay thong tin tu san pham user da nhap
            var sanpham = new SanPham
            {
                TenSP = viewModel.TenSP,
                GiaKhuyenMai = viewModel.GiaKhuyenMai,
                GiaBan = viewModel.GiaBan,
                GioiTinh = viewModel.GioiTinh,
                MaL = viewModel.MaL,
                MaBST = viewModel.MaBST,
                SoLuong = viewModel.SoLuong,
                ThongTin = viewModel.ThongTin,
                ngayNhapHang = viewModel.ngayNhapHang,
                AnhBia = viewModel.AnhBia
            };
            //Kiem tra duong dan file

            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Thông tin có vấn đề";
            }
            //Them vao CSDL
            else
            {
                //if (ModelState.IsValid)
                {
                    //Luu ten fie, luu y bo sung thu vien using System.IO;
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    //Luu duong dan cua file
                    var path = Path.Combine(Server.MapPath("~/images/anhbia"), fileName);
                    //Kiem tra hình anh ton tai chua?
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Thông tin có vấn đề";
                    }
                    else
                    {
                        //Luu hinh anh vao duong dan
                        fileUpload.SaveAs(path);
                    }
                    //Luu vao CSDL  
                    sanpham.AnhBia = "images/anhbia/" + fileName;
                    data.SanPhams.InsertOnSubmit(sanpham);
                    data.SubmitChanges();
                }
            }
            //Them HinhAnh1
            if (ModelState.IsValid)
            {   //iterating through multiple file collection   
                foreach (HttpPostedFileBase file in files)
                {
                    if (file != null)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/images/hinhanh"), fileName);
                        file.SaveAs(path);
                        data.HinhAnhs.InsertOnSubmit(new HinhAnh { MaSP = sanpham.MaSP, TenHinhAnh = "images/hinhanh/" + fileName });
                        ViewBag.UploadStatus = files.Count().ToString() + " files uploaded successfully.";
                        data.SubmitChanges();
                    }
                    else if (file == null)
                    {
                        ViewData["hinhanh"] = "Phải chọn hình ảnh";
                    }
                }
            }
            //Kiểm tra đã select chưa
            if (viewModel.SelectedSizes[0] == 0)
            {
                ViewBag.Thongbao = "Thông tin có vấn đề";
            }
            // Tien hanh them thong tin vao bang SanPhamSize
            else
            {
                for (int i = 0; i < viewModel.SelectedSizes.Count(); i++)
                {
                    data.SanPhamSizes.InsertOnSubmit(new SanPhamSize { MaSP = sanpham.MaSP, MaS = viewModel.SelectedSizes[i] });
                    data.SubmitChanges();
                }
            }
            ViewBag.ThongBao = "Thành công";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return Redirect("~/Admin/Login");
            }
            SanPham sanpham = data.SanPhams.SingleOrDefault(n => n.MaSP == id);
            // Lay size 
            //Lay ds tu tabke LoạiSảnPhẩm, sắp xep tang dan trheo ten Loại, chon lay gia tri MaL, hien thi thi TenLoai
            ViewBag.MaL = new SelectList(data.LoaiSanPhams.ToList().OrderBy(n => n.TenLoai), "MaL", "TenLoai");
            ViewBag.MaBST = new SelectList(data.BoSuuTaps.ToList().OrderBy(n => n.TenBST), "MaBST", "TenBST");

            var viewModel = new ThemSanPhamViewModel
            {
                MaSP = sanpham.MaSP,
                TenSP = sanpham.TenSP,
                GiaKhuyenMai = (int)sanpham.GiaKhuyenMai,
                GiaBan = (int)sanpham.GiaBan,
                GioiTinh = sanpham.GioiTinh,
                MaL = sanpham.MaL,
                MaBST = sanpham.MaBST,
                SoLuong = sanpham.SoLuong,
                ThongTin = sanpham.ThongTin,
                ngayNhapHang = sanpham.ngayNhapHang,

            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(int id, ThemSanPhamViewModel viewModel)
        {
            SanPham sanpham = data.SanPhams.SingleOrDefault(n => n.MaSP == id);
            //Dua du lieu vao dropdownload
            ViewBag.MaL = new SelectList(data.LoaiSanPhams.ToList().OrderBy(n => n.TenLoai), "MaL", "TenLoai");
            ViewBag.MaBST = new SelectList(data.BoSuuTaps.ToList().OrderBy(n => n.TenBST), "MaBST", "TenBST");
            // Tao san pham moi lay thong tin tu san pham user da nhap
            viewModel = new ThemSanPhamViewModel
            {
                MaSP = sanpham.MaSP,
                TenSP = sanpham.TenSP,
                GiaKhuyenMai = (int)sanpham.GiaKhuyenMai,
                GiaBan = (int)sanpham.GiaBan,
                GioiTinh = sanpham.GioiTinh,
                MaL = sanpham.MaL,
                MaBST = sanpham.MaBST,
                SoLuong = sanpham.SoLuong,
                ThongTin = sanpham.ThongTin,
                ngayNhapHang = sanpham.ngayNhapHang,
            };
            ViewBag.ThongBao = "Thành công";
            TryUpdateModel(sanpham);
            data.SubmitChanges();
            return this.Create();
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            DeleteViewModel deletes = new DeleteViewModel();
            deletes.DeleteSize(id);
            deletes.DeleteHinhAnh(id);
            deletes.DeleteBinhLuan(id);
            deletes.DeleteSanPham(id);
            return View();
        }
        }
}