using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class User : IdentityUser
    {
       
        public string Fio { get; set; }
        public int Discount { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        public User()
        {
            Orders = new HashSet<Order>();
            Addresses = new HashSet<Address>();
        }
    }
}
