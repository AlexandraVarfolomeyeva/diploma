using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Models
{
    public class BookOrderView
    {
        public int Id;
        public int Amount;
        public float Price;
        public BookView Book { get; set; }
    }
}
