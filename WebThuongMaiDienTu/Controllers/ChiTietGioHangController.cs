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

        // GET: ChiTietGioHang của khách hàng
        public ActionResult IndexKhachHang()
        {
            // Kiểm tra quyền khách hàng
            var taiKhoan = (TaiKhoan)Session["TaiKhoan"];
            if (taiKhoan == null || taiKhoan.taiKhoanAdmin != false)
            {
                // Lưu URL hiện tại (IndexKhachHang) vào Session trước khi chuyển hướng đến trang đăng nhập
                Session["returnUrl"] = Url.Action("IndexKhachHang", "ChiTietGioHang");

                // Nếu không có quyền khách hàng, chuyển hướng về trang đăng nhập
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            using (var db = new shopDienThoaiEntities())
            {
                // Lấy mã khách hàng từ tài khoản hiện tại
                int maKhachHang = taiKhoan.maKhachHang;

                // Lấy giỏ hàng của khách hàng
                var gioHang = db.GioHang.SingleOrDefault(g => g.maKhachHang == maKhachHang);
                if (gioHang == null)
                {
                    // Nếu không tìm thấy giỏ hàng, có thể xử lý theo nhu cầu
                    return View(new List<ChiTietGioHang>());
                }

                // Lấy danh sách các mã sản phẩm đã đặt hàng của khách hàng
                var sanPhamDaDat = db.ChiTietDonHang
                                      .Where(ctdh => ctdh.DonHang.maKhachHang == maKhachHang)
                                      .Select(ctdh => ctdh.maSanPham)
                                      .ToList();

                // Lọc các sản phẩm trong giỏ hàng dựa trên mã giỏ hàng và chưa được đặt hàng
                var chiTietGioHangs = db.ChiTietGioHang
                                         .Where(ct => ct.maGioHang == gioHang.maGioHang &&
                                                      !sanPhamDaDat.Contains(ct.maSanPham)) // Kiểm tra nếu chưa có trong danh sách đặt hàng
                                         .OrderByDescending(row => row.maChiTietGioHang)
                                         .ToList();

                // Lấy danh sách tất cả sản phẩm nếu cần
                ViewBag.Sanpham = db.SanPham.ToList();

                return View(chiTietGioHangs);
            }
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