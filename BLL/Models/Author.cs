using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using BLL.Models;

namespace BLL.Models
{
    public class AuthorModel
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public virtual IEnumerable<BookAuthorModel> BookAuthors { get; set; }
    }
}
