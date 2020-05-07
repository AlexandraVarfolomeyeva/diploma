using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace diploma.Models
{
    public class BookDetails
    {
        public BookAdd Book;
        public Order CurrentOrder;
        public IEnumerable<Comment> Comments;
    }
}
