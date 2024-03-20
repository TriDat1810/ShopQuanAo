using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanHang.Models;
using System.Web.Mvc;
using System.Data.SqlClient;

namespace WebBanHang.Areas.Admin.Models
{    
    public class DeleteViewModel
    {
        dbShopQuanAoDataContext data = new dbShopQuanAoDataContext();
        public void DeleteSize(int id)
        {
            var sanphamsizes = data.SanPhams
            .Join(data.SanPhamSizes,
              post => post.MaSP,
              metal => metal.MaSP,
             (post, metal) => new { SanPham = post, SanPhamSize = metal })
           .Where(postAndMeta => postAndMeta.SanPham.MaSP == id);
            foreach (var item in sanphamsizes)
            {
                data.SanPhamSizes.DeleteOnSubmit(item.SanPhamSize);
            }
            data.SubmitChanges();
        }
        public void DeleteHinhAnh(int id)
        {
            var hinhanhs = data.SanPhams
            .Join(data.HinhAnhs,
              post => post.MaSP,
              metal => metal.MaSP,
             (post, metal) => new { SanPham = post, HinhAnh = metal })
           .Where(postAndMeta => postAndMeta.SanPham.MaSP == id);
            foreach (var item in hinhanhs)
            {
                data.HinhAnhs.DeleteOnSubmit(item.HinhAnh);
            }
            data.SubmitChanges();
        }
        public void DeleteBinhLuan(int id)
        {
            var binhluans = data.SanPhams
            .Join(data.BinhLuans,
              post => post.MaSP,
              metal => metal.MaSP,
             (post, metal) => new { SanPham = post, BinhLuan = metal })
           .Where(postAndMeta => postAndMeta.SanPham.MaSP == id);
            foreach (var item in binhluans)
            {
                data.BinhLuans.DeleteOnSubmit(item.BinhLuan);
            }
            data.SubmitChanges();
        }

        public void DeleteChiTietHoaDon(int id)
        {
            var chitiethoadons = data.SanPhams
            .Join(data.ChiTietHoaDons,
              post => post.MaSP,
              metal => metal.MaSP,
             (post, metal) => new { SanPham = post, ChiTietHoaDon = metal })
           .Where(postAndMeta => postAndMeta.SanPham.MaSP == id);
            foreach (var item in chitiethoadons)
            {
                data.ChiTietHoaDons.DeleteOnSubmit(item.ChiTietHoaDon);
            }
            data.SubmitChanges();
        }
        public void DeleteSanPham(int id)
        {
            var sanpham = (from s in data.SanPhams
                          where s.MaSP == id
                          select s).FirstOrDefault();
            data.SanPhams.DeleteOnSubmit(sanpham);
            data.SubmitChanges();
        }
    }
}