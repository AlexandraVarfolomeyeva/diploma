using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace diploma.Models
{
    public class Comment
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
        public int IdBook { get; set; }
        public virtual Book Book { get; set; }
    }
}
