using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BLL.Models
{
    public class AddressModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Не указан адрес!")]
        public string Name { get; set; }
        public int IdCity { get; set; }
        public virtual CityModel City { get; set; }
        public string IdUser { get; set; }
        public virtual UserModel User { get; set; }
        public virtual ICollection<OrderModel> Orders { get; set; }
        public bool isDeleted { get; set; }
        public AddressModel()
        {
            Orders = new HashSet<OrderModel>();
        }
    }
}
