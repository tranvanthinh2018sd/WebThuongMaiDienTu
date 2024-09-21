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
            var taiKhoan = (TaiKhoan)Session["TaiKhoan"];
            if(taiKhoan == null || taiKhoan.taiKhoanAdmin == false)
            {
                Session.Clear();
                return RedirectToAction("DangNhap", "TaiKhoan");

            }
            return View();
        }
    }
}