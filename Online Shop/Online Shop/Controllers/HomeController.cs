using Online_Shop.DAL;
using Online_Shop.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Online_Shop.Controllers
{
    public class HomeController : Controller
    {
        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();

        /// <summary>
        /// Home Page
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            
            ViewBag.FeaturedProducts = _unitOfWork.GetRepositoryInstance<Product>().GetListByParameter(i => i.IsFeatured == true && i.IsDelete == false && i.IsActive == true).ToList();
             return View();
        }

        #region Disposing UnitOfWork Context ...
        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
        #endregion
    }
}