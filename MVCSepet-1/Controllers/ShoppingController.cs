using MVCSepet_1.CustomTools;
using MVCSepet_1.DesignPatterns.SingletonPattern;
using MVCSepet_1.Models;
using MVCSepet_1.Models.VMClasses.PageVM;
using MVCSepet_1.Models.VMClasses.PureVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCSepet_1.Controllers
{
    public class ShoppingController : Controller
    {
        NorthwindEntities _db;
        public ShoppingController()
        {
            _db = DBTool.DbInstance;
        }
        public ActionResult ListProducts()
        {
            List<ProductVM> products = _db.Products.Select(x => new ProductVM
            {
                ID = x.ProductID,
                ProductName = x.ProductName,
                UnitPrice = x.UnitPrice
            }).ToList();

            ListProductPageVM lpvm = new ListProductPageVM
            {
                Products = products,
            };
            return View(lpvm);
        }

        public ActionResult AddToCart(int id)
        {

            Cart c = Session["scart"] == null ? new Cart() : Session["scart"] as Cart;
            Product eklenecek = _db.Products.Find(id);
            CartItem ci = new CartItem();

            ci.ProductName = eklenecek.ProductName;
            ci.ID = eklenecek.ProductID;
            ci.UnitPrice = eklenecek.UnitPrice;

            c.SepeteEkle(ci);
            Session["scart"] = c;
            TempData["mesaj"] = $"{ci.ProductName} isimli ürün sepetinize eklenmiştir.";
            return RedirectToAction("ListProducts");
        }

        public ActionResult CartPage()
        {
            if (Session["scart"] != null)
            {
                Cart c = Session["scart"] as Cart;
                CartPageVM cpvm = new CartPageVM
                {
                    Cart = c
                };
                return View(cpvm);
            }
            ViewBag.SepetBos = "Sepetinizde ürün bulunmamaktadır";
            return View();
        }


        public ActionResult DeleteFromCart(int id)
        {
            if (Session["scart"] != null)
            {
                Cart c = Session["scart"] as Cart;
                c.SepettenSil(id);
                if (c.Sepetim.Count == 0) Session.Remove("scart");
                return RedirectToAction("CartPage");

            }
            return RedirectToAction("ListProducts");

        }
    }
}