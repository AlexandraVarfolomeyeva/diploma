using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace BLL.Models
{
    public class BookListViewModel
    {
        public int page;
        public string searchString;
        public string sortOrder;
        public bool Stored;
        public int Genre;
        public string AuthorSearch;
        public IPagedList<BookView> Books;
        public string UserName;
        public OrderModel CurrentOrder;
    }
}
