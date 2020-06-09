using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Entities
{
    [Table("BookOrder")]
    public  class BookOrder //строка заказа
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
        public virtual Book Book { get; set; }
        public virtual Order Order { get; set; }
    }
}
