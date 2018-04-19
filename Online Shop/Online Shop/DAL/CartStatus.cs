using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Online_Shop.DAL
{
    public class CartStatus
    {
        public int CartStatusId { get; set; }
        public string Cartstatus { get; set; }

        
        public virtual ICollection<Cart> Cart { get; set; }
    }
}