using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Configuration;
using ShoppingCart.Models;
using ShoppingCart.ViewModel;
using System.Web.Security;
using System.IO;
using System.Text;
using System.Data;
using System.Net;
using System.Net.Mail;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;


namespace ShoppingCart.Controllers
{
    public class AccountController : Controller
    {
        #region Define Variables
        private readonly HttpClient client;
        private readonly string BaseUrl;
        List<Cart> li = new List<Cart>();
        #endregion

        #region Constructor
        public AccountController()
        {
            client = new HttpClient();
            BaseUrl = ConfigurationManager.AppSettings["url"];
            client.BaseAddress = new Uri(BaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        #endregion

        #region Add To Cart
        public ActionResult Index()
        {
            //check cart
            if (TempData["cart"] != null)
            {
                float x = 0;
                List<Cart> li2 = TempData["cart"] as List<Cart>;
                foreach (var item in li2)
                {
                    x += item.Bill;

                }

                TempData["total"] = x;
            }
            TempData.Keep();
            List<Product> PR = new List<Product>();

            HttpResponseMessage getProducts = client.GetAsync("GetProducts").Result;
            if (getProducts.IsSuccessStatusCode)
            {
                PR = getProducts.Content.ReadAsAsync<List<Product>>().Result.ToList();
                return View(PR);
            }
            return View();
        }

        public ActionResult AddToCart(int Id)
        {
            Product p = new Product();
            HttpResponseMessage getdetails = client.GetAsync("AddToCart/" + Id).Result;
            if (getdetails.IsSuccessStatusCode)
            {
                p = getdetails.Content.ReadAsAsync<Product>().Result;
                return View(p);
            }
            return View();

        }

        [HttpPost]
        public ActionResult AddToCart(Product pi, string qty, int Id)
        {

            Product p = new Product();
            Cart c = new Cart();

            //API Cal
            HttpResponseMessage getdetails1 = client.GetAsync("AddToCart/" + Id).GetAwaiter().GetResult();
            if (getdetails1.IsSuccessStatusCode)
            {
                p = getdetails1.Content.ReadAsAsync<Product>().Result;
            }

            c.Productid = p.Product_Id;
            c.Price = (float)p.Product_Price;
            c.Qty = Convert.ToInt32(qty);
            c.Bill = c.Price * c.Qty;
            c.Productname = p.Product_Name;
            if (TempData["cart"] == null)
            {
                li.Add(c);
                TempData["cart"] = li;

            }
            else
            {
                List<Cart> li2 = TempData["cart"] as List<Cart>;
                li2.Add(c);
                TempData["cart"] = li2;
            }

            TempData.Keep();

            return RedirectToAction("Index");
        }
        #endregion

        #region Login
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }
        //Register User
        [HttpPost]
        public ActionResult SaveRegisterDetails(Register regUser)
        {
            var getEmployee = client.PostAsJsonAsync("SaveRegisterDetails", regUser).Result;
            if (getEmployee.IsSuccessStatusCode)
            {
                ViewBag.Message = "User Details Saved";
                return View("Register");
            }
            else
            {

                return View("Register", regUser);
            }

        }
        [HttpPost]
        //Login
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                //Validating the user, whether the user is valid or not.
                var isValidUser = IsValidUser(model);

                if (isValidUser != null)
                {
                    TempData["UserId"] = isValidUser.Id;
                    FormsAuthentication.SetAuthCookie(model.Email, false);
                    TempData.Keep();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Failure", "Wrong Username and password combination !");
                    return View();
                }
            }
            else
            {
                return View(model);
            }
        }
        //function to check if User is valid or not
        [HttpPost]
        public RegisterUser IsValidUser(LoginViewModel model)
        {


            RegisterUser reg = new RegisterUser();
            HttpResponseMessage getEmployee = client.PostAsJsonAsync("IsValidUser", model).GetAwaiter().GetResult();
            if (getEmployee.IsSuccessStatusCode)
            {
                reg = getEmployee.Content.ReadAsAsync<RegisterUser>().Result;
                TempData["UserId"] = reg.Id;
                TempData.Keep();
                return reg;
            }
            else
            {

                return null;
            }
        }
        //Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("Index");
        }
        #endregion

        #region checkout
        public ActionResult Checkout()
        {
            TempData.Keep();
            return View();
        }
        //Save Order Details
        public ActionResult ConfirmOrder()
        {
            int UserID = 0;
            Invoice inv = new Invoice();
            Invoice invRes = new Invoice();
            Cart c = new Cart();
            List<Order> orderRes = new List<Order>();
            List<Cart> cartItems = TempData["cart"] as List<Cart>;

            var cartTotal = TempData["Total"];
            inv.Invoice_TotalBill = Convert.ToDouble(cartTotal);

            if (TempData["UserId"] != null)
            {
                UserID = (int)TempData["UserId"];
            }
            inv.UserId = UserID;
            HttpResponseMessage getInvoiceDetails = client.PostAsJsonAsync("SaveInvoiceDetails", inv).GetAwaiter().GetResult();
            if (getInvoiceDetails.IsSuccessStatusCode)
            {
                Order order = new Order();
                invRes = getInvoiceDetails.Content.ReadAsAsync<Invoice>().Result;

                foreach (var item in cartItems)
                {
                    order.Invoice_Id = invRes.Invoice_Id;
                    order.Order_Bill = item.Bill;
                    order.Order_Quantity = item.Qty;
                    order.Order_UnitPrice = (int?)item.Price;
                    order.Product_Id = item.Productid;
                    order.Order_Date = DateTime.Now;
                    orderRes.Add(order);
                }

                HttpResponseMessage getResult = client.PostAsJsonAsync("SaveOrderDetails", orderRes).GetAwaiter().GetResult();
                if (getResult.IsSuccessStatusCode)
                {
                    ViewBag.Message = "Order Confirmed";
                    return View("Index");
                }
                else
                {
                    return null;
                }
            }
            else
            {
                ViewBag.Message = "Issue";
                return View("Index");
            }

            return View();
        }
        #endregion

        #region Email
        public ActionResult SendInvoice()
        {
            if (TempData["cart"] != null)
            {

                using (StringWriter sw = new StringWriter())
                {
                    using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                    {
                        Cart c = new Cart();
                        List<Cart> li2 = TempData["cart"] as List<Cart>;
                        li2.Add(c);
                        TempData["cart"] = li2;

                        using (MemoryStream memoryStream = new MemoryStream())
                        {

                            byte[] bytes = memoryStream.ToArray();
                            memoryStream.Close();


                            MailMessage mail = new MailMessage();
                            mail.To.Add("soniya.parepalli@gmail.com");
                            mail.From = new MailAddress("soniya.parepalli@gmail.com");
                            mail.Subject = "hfgv";
                            mail.Body = "hffgh";
                            mail.IsBodyHtml = true;
                            SmtpClient smtp = new SmtpClient();
                            smtp.Host = "smtp.gmail.com";
                            smtp.EnableSsl = true;
                            NetworkCredential NetworkCred = new NetworkCredential("soniya.parepalli@gmail.com", "1254");
                            smtp.UseDefaultCredentials = true;
                            smtp.Credentials = NetworkCred;
                            smtp.Port = 587;
                            smtp.Send(mail);
                        }
                    }
                }

            }

            return View();
        }
        #endregion

    }
}