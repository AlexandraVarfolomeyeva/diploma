using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Models
{
    public class AddressView
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public int IdCity { get; set; }
        public string City { get; set; }
        public string IdUser { get; set; }
        public float DeliverySum { get; set; } //стоимость доставки
        public int DeliveryTime { get; set; } //time доставки
    }
}
