using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebThuongMaiDienTu.Models;

namespace WebThuongMaiDienTu.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            shopDienThoaiEntities db = new shopDienThoaiEntities();
            List<SanPham> sanPhams = db.SanPham.ToList();
            return View(sanPhams);
        }
        // GET: Shop/Details/5
        public ActionResult Details(int maSanPham)
        {
            shopDienThoaiEntities db = new shopDienThoaiEntities();
            SanPham sanpham = db.SanPham.Where(row => row.maSanPham == maSanPham).FirstOrDefault();
            return View(sanpham);
        }
    }
}