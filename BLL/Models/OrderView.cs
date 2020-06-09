using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Models
{
    public class OrderView
    {
        public int Id { get; set; }
        public DateTime DateDelivery { get; set; }//дата доставки
        public DateTime DateOrder { get; set; }//дата заказа
        public float SumOrder { get; set; } //стоимость заказа
        public float SumDelivery { get; set; } //стоимость доставки
        public int Active { get; set; } //является ли заказ активным
        public int Amount { get; set; } //количество книг в заказе
        public int Weight { get; set; }
        public AddressView Address { get; set; }
        public IEnumerable<BookOrderView> BookOrders { get; set; }
    }
}
