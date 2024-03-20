using ShopQuanAo.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHang.Models;
using PagedList;
using PagedList.Mvc;
using System.Web.UI;

namespace WebBanHang.Controllers
{
    public class ShopQuanAoController : Controller
    {
        // GET: ShopQuanAo
        dbShopQuanAoDataContext data = new dbShopQuanAoDataContext();
        public List<SanPham> Laymoi(int count)
        {

            return data.SanPhams.OrderByDescending(a => a.ngayNhapHang).Take(count).ToList();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Partial_ItemsByCateId()
        {
            var items = data.SanPhams.Take(10).ToList();
            return View(items);
        }
        public ActionResult Partial_Product(string Searchtext, int? page, int? id)
        {
            var pageSize = 12;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<SanPham> items = data.SanPhams.OrderByDescending(x => x.MaSP); // lấy tất cả sản phẩm, sắp xếp theo ID giảm dần

            if (!string.IsNullOrEmpty(Searchtext))
            {
                items = items.Where(x => x.TenSP.Contains(Searchtext));
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;

            if (id != null)
            {
                items = items.Where((x) => x.LoaiSanPham.MaL == id).ToList();
            }
            return View(items);
        }
        public ActionResult Partial_ItemsSale()
        {
            var items = data.SanPhams.OrderByDescending(x => x.LuotMua).Take(10).ToList();
            return View(items);
        }
        public ActionResult Partial_ProductCategory()
        {
            var loaisp = from cd in data.LoaiSanPhams select cd;
            return PartialView(loaisp);
        }
        
        public PartialViewResult IndexLaymoi()
        {
            var quanaomoi = Laymoi(5);
            return PartialView(quanaomoi);
        }
        public ActionResult SPTheoLoai(string Searchtext, int id, int? page)
        {
            var pageSize = 8;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<SanPham> items = from s in data.SanPhams where s.MaL == id select s;
            if (!string.IsNullOrEmpty(Searchtext))
            {
                items = items.Where(x => x.TenSP.Contains(Searchtext));
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(items);
            
        }
        public ActionResult Details(int id)
        {

            var sanpham = data.SanPhams.FirstOrDefault(s => s.MaSP == id);


            var sanPhamCungLoai = data.SanPhams
                .Where(sp => sp.LoaiSanPham == sanpham.LoaiSanPham)
                .Take(3);

            var binhluan = data.BinhLuans.Where(bl => bl.MaSP == id).ToList();

            var loaisanpham = data.LoaiSanPhams
               .Where(lsp => lsp.MaL == id);
            var hinhanh = data.HinhAnhs
                .Where(ha => ha.MaSP == id);
            var sanphamsize = data.SanPhamSizes
                .Where(sps => sps.MaSP == id);
            var size = from sz in data.Sizes select sz;
            var item = data.SanPhams.SingleOrDefault(sp => sp.MaSP == id);
            var availableSizes = data.Sizes.ToList();

            var detailView = new DetailedViewModels
            {
                SanPham = sanpham,
                SanPhamCungloai = sanPhamCungLoai,
                BinhLuan = binhluan,
                HinhAnh = hinhanh,
                SanPhamSize = sanphamsize,
                AvailableSizes = availableSizes,
                Size = size

            };
            return View(detailView);
        }
        public ActionResult Partial_ProductBST()
        {
            var bst = from cd in data.BoSuuTaps select cd;
            return PartialView(bst);
        }
        public PartialViewResult SanPhamTheoBST(string Searchtext, int id, int? page)
        {
            var pageSize = 8;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<SanPham> items = from s in data.SanPhams where s.MaBST == id select s;
            if (!string.IsNullOrEmpty(Searchtext))
            {
                items = items.Where(x => x.TenSP.Contains(Searchtext));
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return PartialView(items);
        }
        public ActionResult Partial_ProductSize()
        {
            var sz = from cd in data.Sizes select cd;
            return PartialView(sz);
        }
        public PartialViewResult SanPhamTheoSizes(string Searchtext, int id, int? page)
        {
            var pageSize = 8;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<SanPhamSize> items = from s in data.SanPhamSizes where s.MaS == id select s;
            if (!string.IsNullOrEmpty(Searchtext))
            {
                items = items.Where(x => x.SanPham.TenSP.Contains(Searchtext));
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return PartialView(items);
        }
    }
}