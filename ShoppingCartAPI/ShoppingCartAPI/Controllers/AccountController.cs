using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Configuration;
using DAL;
using System.Net;
using System.Web.Http;
using ShoppingCartAPI.Models;
namespace ShoppingCartAPI.Controllers
{
    public class AccountController : ApiController
    {
        #region Registering User
        [Route("SaveRegisterDetails")]
        [HttpPost]
        public HttpResponseMessage SaveRegisterDetails(RegisterUser registerDetails)
        {
            try
            {
                //create database context using Entity framework 
                using (var databaseContext = new ShoppingCartDBEntities())
                {
                    RegisterUser reglog = new RegisterUser();

                    reglog.FirstName = registerDetails.FirstName;
                    reglog.LastName = registerDetails.LastName;
                    reglog.Email = registerDetails.Email;
                    reglog.Password = registerDetails.Password;


                    databaseContext.RegisterUsers.Add(reglog);
                    databaseContext.SaveChanges();

                    var message = Request.CreateResponse(HttpStatusCode.Created, registerDetails);
                    message.Headers.Location = new Uri(Request.RequestUri + "/" +
                        registerDetails.Id.ToString());

                    return message;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [Route("IsValidUser")]
        [HttpPost]
        public HttpResponseMessage IsValidUser(LoginModel model)
        {
            using (var dataContext = new ShoppingCartDBEntities())
            {
                //Retireving the user details from DB based on username and password enetered by user.
                RegisterUser user = dataContext.RegisterUsers.Where(query => query.Email.Equals(model.Email) && query.Password.Equals(model.Password)).SingleOrDefault();
                //If user is present, then true is returned.
                if (user != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, user);
                }
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "");

            }
        }
        #endregion

        #region Get Products
       

        [Route("GetProducts")]
        [HttpGet]
        public HttpResponseMessage GetProducts()
        {
            using (var dataContext = new ShoppingCartDBEntities())
            {
                IEnumerable<Tbl_Product> products = dataContext.Tbl_Product.OrderByDescending(x => x.Product_Id).ToList();
                if (products != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, products);
                }
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "");

            }
        }

        [Route("AddToCart/{Id:int}")]
        [HttpGet]
        public HttpResponseMessage AddToCart(int Id)
        {
            using (var dataContext = new ShoppingCartDBEntities())
            {
                Tbl_Product products = dataContext.Tbl_Product.Where(x => x.Product_Id == Id).SingleOrDefault();

                if (products != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, products);
                }
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "");

            }
        }
        #endregion

        #region Save Orders
       
        [Route("SaveInvoiceDetails")]
        [HttpPost]
        public HttpResponseMessage SaveInvoiceDetails(Tbl_Invoice invDetails)
        {
            try
            {
                using (var databaseContext = new ShoppingCartDBEntities())
                {
                    int invoiceid = 0;
                    Tbl_Invoice inv = new Tbl_Invoice();

                    inv.Invoice_Date = DateTime.Now;
                    inv.Invoice_TotalBill = invDetails.Invoice_TotalBill;
                    inv.UserId = invDetails.UserId;

                    databaseContext.Tbl_Invoice.Add(inv);
                    databaseContext.SaveChanges();
                    invoiceid = inv.Invoice_Id;
                    if (inv != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, inv);
                    }
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "");

                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

       
        [Route("SaveOrderDetails")]
        [HttpPost]
        public HttpResponseMessage SaveOrderDetails(List<Tbl_Order> oDetails)
        {
            try
            {
                using (var databaseContext = new ShoppingCartDBEntities())
                {
                    foreach (var item in oDetails)
                    {
                        Tbl_Order ord = new Tbl_Order();
                        ord.Order_Date = DateTime.Now;
                        ord.Invoice_Id = item.Invoice_Id;
                        ord.Order_Bill = item.Order_Bill;
                        ord.Order_Quantity = item.Order_Quantity;
                        ord.Order_UnitPrice = item.Order_UnitPrice;
                        ord.Product_Id = item.Product_Id;
                        databaseContext.Tbl_Order.Add(ord);
                        databaseContext.SaveChanges();
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, oDetails);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }


        #endregion


    }
}