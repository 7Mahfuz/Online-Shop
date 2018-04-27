using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Online_Shop.Models
{
    public class ShippingDetail
    {
        public string OrderId { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
         
        public string Country { get; set; }
        [Required]
        public string ZipCode { get; set; }
        public decimal TotalPrice { get; set; }
        
       
    }
}