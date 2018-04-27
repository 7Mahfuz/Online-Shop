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
    public class SearchController : Controller
    {
        // Instance on Unit of Work         
        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();
        private int _memberId;
        public int memberId
        {
            get { return Convert.ToInt32(Session["MemberId"]); }
            set { _memberId = Convert.ToInt32(Session["MemberId"]); }
        }
        public OnlineShopContext _context = new OnlineShopContext();

        public ActionResult Index(string searchKey = "")
        {
            ViewBag.searchKey = searchKey;
            List<Search_Result> Sr = new List<Search_Result>();
            // Searching from PRoduct table
            var model = new List<Product>();
            if (searchKey == "") model = _context.Product.ToList();
            else
                model = _context.Product.Where(x => x.ProductName.Contains(searchKey)).ToList();
            foreach (var item in model)
            {
                Search_Result sr = new Search_Result();
                sr.Price = item.Price; sr.ProductId = item.ProductId;
                sr.ProductImage = item.ProductImage; sr.ProductName = item.ProductName;
                sr.Description = item.Description; sr.CategoryName = _context.Category.FirstOrDefault(x => x.CategoryId == item.CategoryId).CategoryName;
                Sr.Add(sr);
            }

            //Now searching from category table


            int count = _context.Category.Count(x => x.CategoryName.Contains(searchKey));

            if (count > 0)
            {
                int Catid = _context.Category.FirstOrDefault(x => x.CategoryName.Contains(searchKey)).CategoryId;

                model = _context.Product.Where(x => x.CategoryId == Catid).ToList();
                foreach (var item in model)
                {
                    Search_Result sr = new Search_Result();
                    sr.Price = item.Price; sr.ProductId = item.ProductId;
                    sr.ProductImage = item.ProductImage; sr.ProductName = item.ProductName;
                    sr.Description = item.Description; sr.CategoryName = _context.Category.FirstOrDefault(x => x.CategoryId == item.CategoryId).CategoryName;
                    Sr.Add(sr);
                }
            }
        
            _context.Dispose();
            //List<Search_Result> sr = _unitOfWork.GetRepositoryInstance<Search_Result>().GetResultBySqlProcedure("USP_Search @searchKey", new SqlParameter("searchKey", SqlDbType.VarChar) { Value = searchKey }).ToList();
            return View(Sr);
        }

        /// <summary>
        /// Product Detail
        /// </summary>
        /// <param name="pId"></param>
        /// <returns></returns>
        public ActionResult ProductDetail(int pId)
        {
            Product pd = _unitOfWork.GetRepositoryInstance<Product>().GetFirstOrDefault(pId);
            ViewBag.SimilarProducts = _unitOfWork.GetRepositoryInstance<Product>().GetListByParameter(i => i.CategoryId == pd.CategoryId).ToList();
            return View(pd);
        }
    }
}