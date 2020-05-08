using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Entities
{
    [Table("Order")]
    public  class Order
    {

        public Order()
        {
            BookOrders = new HashSet<BookOrder>();
        }
        [Required]
        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        public DateTime DateDelivery { get; set; }//дата доставки
        public DateTime? DateSubmit { get; set; }//дата подтверждения
        public DateTime? DateSent { get; set; }//дата отправки
        public DateTime? DateCancel { get; set; }//дата отмены заказа
        public DateTime DateOrder { get; set; }//дата заказа
        public float SumOrder { get; set; } //стоимость заказа
        public int Weight { get; set; } //вес в грамах
        public float SumDelivery { get; set; } //стоимость доставки
        public int Active { get; set; } //является ли заказ активным
        public int Amount { get; set; } //количество книг в заказе
        //public string Url { get; set; }
        public virtual ICollection<BookOrder> BookOrders { get; set; }
        public virtual User User { get; set; }
    }
}
