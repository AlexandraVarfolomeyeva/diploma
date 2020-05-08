using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Models
{
    public class BookView
    {
        public int Id { get; set; }
        public string image { get; set; } //url картинки
        public string Year { get; set; } //год публикации
        public int Cost { get; set; } //стоимость
        public int Stored { get; set; } //есть ли на складе
        public string Content { get; set; } //Описание (аннотация) книги
        public string Title { get; set; } //Название книги
        public string Publisher { get; set; }
        public int Score { get; set; } //рейтинг книги
        public int Rated { get; set; } //человек оценили
        public int Weight { get; set; } //вес в грамах
        public string[] Genres { get; set; }
        public string[] Authors { get; set; }
    }
}
