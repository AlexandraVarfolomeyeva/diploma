using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Models
{
    public class CityModel
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public float DeliverySum { get; set; } //стоимость доставки
        public int DeliveryTime { get; set; } //time доставки
        public virtual ICollection<UserModel> Users { get; set; }
    }
}
