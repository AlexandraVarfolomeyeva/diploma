using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Interfaces
{
   public interface IdbRepos
    {
        //Unit of Work -- паттерн
        IRepository<Book> Books { get; }
        IRepository<BookOrder> BookOrders { get; }
        IRepository<BookAuthor> BookAuthors { get; }
        IRepository<BookGenre> BookGenres { get; }
        IRepository<Genre> Genres { get; }
        IRepository<Order> Orders { get; }
        IRepository<Author> Authors { get; }
        IRepository<Comment> Comments { get; }
        IRepository<Publisher> Publishers { get; }
        IRepositoryUser Users { get; }
        IRepository<City> Cities { get; }
        IRepository<Address> Addresses { get; }

        int Save();
    }
}
