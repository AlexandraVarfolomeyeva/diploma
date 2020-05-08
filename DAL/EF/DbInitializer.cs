using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities;

namespace DAL.EF
{
    public static class DbInitializer
    {
        public static void Initialize(BookingContext context)
        {
            context.Database.EnsureCreated();
            if (!context.City.Any())
            {
                var Cities = new City[]
                {
                    new City {Name = "Москва", DeliverySum=150, DeliveryTime=10},
                    new City {Name = "Санкт-Петербург", DeliverySum=150, DeliveryTime=10},
                    new City {Name = "Новосибирск", DeliverySum=150, DeliveryTime=10},
                    new City {Name = "Екатеринбург", DeliverySum=150, DeliveryTime=10},
                    new City {Name = "Нижний Новгород", DeliverySum=150, DeliveryTime=10},
                    new City {Name = "Казань", DeliverySum=150, DeliveryTime=10},
                    new City {Name = "Самара", DeliverySum=150, DeliveryTime=10},
                    new City {Name = "Омск", DeliverySum=150, DeliveryTime=10},
                    new City {Name = "Челябинск", DeliverySum=150, DeliveryTime=10},
                    new City {Name = "Ростов-на-Дону", DeliverySum=150, DeliveryTime=10},
                    new City {Name = "Уфа", DeliverySum=150, DeliveryTime=10},
                    new City {Name = "Волгоград", DeliverySum=150, DeliveryTime=10},
                    new City {Name = "Пермь", DeliverySum=150, DeliveryTime=10},
                    new City {Name = "Красноярск", DeliverySum=150, DeliveryTime=10},
                    new City {Name = "Воронеж", DeliverySum=150, DeliveryTime=10},
                    new City {Name = "Саратов", DeliverySum=150, DeliveryTime=10},
                    new City {Name = "Краснодар", DeliverySum=150, DeliveryTime=10},
                    new City {Name = "Тольятти", DeliverySum=150, DeliveryTime=10},
                    new City {Name = "Барнаул", DeliverySum=150, DeliveryTime=10},
                    new City {Name = "Ижевск", DeliverySum=150, DeliveryTime=10},
                    new City {Name = "Ульяновск", DeliverySum=150, DeliveryTime=10}
                };
                foreach (City b in Cities)
                {
                    context.City.Add(b);
                }
                context.SaveChanges();
            }


            if (!context.Genre.Any())
            {
                var Genres = new Genre[]
                {
                    new Genre {Name="Боевики"},
                    new Genre {Name="Детективы"},
                    new Genre {Name="Детские"},
                    new Genre {Name="Документальные"},
                    new Genre {Name="Дом и семья"},
                    new Genre {Name = "Компьютер и интернет"},
                    new Genre {Name = "Любовные романы"},
                    new Genre {Name = "Научно-образовательные"},
                    new Genre {Name = "Поэзия и драматургия"},
                    new Genre {Name = "Приключения"},
                    new Genre {Name = "Проза"},
                    new Genre {Name = "Религия и духовность"},
                    new Genre {Name = "Современная литература"},
                    new Genre {Name = "Справочная литература"},
                    new Genre {Name = "Старинная литература"},
                    new Genre {Name = "Юмор"},
                    new Genre {Name = "Эротика"},
                    new Genre {Name = "Экономика"},
                    new Genre {Name = "Фантастика"},
                    new Genre {Name = "Психология"},
                    new Genre {Name = "Бизнес"},
                    new Genre {Name = "Лингвистика"},
                    new Genre {Name = "Научная фантастика"},
                    new Genre {Name = "Ужасы"},
                    new Genre {Name = "Триллер"},
                    new Genre {Name = "Политика"}
                };
                foreach (Genre b in Genres)
                {
                    context.Genre.Add(b);
                }
                context.SaveChanges();
            }
        }
    }
}
