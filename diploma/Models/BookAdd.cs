using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace diploma.Models
{
    public class BookAdd
    {
        public int Id { get; set; }
        public string image { get; set; } //url картинки
        public string Year { get; set; } //год публикации
        public int Cost { get; set; } //стоимость
        public int Stored { get; set; } //есть ли на складе
        public string Content { get; set; } //Описание (аннотация) книги
        public string Title { get; set; } //Название книги
        public int Publisher { get; set; }
        public bool isDeleted  { get; set; }
        public int[] idAuthors { get; set; }
        public string[] Authors { get; set; }
        public int[] idGenres { get; set; }
        public string[] Genres { get; set; }
}
}
