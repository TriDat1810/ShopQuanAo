using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanHang.Models
{
    public class DetailedViewModels
    {
        public SanPham SanPham { get; set; }

        public KhachHang KhachHang { get; set; }
        public IEnumerable<SanPham> SanPhamCungloai { get; set; }
        public IEnumerable<BinhLuan> BinhLuan { get; set; }
        public IEnumerable<HinhAnh> HinhAnh { get; set; }
        public LoaiSanPham LoaiSanPham { get; set; }
        public IEnumerable<Size> Size { get; set; }
        public IEnumerable<SanPhamSize> SanPhamSize { get; set; }
        public BinhLuan BinhLuans { get; set; }
        public IEnumerable<Size> AvailableSizes { get; set; }
        public string SelectedSize { get; set; }
        public int SelectedSizeid { get; set; }
    }
}