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
        [RegularExpression("[A-Za-zА-Яа-я- ]{2,}", ErrorMessage = "Пожалуйста, введите название города, избегая цифр и знаков.")]
        [Required(ErrorMessage = "Не указано название города")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Не указана стоимость доставки")]
        public float DeliverySum { get; set; } //стоимость доставки
        [Required(ErrorMessage = "Не указано время доставки")]
        public int DeliveryTime { get; set; } //time доставки
        public virtual IEnumerable<UserModel> Users { get; set; }
        public virtual ICollection<AddressModel> Addresses { get; set; }
        public CityModel()
        {
            Addresses = new HashSet<AddressModel>();
        }
    }
}
