using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Models
{
    public class BookAuthorModel
    {
        [Required]
        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey("Book")]
        public int IdBook { get; set; }
        [Required]
        [ForeignKey("Author")]
        public int IdAuthor { get; set; }
        public virtual BookModel Book { get; set; }
        public virtual AuthorModel Author { get; set; }
    }
}
