using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    [Table("Book")]
    public class Book
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public string image { get; set; } //url картинки
        public string Year { get; set; } //год публикации
        public int Cost { get; set; } //стоимость
        public int Stored {get;set;} //количество на складе
        public int Score { get; set; } //рейтинг книги
        public int Rated { get; set; } //человек оценили
        public int Weight { get; set; } //вес в грамах
        public bool isDeleted { get; set; }
        public string Content { get; set; } //Описание (аннотация) книги
        [Required]
        public string Title { get; set; } //Название книги
        public virtual int IdPublisher { get; set; }
        public virtual Publisher Publisher { get; set; }
        public virtual ICollection<BookOrder> BookOrders { get; set; }
        public virtual ICollection<BookAuthor> BookAuthors { get; set; }
        public virtual ICollection<BookGenre> BookGenres { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public Book()
        {
            BookOrders = new HashSet<BookOrder>();
            BookAuthors = new HashSet<BookAuthor>();
            BookGenres = new HashSet<BookGenre>();
        }
    }
}
