using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebThuongMaiDienTu.Models;

namespace WebThuongMaiDienTu.Controllers
{
    public class HangSanXuatController : Controller
    {
        // GET: HangSanXuat
        public ActionResult Index()
        {
            shopDienThoaiEntities db = new shopDienThoaiEntities();
            List<HangSanXuat> hangSanxuat = db.HangSanXuat.Where(row => row.kichHoat== true).ToList();
            return View(hangSanxuat);        
        }
        //Create
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(HangSanXuat HangSanXuat)
        {            
                shopDienThoaiEntities db = new shopDienThoaiEntities();
                HangSanXuat.kichHoat = true;
                db.HangSanXuat.Add(HangSanXuat);
                db.SaveChanges();
                return RedirectToAction("Index");            
        }

        //Edit
        public ActionResult Edit(int maHang)
        {
            shopDienThoaiEntities db = new shopDienThoaiEntities();
            HangSanXuat hangSanXuat = db.HangSanXuat.Where(row => row.maHang == maHang).FirstOrDefault();
            return View(hangSanXuat);
        }
        [HttpPost]
        public ActionResult Edit(HangSanXuat hang)
        {
            if (ModelState.IsValid)
            {
                shopDienThoaiEntities db = new shopDienThoaiEntities();
                HangSanXuat hangSanXuat = db.HangSanXuat.Where(row => row.maHang == hang.maHang).FirstOrDefault();
                hangSanXuat.maHang = hang.maHang;
                hangSanXuat.tenHang = hang.tenHang;
                hangSanXuat.moTa = hang.moTa;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Edit");
            }
            
        }

        //Delete
        public ActionResult Delete(int maHang)
        {
            shopDienThoaiEntities db = new shopDienThoaiEntities();
            HangSanXuat hangSanXuat = db.HangSanXuat.Where(row => row.maHang == maHang).FirstOrDefault();
            return View(hangSanXuat);
        }
        [HttpPost]
        public ActionResult Delete(int maHang, HangSanXuat hang)
        {
            shopDienThoaiEntities db = new shopDienThoaiEntities();
            HangSanXuat hangSanXuat = db.HangSanXuat.Where(row => row.maHang == maHang).FirstOrDefault();
            hangSanXuat.kichHoat = false;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}