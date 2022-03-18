using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Entities
{
    [Table("BookAuthor")]
    public class BookAuthor
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
        public virtual Book Book { get; set; }
        public virtual Author Author { get; set; }
    }
}
