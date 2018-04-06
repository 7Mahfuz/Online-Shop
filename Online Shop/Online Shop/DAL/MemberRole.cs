using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Online_Shop.DAL
{
    public class MemberRole
    {
        [Key]
        public int MemberRoleId { get; set; }
        public Nullable<int> MemberId { get; set; }
        public Nullable<int> RoleId { get; set; }

        public virtual Roles Roles { get; set; }
    }
}