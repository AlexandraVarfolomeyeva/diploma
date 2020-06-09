using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Models
{
    public class UserModel : IdentityUser
    {
        public string Fio { get; set; }
        public int Discount { get; set; }
        public virtual ICollection<OrderModel> Orders { get; set; }
        public virtual ICollection<AddressModel> Addresses { get; set; }
        public UserModel()
        {
            Orders = new HashSet<OrderModel>();
            Addresses = new HashSet<AddressModel>();
        }
    }
}
