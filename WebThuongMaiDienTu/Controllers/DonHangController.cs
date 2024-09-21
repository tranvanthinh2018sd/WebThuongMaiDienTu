using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebThuongMaiDienTu.Models;

namespace WebThuongMaiDienTu.Controllers
{
    public class DonHangController : Controller
    {
        // GET: DonHang
        public ActionResult Index()
        {
            // Kiểm tra quyền admin
            var taiKhoan = (TaiKhoan)Session["TaiKhoan"];
            if (taiKhoan == null || taiKhoan.taiKhoanAdmin != true)
            {
                // Nếu không có quyền admin, chuyển hướng về trang đăng nhập
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            shopDienThoaiEntities db = new shopDienThoaiEntities();
            List<DonHang> donHangs = db.DonHang.OrderByDescending(row => row.maDonHang).ToList();
            return View(donHangs);
        }

        //Edit
        public ActionResult Edit(int maDonHang)
        {
            // Kiểm tra quyền admin
            var taiKhoan = (TaiKhoan)Session["TaiKhoan"];
            if (taiKhoan == null || taiKhoan.taiKhoanAdmin != true)
            {
                // Nếu không có quyền admin, chuyển hướng về trang đăng nhập
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            shopDienThoaiEntities db = new shopDienThoaiEntities();
            ViewBag.KhachHang = db.KhachHang.ToList();
            DonHang donHang = db.DonHang.Where(row => row.maDonHang == maDonHang).FirstOrDefault();
            return View(donHang);
        }
        [HttpPost]
        public ActionResult Edit(DonHang donHang)
        {
            // Kiểm tra quyền admin
            var taiKhoan = (TaiKhoan)Session["TaiKhoan"];
            if (taiKhoan == null || taiKhoan.taiKhoanAdmin != true)
            {
                // Nếu không có quyền admin, chuyển hướng về trang đăng nhập
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            shopDienThoaiEntities db = new shopDienThoaiEntities();
            DonHang donhang = db.DonHang.Where(row => row.maDonHang == donHang.maDonHang).FirstOrDefault();
            donhang.maDonHang = donHang.maDonHang;
            donhang.maKhachHang = donHang.maKhachHang;
            db.SaveChanges();
            return RedirectToAction("Index");

        }
    }
}