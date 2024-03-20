using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanHang.Models;
using System.ComponentModel.DataAnnotations;

namespace WebBanHang.Areas.Admin.Models
{
    public class ThemSanPhamViewModel
    {
        dbShopQuanAoDataContext data = new dbShopQuanAoDataContext();
        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public int GiaKhuyenMai { get; set; }
        public int GiaBan { get; set; }
        public int? MaL { get; set; }
        public int? MaBST { get; set; }
        public int? SoLuong { get; set; }
        public string ThongTin { get; set; }
        public DateTime? ngayNhapHang { get; set; }
        public string AnhBia { get; set; }
        public int? GioiTinh { get; set; }
        public List<Size> Sizes { get; set; }
        public int[] SelectedSizes { get; set; }
        public HinhAnh HinhAnhs { get; set; }

        [Required(ErrorMessage = "Please select file.")]
        [Display(Name = "Browse File")]
        public HttpPostedFileBase[] files { get; set; }
        public ThemSanPhamViewModel()
        {
            Sizes = new dbShopQuanAoDataContext().Sizes.ToList();
            SelectedSizes = new int[Sizes.Count];

        }
    }
    }