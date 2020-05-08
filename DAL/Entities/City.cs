using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Entities
{
    [Table("City")]
    public class City
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public float DeliverySum { get; set; } //стоимость доставки
        public int DeliveryTime { get; set; } //time доставки
        public virtual ICollection<User> Users { get; set; }
    }
}
