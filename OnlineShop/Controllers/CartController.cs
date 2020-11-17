using Common;
using Model.Dao;
using Model.EF;
using OnlineShop.Areas.Admin.Controllers;
using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace OnlineShop.Controllers
{
    public class CartController : BaseController
    {
        private const string  CartSession = "CartSession";
        // GET: Cart
        public ActionResult Index()
        {
            var cart = Session[CartSession];
            var list = new List<CartItem>();

            if(cart!=null)
            {
                list=(List<CartItem>)cart;

            }    

            return View(list);
        }
        public JsonResult DeleteAll()
        {
            Session[CartSession] = null;
            return Json(new
            {
                status = true
            });
        }

        public JsonResult Delete(long id)
        {
            var sessionCart = (List<CartItem>)Session[CartSession];
            sessionCart.RemoveAll(x => x.Product.ID == id);
            Session[CartSession] = sessionCart;
         
            return Json(new
            {
                status = true
            });
        }


        public JsonResult Update(string cartModel)
        {
            var jsonCart = new JavaScriptSerializer().Deserialize<List<CartItem>>(cartModel);
            //var sessionCart = (List<CartItem>)Session[CartSession];
            List<CartItem> item = (List<CartItem>)Session[CartSession];
            
            foreach (var item1 in item)
            {
                
                var jsonItem = jsonCart.SingleOrDefault(x => x.Product.ID == item1.Product.ID);
                if (jsonItem.Quantity >= 0)
                {
                    item1.Quantity = jsonItem.Quantity;
                }
               
              
            }
           

            Session[CartSession] = item;
            return Json(new
            {
                status = true
            });
        }


        public ActionResult QtyMinus(int id)
        {
            int Count = 0;
            List<CartItem> item =  (List<CartItem>)Session[CartSession];
            for(int i=0;i<item.Count;i++)
            {
                if(item[i].Product.ID==id)
                {
                    Count = i;
                }    
               
            }   
            if(item[Count].Quantity >0)
            {
                item[Count].Quantity--;
            }
            else
            {
                return Content("<script>alert('Số lượng không thể nhỏ hơn 1'); window.history.back()</script>");
            }    
            

            Session[CartSession] = item;

            return RedirectToAction("Index");
        }
        public ActionResult QtyInc(int id)
        {
            int Count = 0;
            List<CartItem> item = (List<CartItem>)Session[CartSession];
            for (int i = 0; i < item.Count; i++)
            {
                if (item[i].Product.ID == id)
                {
                    Count = i;
                }

            }
            if (item[Count].Quantity >= 30)
            {
                return Content("<script>alert('Số lượng không thể nhỏ hơn 30'); window.history.back()</script>");
            }
            else
            {
                item[Count].Quantity++;
            }    


            Session[CartSession] = item;

            return RedirectToAction("Index");
        }



        public ActionResult AddItem(long ProductId,int quantity)
        {
            var product = new ProductDao().ViewDetail(ProductId);
            var cart = Session[CartSession];
            if(cart!=null)
            {
                var list = (List<CartItem>)cart;
                if(list.Exists(x=>x.Product.ID==ProductId))
                {
                    foreach (var item in list)
                    {
                        if (item.Product.ID == ProductId)
                        {
                            item.Quantity += quantity;
                        }

                    }

                }
                else
                {

                    var item = new CartItem();
                    item.Product = product;
                    item.Quantity = quantity;
                    list.Add(item);

                }
                // gán vào session
                Session[CartSession] = list;
                


            }    
            else
            {
                // tạo một đối tượng cart item 
                var item = new CartItem();
                item.Product = product;
                item.Quantity = quantity;
                var list = new List<CartItem>();
                list.Add(item);
                // gán vào session
                Session[CartSession] = list;

            }

            return RedirectToAction("Index");



        }

        [HttpGet]
        public ActionResult Payment()
        {
            var cart = Session[CartSession];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
            }
            return View(list);
        }



        [HttpPost]
        public ActionResult Payment(string shipName, string mobile,string address,string email)
        {
            var order = new Order();
            order.CreateDate = DateTime.Now;
            order.ShipName = shipName;
            order.ShipMobile = mobile;
            order.ShipAddress = address;
            order.ShipEmail = email;

            try
            {
                var id = new OrderDao().Insert(order);
                var cart = (List<CartItem>)Session[CartSession];
                var detailDao = new Model.Dao.OrderDetailDao();
                decimal total = 0;
                foreach (var item in cart)
                {
                    var orderDetail = new OrderDetail();
                    orderDetail.ProductID = item.Product.ID;
                    orderDetail.OrderID = id;
                    orderDetail.Price = item.Product.Price;
                    orderDetail.Quantity = item.Quantity;
                    detailDao.Insert(orderDetail);

                    total += (item.Product.Price.GetValueOrDefault(0) * item.Quantity);
                }
                string content = System.IO.File.ReadAllText(Server.MapPath("~/assets/client/template/neworder.html"));

                content = content.Replace("{{CustomerName}}", shipName);
                content = content.Replace("{{Phone}}", mobile);
                content = content.Replace("{{Email}}", email);
                content = content.Replace("{{Address}}", address);
                content = content.Replace("{{Total}}", total.ToString("N0"));
                var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();

                new MailHelper().SendMail(email, "Đơn hàng mới từ OnlineShop", content);
                new MailHelper().SendMail(toEmail, "Đơn hàng mới từ OnlineShop", content);
            }
            catch(Exception ex)
            {
                return Redirect("/loi-thanh-toan");
            }

            return Redirect("/hoan-thanh");




        }

        public ActionResult Success()
        {


            return View();
        }



    }
}