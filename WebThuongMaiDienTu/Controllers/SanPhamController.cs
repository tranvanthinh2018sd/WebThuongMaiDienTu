﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebThuongMaiDienTu.Models;

namespace WebThuongMaiDienTu.Controllers
{
    public class SanPhamController : Controller
    {
        // GET: SanPham
        [Authorize]
        public ActionResult Index()
        {
            shopDienThoaiEntities db = new shopDienThoaiEntities();

            // Lọc sản phẩm có kichHoat == true và sắp xếp theo maSanPham giảm dần
            List<SanPham> sanpham = db.SanPham
                .Where(row => row.kichHoat == true) // Điều kiện lọc
                .OrderByDescending(row => row.maSanPham) // Sắp xếp giảm dần theo maSanPham
                .ToList();

            return View(sanpham);
        }
        [Authorize]
        public ActionResult Create()
        {
            //trả về danh sách hãng sản xuất
            shopDienThoaiEntities db = new shopDienThoaiEntities();   
            ViewBag.HangSanXuat = db.HangSanXuat.ToList();            
            return View();
        }
        [HttpPost]
        public ActionResult Create(SanPham sanpham, HttpPostedFileBase hinhAnh)
        {
            using (shopDienThoaiEntities db = new shopDienThoaiEntities())
            {
                // Kích hoạt sản phẩm
                sanpham.kichHoat = true;

                // Xử lý tệp hình ảnh nếu có
                if (hinhAnh != null && hinhAnh.ContentLength > 0)
                {
                    // Xóa khoảng trắng trong tên sản phẩm
                    string tenSanPhamKhongKhoangTrang = sanpham.tenSanPham.Replace(" ", "");

                    // Lấy phần mở rộng của tệp hình ảnh (ví dụ: .jpg, .png)
                    string fileExtension = System.IO.Path.GetExtension(hinhAnh.FileName);

                    // Tạo tên tệp mới theo tên sản phẩm và phần mở rộng
                    string fileName = tenSanPhamKhongKhoangTrang + fileExtension;

                    // Đặt đường dẫn lưu tệp hình ảnh
                    string filePath = "~/Uploads/" + fileName;

                    // Lưu tệp hình ảnh vào thư mục trên máy chủ
                    hinhAnh.SaveAs(Server.MapPath(filePath));

                    // Cập nhật đường dẫn hình ảnh vào đối tượng sản phẩm (cắt bỏ 2 ký tự đầu ~/)
                    sanpham.hinhAnh = filePath.Substring(1);  // Lưu với đường dẫn bắt đầu từ Uploads
                }

                // Thêm sản phẩm vào cơ sở dữ liệu và lưu lại
                db.SanPham.Add(sanpham);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
        }
        //Edit
        [Authorize]
        public ActionResult Edit(int maSanPham)
        {
            shopDienThoaiEntities db = new shopDienThoaiEntities();
            SanPham sanPham = db.SanPham.Where(row => row.maSanPham == maSanPham).FirstOrDefault();
            ViewBag.HangSanXuat = db.HangSanXuat.ToList();
            return View(sanPham);
        }
        [HttpPost]
        public ActionResult Edit(SanPham sanpham, HttpPostedFileBase hinhAnh)
        {
                shopDienThoaiEntities db = new shopDienThoaiEntities();
                SanPham sanPham = db.SanPham.Where(row => row.maSanPham == sanpham.maSanPham).FirstOrDefault();
                if(sanpham != null)
                {
                    sanPham.maSanPham = sanpham.maSanPham;
                    sanPham.tenSanPham = sanpham.tenSanPham;
                    sanPham.moTa = sanpham.moTa;
                    sanPham.giaBan = sanpham.giaBan;
                    sanPham.giaKhuyenMai = sanpham.giaKhuyenMai;
                    // Xử lý tệp hình ảnh nếu có
                    if (hinhAnh != null && hinhAnh.ContentLength > 0)
                    {
                        // Xóa khoảng trắng trong tên sản phẩm
                        string tenSanPhamKhongKhoangTrang = sanpham.tenSanPham.Replace(" ", "");

                        // Lấy phần mở rộng của tệp hình ảnh (ví dụ: .jpg, .png)
                        string fileExtension = System.IO.Path.GetExtension(hinhAnh.FileName);

                        // Tạo tên tệp mới theo tên sản phẩm và phần mở rộng
                        string fileName = tenSanPhamKhongKhoangTrang + fileExtension;

                        // Đặt đường dẫn lưu tệp hình ảnh
                        string filePath = "~/Uploads/" + fileName;

                        // Lưu tệp hình ảnh vào thư mục trên máy chủ
                        hinhAnh.SaveAs(Server.MapPath(filePath));

                        // Cập nhật đường dẫn hình ảnh vào đối tượng sản phẩm (cắt bỏ 2 ký tự đầu ~/)
                        sanpham.hinhAnh = filePath.Substring(1);  // Lưu với đường dẫn bắt đầu từ Uploads
                    }
                }                               
                    db.SaveChanges();
                    return RedirectToAction("Edit");
        }
        //Delete
        [Authorize]
        public ActionResult Delete(int maSanPham)
        {
            shopDienThoaiEntities db = new shopDienThoaiEntities();
            SanPham sanPham = db.SanPham.Where(row => row.maSanPham == maSanPham).FirstOrDefault();
            return View(sanPham);
        }
        [HttpPost]
        public ActionResult Delete(int maSanPham, SanPham sp)
        {
            shopDienThoaiEntities db = new shopDienThoaiEntities();
            SanPham sanpham = db.SanPham.Where(row => row.maSanPham == maSanPham).FirstOrDefault();
            sanpham.kichHoat = false;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}