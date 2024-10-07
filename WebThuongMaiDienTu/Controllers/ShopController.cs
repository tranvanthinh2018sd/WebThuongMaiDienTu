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
            var taiKhoan = (TaiKhoan)Session["TaiKhoan"];
            // Lưu URL hiện tại (IndexKhachHang) vào Session trước khi chuyển hướng đến trang đăng nhập
            Session["returnUrl"] = Url.Action("Index", "Shop");
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

        [HttpPost]
        public ActionResult AddToCart(int maSanPham, int soLuong)
        {
            // Kiểm tra quyền quyền khách hàng
            var taiKhoan = (TaiKhoan)Session["TaiKhoan"];
            if (taiKhoan == null || taiKhoan.taiKhoanAdmin != false)
            {
                // Nếu không có quyền khách hàng, chuyển hướng về trang đăng nhập
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            // Giả sử mã khách hàng được lưu trong session
            int maKhachHang = taiKhoan.maKhachHang;
            using (shopDienThoaiEntities db = new shopDienThoaiEntities())
            {
                // Lấy giỏ hàng hiện tại của khách hàng
                var gioHang = db.GioHang.FirstOrDefault(g => g.maKhachHang == maKhachHang);

                if (gioHang == null)
                {
                    // Nếu chưa có giỏ hàng, tạo mới
                    gioHang = new GioHang
                    {
                        maKhachHang = maKhachHang,
                        ngayTao = DateTime.Now
                    };
                    db.GioHang.Add(gioHang);
                    db.SaveChanges(); // Lưu giỏ hàng mới
                }

                // Lấy sản phẩm từ database
                var sanPham = db.SanPham.FirstOrDefault(s => s.maSanPham == maSanPham);
                if (sanPham == null)
                {
                    return HttpNotFound();
                }

                // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
                var chiTietGioHang = db.ChiTietGioHang
                    .FirstOrDefault(c => c.maGioHang == gioHang.maGioHang && c.maSanPham == maSanPham);

                if (chiTietGioHang != null)
                {
                    // Nếu sản phẩm đã tồn tại, cập nhật số lượng
                    chiTietGioHang.soLuong += soLuong;
                }
                else
                {
                    // Nếu chưa có, thêm sản phẩm vào giỏ hàng chi tiết
                    chiTietGioHang = new ChiTietGioHang
                    {
                        maGioHang = gioHang.maGioHang,
                        maSanPham = maSanPham,
                        soLuong = soLuong,
                        donGia = sanPham.giaBan*soLuong
                    };
                    db.ChiTietGioHang.Add(chiTietGioHang);
                }

                // Lưu thay đổi vào database
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}