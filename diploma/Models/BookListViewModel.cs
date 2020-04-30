using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace diploma.Models
{
    public class BookListViewModel
    {
        public IEnumerable<BookView> Books;
        public string UserName;
        public Order CurrentOrder;
    }
}
