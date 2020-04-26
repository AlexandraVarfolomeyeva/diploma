using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using diploma.Models;

namespace diploma.Data
{
    public static class DbInitializer
    {
        public static void Initialize(BookingContext context)
        {
            context.Database.EnsureCreated();
            if (!context.User.Any())
            {
                var Cities = new City[]
                {
                    new City {Name = "Москва", DeliverySum=150},
                    new City {Name = "Санкт-Петербург", DeliverySum=150},
                    new City {Name = "Новосибирск", DeliverySum=150},
                    new City {Name = "Екатеринбург", DeliverySum=150},
                    new City {Name = "Нижний Новгород", DeliverySum=150},
                    new City {Name = "Казань", DeliverySum=150},
                    new City {Name = "Самара", DeliverySum=150},
                    new City {Name = "Омск", DeliverySum=150}
                };
                foreach (City b in Cities)
                {
                    context.City.Add(b);
                }
                context.SaveChanges();
            }
            //if (!context.User.Any())
            //{
            //    var users = new User[]
            //    {
            //    //new User {Url="http://diploma.msdn.com/donald", Address="Lake", Login="donald", Name="donaldduck", Password="1234", Phone="123409"},
            //    //new User {Url="http://diploma.msdn.com/mikky", Address="hole", Login="mikky", Name="mikkymouse", Password="4567", Phone="456787"},
            //    //new User {Url="http://diploma.msdn.com/miny", Address="hole", Login="miny", Name="minymouse", Password="7890", Phone="789065"}

            //    };
            //    foreach (User b in users)
            //    {
            //        context.User.Add(b);
            //    }
            //    context.SaveChanges();
            //}

            //if (!context.Order.Any())
            //{
            //    var orders = new Order[]
            //{
            //    //new Order { UserId ="1", SumOrder=498, SumDelivery=129, User=context.User.FirstOrDefault()},
            //    //new Order { UserId ="1", SumOrder=5679, SumDelivery=56, User=context.User.FirstOrDefault()}
            //};
            //    foreach (Order p in orders)
            //    {
            //        context.Order.Add(p);
            //    }
            //    context.SaveChanges();
            //}



            //if (!context.BookOrder.Any())
            //{
            //    var bookOrders = new BookOrder[]
            //{
            //    new BookOrder { IdOrder=1, IdBook=1, Order=context.Order.FirstOrDefault(), Book=context.Book.FirstOrDefault()},
            //    new BookOrder { IdOrder=2, IdBook=2,  Order=context.Order.LastOrDefault(), Book=context.Book.LastOrDefault()}
            //};
            //    foreach (BookOrder p in bookOrders)
            //    {
            //        context.BookOrder.Add(p);
            //    }
            //    context.SaveChanges();
            //}


            //if (!context.Book.Any())
            //{
            //    var books = new Book[]
            //{
            //    new Book { Content="\"Если даже такая нищебродская тушка, как я сумела поправить свое финансовое положение, сможет кто угодно!\" - заявляет Джен Синсеро...",Title="НЕ НОЙ. Вековая мудрость, которая гласит: хватит жаловаться – пора становиться богатым", Cost = 477, Year="2019", Publisher=""},
            //    new Book { Content="Какова бы ни была ваша цель - существенно изменить лишь некоторые ключевые аспекты своей жизни или радикально преобразовать все свое существование, чтобы все, с чем вам приходится бороться сейчас, стало лишь неприятным воспоминанием, - вы выбрали правильную книгу... ",Title="Магия утра. Как первый час дня определяет ваш успех", Cost = 828, Year="2019", Publisher=""}
            //};
            //    foreach (Book p in books)
            //    {
            //        context.Book.Add(p);
            //    }
            //    context.SaveChanges();
            //}
        }
    }
}
