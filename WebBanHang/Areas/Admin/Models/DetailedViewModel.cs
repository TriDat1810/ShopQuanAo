using WebBanHang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopQuanAo.Areas.Admin.Models
{
    public class DetailedViewModel
    {
        public SanPham SanPham { get; set; }
        public IEnumerable<SanPham> SanPhamCungloai { get; set; }
        public IEnumerable<BinhLuan> BinhLuan { get; set; }
        public IEnumerable<HinhAnh> HinhAnh { get; set; }
        public IEnumerable<Size> Size { get; set; }
        public IEnumerable<SanPhamSize> SanPhamSize { get; set; }
        public BinhLuan BinhLuans { get; set; }

    }
}