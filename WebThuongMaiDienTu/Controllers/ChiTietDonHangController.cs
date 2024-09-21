using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebThuongMaiDienTu.Models;

namespace WebThuongMaiDienTu.Controllers
{
    public class ChiTietDonHangController : Controller
    {
        // GET: ChiTietDonHang
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
            List<ChiTietDonHang> chiTietDonHangs = db.ChiTietDonHang.OrderByDescending(row => row.maChiTietDonHang).ToList();
            return View(chiTietDonHangs);
        }

        //Edit
        public ActionResult Edit(int maChiTietDonHang)
        {
            // Kiểm tra quyền admin
            var taiKhoan = (TaiKhoan)Session["TaiKhoan"];
            if (taiKhoan == null || taiKhoan.taiKhoanAdmin != true)
            {
                // Nếu không có quyền admin, chuyển hướng về trang đăng nhập
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            shopDienThoaiEntities db = new shopDienThoaiEntities();
            ViewBag.DonHang = db.DonHang.ToList();
            ViewBag.SanPham = db.SanPham.ToList();
            ChiTietDonHang chiTietDonHang = db.ChiTietDonHang.Where(row => row.maChiTietDonHang == maChiTietDonHang).FirstOrDefault();
            return View(chiTietDonHang);
        }
        [HttpPost]
        public ActionResult Edit(ChiTietDonHang chiTietDonHang)
        {
            // Kiểm tra quyền admin
            var taiKhoan = (TaiKhoan)Session["TaiKhoan"];
            if (taiKhoan == null || taiKhoan.taiKhoanAdmin != true)
            {
                // Nếu không có quyền admin, chuyển hướng về trang đăng nhập
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            shopDienThoaiEntities db = new shopDienThoaiEntities();
            ChiTietDonHang chitietdonhang = db.ChiTietDonHang.Where(row => row.maChiTietDonHang == chiTietDonHang.maChiTietDonHang).FirstOrDefault();
            chitietdonhang.maChiTietDonHang = chiTietDonHang.maChiTietDonHang;
            chitietdonhang.maSanPham = chiTietDonHang.maSanPham;
            chitietdonhang.maDonHang = chiTietDonHang.maDonHang;
            chitietdonhang.soLuong = chiTietDonHang.soLuong;
            chitietdonhang.donGia = chiTietDonHang.donGia;
            db.SaveChanges();
            return RedirectToAction("Index");

        }
    }
}