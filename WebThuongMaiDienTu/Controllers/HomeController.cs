using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebThuongMaiDienTu.Models;

namespace WebThuongMaiDienTu.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            // Kiểm tra quyền admin
            var taiKhoan = (TaiKhoan)Session["TaiKhoan"];
            if (taiKhoan == null || taiKhoan.taiKhoanAdmin != true)
            {
                // Nếu không có quyền admin, chuyển hướng về trang đăng nhập
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            using (shopDienThoaiEntities db = new shopDienThoaiEntities())
            {
                // Lấy số lượng sản phẩm theo từng hãng
                var productCountByManufacturer = (from sp in db.SanPham
                                                  join hsx in db.HangSanXuat on sp.maHang equals hsx.maHang
                                                  group sp by hsx.tenHang into g
                                                  select new SanPhamThongKeViewModel
                                                  {
                                                      HangSanXuat = g.Key,
                                                      SoLuongSanPham = g.Count() // Đếm sản phẩm theo từng hãng
                                                  }).ToList();

                return View(productCountByManufacturer);
            }
        }

    }
}