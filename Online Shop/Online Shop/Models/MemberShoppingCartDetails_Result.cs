using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Online_Shop.Models
{
    public class MemberShoppingCartDetails_Result
    {
        public int CartId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string CategoryName { get; set; }
    }
}