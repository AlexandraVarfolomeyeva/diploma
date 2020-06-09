using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Models
{
   
    public  class BookOrderModel //строка заказа
    {
        [Required]
        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey("Book")]
        public int IdBook { get; set; }
        [Required]
        [ForeignKey("Order")]
        public int IdOrder { get; set; }
        public int Amount { get; set; } //количество книг в заказе
        public float Price { get; set; } //цена одной книги
        public virtual BookModel Book { get; set; }
        public virtual OrderModel Order { get; set; }
    }
}
