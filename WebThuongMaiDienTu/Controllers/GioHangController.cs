using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebThuongMaiDienTu.Models;

namespace WebThuongMaiDienTu.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GioHang
        [Authorize]
        public ActionResult Index()
        {
            shopDienThoaiEntities db = new shopDienThoaiEntities();

            // Lọc sản phẩm có kichHoat == true và sắp xếp theo maSanPham giảm dần
            List<GioHang> gioHangs = db.GioHang                
                .OrderByDescending(row => row.maGioHang) // Sắp xếp giảm dần theo maSanPham
                .ToList();
            return View(gioHangs);
        }
    }
}