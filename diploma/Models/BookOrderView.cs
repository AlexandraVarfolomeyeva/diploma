using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace diploma.Models
{
    public class BookOrderView
    {
        public int Id;
        public int Amount;
        public BookView Book { get; set; }
    }
}
