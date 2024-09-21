using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebThuongMaiDienTu.Models;

namespace WebThuongMaiDienTu.Controllers
{
    public class ChiTietGioHangController : Controller
    {
        // GET: ChiTietGioHang
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
            List<ChiTietGioHang> chiTietGioHangs = db.ChiTietGioHang.OrderByDescending(row => row.maChiTietGioHang).ToList();
            return View(chiTietGioHangs);
        }

        //Edit
        public ActionResult Edit(int maChiTietGioHang)
        {
            // Kiểm tra quyền admin
            var taiKhoan = (TaiKhoan)Session["TaiKhoan"];
            if (taiKhoan == null || taiKhoan.taiKhoanAdmin != true)
            {
                // Nếu không có quyền admin, chuyển hướng về trang đăng nhập
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            shopDienThoaiEntities db = new shopDienThoaiEntities();
            ViewBag.GioHang = db.GioHang.ToList();
            ViewBag.SanPham = db.SanPham.ToList();
            ChiTietGioHang chiTietGioHang = db.ChiTietGioHang.Where(row => row.maChiTietGioHang == maChiTietGioHang).FirstOrDefault();
            return View(chiTietGioHang);
        }
        [HttpPost]
        public ActionResult Edit(ChiTietGioHang chiTietGioHang)
        {
            // Kiểm tra quyền admin
            var taiKhoan = (TaiKhoan)Session["TaiKhoan"];
            if (taiKhoan == null || taiKhoan.taiKhoanAdmin != true)
            {
                // Nếu không có quyền admin, chuyển hướng về trang đăng nhập
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            shopDienThoaiEntities db = new shopDienThoaiEntities();
            ChiTietGioHang chitietgiohang = db.ChiTietGioHang.Where(row => row.maChiTietGioHang == chiTietGioHang.maChiTietGioHang).FirstOrDefault();
            chitietgiohang.maChiTietGioHang = chiTietGioHang.maChiTietGioHang;
            chitietgiohang.maGioHang = chiTietGioHang.maGioHang;
            chitietgiohang.maSanPham = chiTietGioHang.maSanPham;
            chitietgiohang.soLuong = chiTietGioHang.soLuong;
            chitietgiohang.donGia = chiTietGioHang.donGia;
            db.SaveChanges();
            return RedirectToAction("Index");

        }
    }
}