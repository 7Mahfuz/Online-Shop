using Online_Shop.DAL;
using Online_Shop.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Online_Shop.Controllers
{
    public class AdminController : Controller
    {
        private GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }





        public ActionResult Dashboard()
        {
            return View();
        }


        public ActionResult Categories()
        {
            List<Category> AllCategories = _unitOfWork.GetRepositoryInstance<Category>().GetAllRecordsIQueryable().Where(i => i.IsDelete == false).ToList();
            return View(AllCategories);
        }
    }
}