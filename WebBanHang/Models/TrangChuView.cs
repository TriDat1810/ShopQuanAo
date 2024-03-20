using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanHang.Models
{
    public class TrangChuView
    {
        public List<SanPham> LayMoi { get; set; }
        // public IEnumerable<SanPham> LayMoi { get; set;  }
        // Tinh da hinh trong OOP
        public List<SanPham> LayHet { get; set; }
    }
}