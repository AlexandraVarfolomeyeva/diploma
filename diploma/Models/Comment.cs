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
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public int IdBook { get; set; }
        public virtual Book Book { get; set; }
    }
}
