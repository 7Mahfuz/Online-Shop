using Newtonsoft.Json;
using Online_Shop.DAL;
using Online_Shop.Models;
using Online_Shop.Repository;
using Online_Shop.Utility;
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
        UploadContent uc = new UploadContent();
        OnlineShopContext context = new OnlineShopContext();

        public ActionResult Index()
        {
            int see = context.MemberRole.Count(x => x.RoleId == 1);
            if(see==0)
            {
                return RedirectToAction("Register");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (CheckAlreadyLoggedIn())
                return Redirect(returnUrl != null ? returnUrl : "/admin/dashboard");
            else
            {
                LoginViewModel loginModel = new LoginViewModel();
                loginModel.UserEmailId = Request.Cookies["RememberMe_UserEmailId"] != null ? Request.Cookies["RememberMe_UserEmailId"].Value : "";
                loginModel.Password = Request.Cookies["RememberMe_Password"] != null ? Request.Cookies["RememberMe_Password"].Value : "";
                ViewBag.ReturnUrl = returnUrl;
                return View(loginModel);
            }
        }

        /// <summary>
        /// Check if user is already logged in
        /// </summary>
        /// <returns></returns>
        private bool CheckAlreadyLoggedIn()
        {
            bool alreadyLoggedIn = false;
            if (Session["MemberId"] != null && Request.Cookies["MemberRole"] != null && Request.Cookies["MemberRole"].Value == "Admin")
            {
                int memberId = Convert.ToInt32(Session["MemberId"]);
                var user = _unitOfWork.GetRepositoryInstance<Member>().GetFirstOrDefaultByParameter(i => i.MemberId == memberId && i.IsActive == true && i.IsDelete == false);
                if (user != null)
                {
                    Response.Cookies["MemberName"].Value = user.FirstName;
                    var roles = _unitOfWork.GetRepositoryInstance<MemberRole>().GetFirstOrDefaultByParameter(i => i.MemberId == user.MemberId && i.RoleId == 3);
                    if (roles != null)
                        alreadyLoggedIn = true;
                }
            }
            return alreadyLoggedIn;
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                string EncryptedPassword = EncryptDecrypt.Encrypt(model.Password, true);
                var user = _unitOfWork.GetRepositoryInstance<Member>().GetFirstOrDefaultByParameter(i => i.EmailId == model.UserEmailId && i.Password == EncryptedPassword && i.IsActive == true && i.IsDelete == false);
                if (user != null)
                {
                    Session["MemberId"] = user.MemberId;
                    Response.Cookies["MemberName"].Value = user.FirstName;
                    var roles = _unitOfWork.GetRepositoryInstance<MemberRole>().GetFirstOrDefaultByParameter(i => i.MemberId == user.MemberId && i.RoleId == model.UserType);
                    if (roles != null)
                    {
                        Response.Cookies["MemberRole"].Value = _unitOfWork.GetRepositoryInstance<Roles>().GetFirstOrDefaultByParameter(i => i.RoleId == model.UserType).RoleName;
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Password is wrong");
                        return View(model);
                    }
                    if (Request["RememberMe1"] == "on")
                    {
                        Response.Cookies["RememberMe_UserEmailId"].Value = user.EmailId;
                        Response.Cookies["RememberMe_Password"].Value = user.Password;
                    }
                    else
                    {
                        Response.Cookies["RememberMe_UserEmailId"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["RememberMe_Password"].Expires = DateTime.Now.AddDays(-1);
                    }
                    return Redirect(returnUrl != null ? returnUrl : "/admin/dashboard");
                }
                else
                {
                    ModelState.AddModelError("Password", "Invalid email address or password");
                }
            }
            return View(model);
        }





        [AllowAnonymous]
        public ActionResult Register()
        {
            RegisterViewModel model = new RegisterViewModel();
            model.UserType = 1;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Register(RegisterViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {                 // Adding Member                 
                Member mem = new Member();
                mem.FirstName = model.FirstName;
                mem.LastName = model.LastName;
                mem.EmailId = model.UserEmailId;
                mem.CreatedOn = DateTime.Now;
                mem.ModifiedOn = DateTime.Now;
                mem.Password = EncryptDecrypt.Encrypt(model.Password, true);
                mem.IsActive = true;
                mem.IsDelete = false;
                _unitOfWork.GetRepositoryInstance<Member>().Add(mem);
                // Adding Member Role                 
                MemberRole mem_Role = new MemberRole();
                mem_Role.MemberId = mem.MemberId;
                mem_Role.RoleId = 1;
                _unitOfWork.GetRepositoryInstance<MemberRole>().Add(mem_Role);

                TempData["VerificationLinlMsg"] = "Registered successfully.";
                //Session["MemberId"] = mem.MemberId;
               // Response.Cookies["MemberName"].Value = mem.FirstName;
               // Response.Cookies["MemberRole"].Value = "Admin";
                return RedirectToAction("Dashboard");
            }
            return View( model);
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ChangePassword(ChangePasswordViewModel cpm)
        {
            int LoginMemberId = Convert.ToInt32(Session["MemberId"]);
            var ExistingDetails = _unitOfWork.GetRepositoryInstance<Member>().GetFirstOrDefaultByParameter(i => i.MemberId == LoginMemberId && i.IsActive == true && i.IsDelete == false);
            if (EncryptDecrypt.Encrypt(cpm.OldPassword, true) == ExistingDetails.Password)
            {
                ExistingDetails.Password = EncryptDecrypt.Encrypt(cpm.NewPassword, true);
                _unitOfWork.SaveChanges();
                ViewBag.PasswordChangeMsg = "Password changed successfully";
            }
            else
                ModelState.AddModelError("OldPassword", "Current password is wrong");
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

        public ActionResult AddCategory()
        {
            return UpdateCategory(0);
        }

        public ActionResult UpdateCategory(int categoryId)
        {
            CategoryDetail cd;
            if (categoryId != 0)
                cd =
                cd = JsonConvert.DeserializeObject<CategoryDetail>(JsonConvert.SerializeObject(_unitOfWork.GetRepositoryInstance<Category>().GetFirstOrDefault(categoryId)));
            else
                cd = new CategoryDetail();
            return View("UpdateCategory", cd);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCategory(CategoryDetail cd)
        {
            if (ModelState.IsValid)
            {
                Category cat = _unitOfWork.GetRepositoryInstance<Category>().GetFirstOrDefault(cd.CategoryId);
                cat = cat != null ? cat : new Category();
                cat.CategoryName = cd.CategoryName;
                if (cd.CategoryId != 0)
                {
                    _unitOfWork.GetRepositoryInstance<Category>().Update(cat);
                    _unitOfWork.SaveChanges();
                }
                else
                {
                    cat.IsActive = true;
                    cat.IsDelete = false;
                    _unitOfWork.GetRepositoryInstance<Category>().Add(cat);
                }

                return RedirectToAction("Categories");
            }
            else
                return View("UpdateCategory", cd);
        }


        /// <summary>
        /// Delete Category
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public int DeleteCategory(int itemId)
        {
            _unitOfWork.GetRepositoryInstance<Category>().InactiveAndDeleteMarkByWhereClause(i => i.CategoryId == itemId, (u => { u.IsActive = false; u.IsDelete = true; }));
            return 1;
        }

        /// <summary>
        /// Check Category Exist
        /// </summary>
        /// <param name="CategoryName"></param>
        /// <returns></returns>
        public JsonResult CheckCategoryExist(string CategoryName)
        {
            int CategoryId = 0;
            if (HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["categoryId"] != null)
                CategoryId = Convert.ToInt32(HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["categoryId"]);
            var CategoryExist = _unitOfWork.GetRepositoryInstance<Category>().GetAllRecordsIQueryable().Where(i => i.CategoryName == CategoryName && i.CategoryId != CategoryId && i.IsActive == true && i.IsDelete == false).Count();
            return CategoryExist == 0 ? Json(true, JsonRequestBehavior.AllowGet) : Json(false, JsonRequestBehavior.AllowGet);
        }


        #region Manage Products...
        #region Product Listing...
        /// <summary>
        /// Product Listing
        /// </summary>
        /// <returns></returns>
        public ActionResult Products()
        {
            List<Product> products = _unitOfWork.GetRepositoryInstance<Product>().GetAllRecordsIQueryable().Where(i => i.IsDelete == false).ToList();
            return View(products);
        }
        #endregion

        #region Add/Update Product...
        /// <summary>
        /// Add Product
        /// </summary>
        /// <returns></returns>
        public ActionResult AddProduct()
        {
            return UpdateProduct(0);
        }

        /// <summary>
        /// Update Product
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public ActionResult UpdateProduct(int productId)
        {
            ProductDetail pd = _unitOfWork.GetRepositoryInstance<Product>().GetListByParameter(i => i.ProductId == productId).Select(j => new ProductDetail { CategoryId = j.CategoryId, Description = j.Description, IsActive = j.IsActive ?? default(bool), Price = j.Price ?? default(decimal), ProductId = j.ProductId, ProductImage = j.ProductImage, ProductName = j.ProductName, IsFeatured = j.IsFeatured ?? default(bool) }).FirstOrDefault();
            pd = pd != null ? pd : new ProductDetail();
            pd.Categories = new SelectList(_unitOfWork.GetRepositoryInstance<Category>().GetAllRecordsIQueryable(), "CategoryId", "CategoryName");
            return View("UpdateProduct", pd);
        }

        /// <summary>
        /// Updating Product details to DB
        /// </summary>
        /// <param name="pd"></param>
        /// <param name="_ProductImage"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProduct(ProductDetail pd, HttpPostedFileBase _ProductImage)
        {
            if (ModelState.IsValid)
            {
                Product prod = _unitOfWork.GetRepositoryInstance<Product>().GetFirstOrDefault(pd.ProductId);
                prod = prod != null ? prod : new Product();
                prod.CategoryId = pd.CategoryId;
                prod.Description = pd.Description;
                prod.IsActive = pd.IsActive;
                prod.IsFeatured = pd.IsFeatured;
                prod.Price = pd.Price;
                prod.ProductImage = _ProductImage != null ? _ProductImage.FileName : prod.ProductImage;
                prod.ProductName = pd.ProductName;
                prod.ModifiedDate = DateTime.Now;
                if (prod.ProductId == 0)
                {
                    prod.CreatedDate = DateTime.Now;
                    prod.IsDelete = false;
                    _unitOfWork.GetRepositoryInstance<Product>().Add(prod);
                }
                else
                {
                    _unitOfWork.GetRepositoryInstance<Product>().Update(prod);
                    _unitOfWork.SaveChanges();
                }
                if (_ProductImage != null)
                    uc.UploadImage(_ProductImage, prod.ProductId + "_", "/Content/ProductImage/", Server, _unitOfWork, 0, prod.ProductId, 0);
                return RedirectToAction("Products");
            }
            pd.Categories = new SelectList(_unitOfWork.GetRepositoryInstance<Category>().GetAllRecordsIQueryable(), "CategoryId", "CategoryName");
            return View("UpdateProduct", pd);
        }

        /// <summary>
        /// Check Product Exist
        /// </summary>
        /// <param name="ProductName"></param>
        /// <returns></returns>
        public JsonResult CheckProductExist(string ProductName)
        {
            int productId = 0;
            if (HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["productId"] != null)
                productId = Convert.ToInt32(HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["productId"]);
            var productExist = _unitOfWork.GetRepositoryInstance<Product>().GetAllRecordsIQueryable().Where(i => i.ProductName == ProductName && i.ProductId != productId && i.IsActive == true && i.IsDelete == false).Count();
            return productExist == 0 ? Json(true, JsonRequestBehavior.AllowGet) : Json(false, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion

        #region Product Details...
        /// <summary>
        /// Product Detail
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public ActionResult ProductDetail(int productId)
        {
            Product pd = _unitOfWork.GetRepositoryInstance<Product>().GetFirstOrDefault(productId);
            return View(pd);
        }
        #endregion

        /// <summary>
        /// All orders
        /// </summary>
        /// <returns></returns>
        public ActionResult Orders()
        {
            List<Cart> AllOrders = _unitOfWork.GetRepositoryInstance<Cart>().GetListByParameter(i => i.CartStatusId == 3).ToList();
            return View(AllOrders);
        }

        /// <summary>
        /// Order details of a product
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public ActionResult OrderDetail(int productId)
        {
            List<Cart> ProductOrders = _unitOfWork.GetRepositoryInstance<Cart>().GetListByParameter(i => i.CartStatusId == 3 && i.ProductId == productId).ToList();
            return View(ProductOrders);
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