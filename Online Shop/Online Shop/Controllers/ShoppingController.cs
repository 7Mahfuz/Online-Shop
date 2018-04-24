using Online_Shop.Authorising;
using Online_Shop.DAL;
using Online_Shop.Models;
using Online_Shop.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Online_Shop.Controllers
{
    [AuthorizeUser]
    public class ShoppingController : Controller
    {
        #region Other Class references ...         
        // Instance on Unit of Work         
        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();
        public OnlineShopContext _context = new OnlineShopContext();
        private int _memberId;
        public int memberId
        {
            get { return Convert.ToInt32(Session["MemberId"]); }
            set { _memberId = Convert.ToInt32(Session["MemberId"]); }
        }
        #endregion

        /// <summary>
        /// Add Product To Cart
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public ActionResult AddProductToCart(int productId)
        {
            Cart c = new Cart();
            c.AddedOn = DateTime.Now;
            c.CartStatusId = 1;
            c.MemberId = memberId;
            c.ProductId = productId;
            c.UpdatedOn = DateTime.Now;
            _unitOfWork.GetRepositoryInstance<Cart>().Add(c);
            _unitOfWork.SaveChanges();
            TempData["ProductAddedToCart"] = "Product added to cart successfully";
            return RedirectToAction("Index", "Search");
        }

        /// <summary>
        /// MyCart
        /// </summary>
        /// <returns>List of cart items</returns>
        public ActionResult MyCart()
        {
            //List<MemberShoppingCartDetails_Result> cd = _unitOfWork.GetRepositoryInstance<MemberShoppingCartDetails_Result>().GetResultBySqlProcedure("USP_MemberShoppingCartDetails @memberId",
            //    new SqlParameter("memberId", System.Data.SqlDbType.Int) { Value = memberId }).ToList();
            List<MemberShoppingCartDetails_Result> CD = new List<MemberShoppingCartDetails_Result>();
            
            var carts = _context.Cart.Where(x => x.MemberId == memberId).ToList();
            foreach(var item in carts)
            {
                MemberShoppingCartDetails_Result temp = new MemberShoppingCartDetails_Result();
                
                temp.CartId = item.CartId;temp.ProductId = item.ProductId;
                temp.ProductImage = _context.Product.FirstOrDefault(x => x.ProductId == item.ProductId).ProductImage;
                temp.ProductName= _context.Product.FirstOrDefault(x => x.ProductId == item.ProductId).ProductName;
                temp.Price= _context.Product.FirstOrDefault(x => x.ProductId == item.ProductId).Price;
                temp.CategoryName = _context.Category.FirstOrDefault(x => x.CategoryId == (_context.Product.FirstOrDefault(y => y.ProductId == temp.ProductId).CategoryId)).CategoryName;
                CD.Add(temp);
            }

            return View(CD);
        }

        /// <summary>
        /// Remove Cart Item
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public ActionResult RemoveCartItem(int productId)
        {
            Cart c = _unitOfWork.GetRepositoryInstance<Cart>().GetFirstOrDefaultByParameter(i => i.ProductId == productId && i.MemberId == memberId && i.CartStatusId == 1);
            c.CartStatusId = 2;
            c.UpdatedOn = DateTime.Now;
            _unitOfWork.GetRepositoryInstance<Cart>().Update(c);
            _unitOfWork.SaveChanges();
            return RedirectToAction("MyCart");
        }

        /// <summary>
        /// CheckOut the Cart items
        /// </summary>
        /// <returns></returns>
        public ActionResult CheckOut()
        {
            //List<MemberShoppingCartDetails_Result> cd = _unitOfWork.GetRepositoryInstance<MemberShoppingCartDetails_Result>().GetResultBySqlProcedure("USP_MemberShoppingCartDetails @memberId",
            //   new SqlParameter("memberId", System.Data.SqlDbType.Int) { Value = memberId }).ToList();
            List<MemberShoppingCartDetails_Result> CD = new List<MemberShoppingCartDetails_Result>();

            var carts = _context.Cart.Where(x => x.MemberId == memberId).ToList();
            foreach (var item in carts)
            {
                MemberShoppingCartDetails_Result temp = new MemberShoppingCartDetails_Result();

                temp.CartId = item.CartId; temp.ProductId = item.ProductId;
                temp.ProductImage = _context.Product.FirstOrDefault(x => x.ProductId == item.ProductId).ProductImage;
                temp.ProductName = _context.Product.FirstOrDefault(x => x.ProductId == item.ProductId).ProductName;
                temp.Price = _context.Product.FirstOrDefault(x => x.ProductId == item.ProductId).Price;
                temp.CategoryName = _context.Category.FirstOrDefault(x => x.CategoryId == (_context.Product.FirstOrDefault(y => y.ProductId == temp.ProductId).CategoryId)).CategoryName;
                CD.Add(temp);
            }
            ViewBag.TotalPrice = CD.Sum(i => i.Price);
            ViewBag.CartIds = string.Join(",", CD.Select(i => i.CartId).ToList());
            return View(CD);
        }

        /// <summary>
        /// Payment Success
        /// </summary>
        /// <param name="shippingDetails"></param>
        /// <returns></returns>
        public ActionResult PaymentSuccess(ShippingDetail shippingDetails)
        {
            ShippingDetails sd = new ShippingDetails();
            sd.MemberId = memberId;
            sd.AddressLine = shippingDetails.Address;
            sd.City = shippingDetails.City;
            sd.State = shippingDetails.State;
            sd.Country = shippingDetails.Country;
            sd.ZipCode = shippingDetails.ZipCode;
            sd.OrderId = Guid.NewGuid().ToString();
            sd.AmountPaid = shippingDetails.TotalPrice;
            sd.PaymentType = shippingDetails.PaymentType;
            _unitOfWork.GetRepositoryInstance<ShippingDetails>().Add(sd);
            _unitOfWork.GetRepositoryInstance<Cart>().UpdateByWhereClause(i => i.MemberId == memberId && i.CartStatusId == 1, (j => j.CartStatusId = 3));
            _unitOfWork.SaveChanges();
            if (!string.IsNullOrEmpty(Request["CartIds"]))
            {
                int[] cartIdsToUpdate = Request["CartIds"].Split(',').Select(Int32.Parse).ToArray();
                _unitOfWork.GetRepositoryInstance<Cart>().UpdateByWhereClause(i => cartIdsToUpdate.Contains(i.CartId), (j => j.ShippingDetailId = sd.ShippingDetailId));
                _unitOfWork.SaveChanges();

            }
            return View(sd);
        }

    }
}