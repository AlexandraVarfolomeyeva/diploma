using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Models
{
    public class AdminOrderView
    {
        public int Id { get; set; }
        public DateTime DateDelivery { get; set; }//дата доставки
        public DateTime DateOrder { get; set; }//дата заказа
        public float SumOrder { get; set; } //стоимость заказа
        public int Active { get; set; } //является ли заказ активным
        public int Amount { get; set; } //количество книг в заказе
        public string City { get; set; } //адрес доставки
        public string Address { get; set; } //адрес доставки
        public string FIO { get; set; } //ФИО
        public string Phone { get; set; } //номер телефона для связи
        public string Email { get; set; } //Email для саязи
        public float SumDelivery { get; set; } //стоимость доставки
        public IEnumerable<BookOrderView> BookOrders { get; set; }
    }
}
