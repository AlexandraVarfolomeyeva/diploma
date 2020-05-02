using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace diploma.Models
{
    public class BookListViewModel
    {
        public int page;
        public string searchString;
        public string sortOrder;
        public bool Stored;
        public int Genre;
        public IPagedList<BookView> Books;
        public string UserName;
        public Order CurrentOrder;
    }
}
