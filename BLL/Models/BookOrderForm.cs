using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Models
{
    public class BookOrderForm
    {
        public int IdBook { get; set; }
        public int IdOrder { get; set; }
        public int Amount { get; set; } //количество книг в заказе
    }
}
