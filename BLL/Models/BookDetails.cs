using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Models
{
    public class BookDetails
    {
        public BookAdd Book;
        public OrderModel CurrentOrder;
        public IEnumerable<CommentModel> Comments;
    }
}
