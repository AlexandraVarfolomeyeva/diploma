using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace diploma.Models
{
    public class User : IdentityUser
    {
       
        public string Fio { get; set; }
        public string Address { get; set; }
        public int IdCity { get; set; }
        public virtual City City { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public User()
        {
            Orders = new HashSet<Order>();
        }
    }
}
