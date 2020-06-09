using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Models
{
    public class BookAdd
    {
        public int Id { get; set; }
        public string image { get; set; } //url картинки
        public string Year { get; set; } //год публикации
        public int Cost { get; set; } //стоимость
        public int Stored { get; set; } //есть ли на складе
        public int Weight { get; set; }
        public string Content { get; set; } //Описание (аннотация) книги
        [Required]
        public string Title { get; set; } //Название книги
        public int Publisher { get; set; }
        public bool isDeleted  { get; set; }
        public int[] idAuthors { get; set; }
        public string[] Authors { get; set; }
        public int[] idGenres { get; set; }
        public string[] Genres { get; set; }
}
}
