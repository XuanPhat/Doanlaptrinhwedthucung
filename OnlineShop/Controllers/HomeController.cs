using Model.Dao;
using OnlineShop.Areas.Admin.Controllers;
using OnlineShop.Common;
using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.Slides = new SlideDao().ListAll();
          var productDao = new ProductDao();
            ViewBag.NewProducts = productDao.ListNewProduct(4);
            ViewBag.ListFeatureProducts = productDao.ListFeatureProduct(4);

            return View();
        }


        [ChildActionOnly]
        public ActionResult MainMenu()
        {
            var model = new MenuDao().ListByGroupId(1);

            return PartialView(model);

        }


        [ChildActionOnly]
        public ActionResult TopMenu()
        {
            var model = new MenuDao().ListByGroupId(2);

            return PartialView(model);

        }

        [ChildActionOnly]
        public ActionResult Footer()
        {

            var model = new FooterDao().getFooter();

            return PartialView(model);

        }
        [ChildActionOnly]
        public ActionResult HeaderCart()
        {

            var cart = Session[CommonConstants.CartSession];
            var list = new List<CartItem>();

            if (cart != null)
            {
                list = (List<CartItem>)cart;

            }

            return PartialView(list);

           

        }



    }
}