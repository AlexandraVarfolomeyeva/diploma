using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Entities
{
    [Table("Comment")]
    public class Comment
    {
        [Required]
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime DateComment { get; set; }
        [Required]
        public int IdBook { get; set; }
     
        public virtual Book Book { get; set; }
    }
}
