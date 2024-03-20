using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanHang.Models
{
    public class GioHangViewModels
    {
        dbShopQuanAoDataContext data = new dbShopQuanAoDataContext();

        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public string AnhBia { get; set; }
        public int MaS { get; set; }
        public double DonGia { get; set; }
        public int SoLuong { get; set; }
        public string GhiChu { get; set; }
        public int GiaKhuyenMai { get; set; }
        public double GiaBan { get; set; }
        public List<SanPham> SanPhams { get; set; }
        public List<SanPhamSize> SanPhamSizes { get; set; }
        public string TenS { get; set; }

        public int? SizeId { get; set; }

        public Double ThanhTien
        {
            get { return SoLuong * DonGia; }
        }

        public GioHangViewModels(int id, int? sizeId)
        {
            MaSP = id;
            SanPham sanpham = data.SanPhams.FirstOrDefault(n => n.MaSP == id);

            SanPhamSize sanphamsizes = data.SanPhamSizes.FirstOrDefault(n => n.MaSP == id && n.MaS == sizeId);

            SizeId = sizeId;
            TenS = sanphamsizes?.Size.TenS;
            GiaKhuyenMai = (int)sanpham.GiaKhuyenMai;
            TenSP = sanpham.TenSP;
            AnhBia = sanpham.AnhBia;
            MaS = (int)sizeId;
            GiaBan = double.Parse(sanpham.GiaBan.ToString());
            if (sanpham.GiaKhuyenMai > 0)
            {
                DonGia = double.Parse(sanpham.GiaKhuyenMai.ToString());
            }
            else
            {
                DonGia = double.Parse(sanpham.GiaBan.ToString());
            }
            SoLuong = 1;
        }
    }
}