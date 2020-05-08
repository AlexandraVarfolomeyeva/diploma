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
        public string Address { get; set; }
        public int IdCity { get; set; }
        public virtual CityModel City { get; set; }
        public virtual ICollection<OrderModel> Orders { get; set; }
        public UserModel()
        {
            Orders = new HashSet<OrderModel>();
        }
    }
}
