using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    [Table("Address")]
    public class Address
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int IdCity { get; set; }
         public bool isDeleted { get; set; }
        public string IdUser { get; set; }
        public virtual User User { get; set; }
        public virtual City City { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
       
        public Address()
        {
            Orders = new HashSet<Order>();
        }
    }
}
