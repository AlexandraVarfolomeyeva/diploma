using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories
{
    public class DBReposSQL : IdbRepos
    {
        private BookingContext _context;

        private BookRepo bookRepo;
        private AuthorRepo authorRepo;
        private BookAuthorRepo bookAuthorRepo;
        private BookGenreRepo bookGenreRepo;
        private BookOrderRepo bookOrderRepo;
        private CityRepo cityRepo;
        private CommentRepo commentRepo;
        private GenreRepo genreRepo;
        private OrderRepo orderRepo;
        private PublisherRepo publisherRepo;
        private UserRepo userRepo;
        private AddressRepo addressRepo;

        public DBReposSQL(BookingContext context)
        {
            _context=context;
        }
        //public DBReposSQL()
        //{
        //    _context = new BookingContext();
        //}

        public IRepository<Book> Books
        {
            get
            {
                if (bookRepo == null)
                    bookRepo = new BookRepo(_context);
                return bookRepo;
            }
        }

        public IRepository<Author> Authors
        {
            get
            {
                if (authorRepo == null)
                    authorRepo = new AuthorRepo(_context);
                return authorRepo;
            }
        }

        public IRepository<BookAuthor> BookAuthors
        {
            get
            {
                if (bookAuthorRepo == null)
                    bookAuthorRepo = new BookAuthorRepo(_context);
                return bookAuthorRepo;
            }
        }

        public IRepository<BookGenre> BookGenres
        {
            get
            {
                if (bookGenreRepo == null)
                    bookGenreRepo = new BookGenreRepo(_context);
                return bookGenreRepo;
            }
        }

        public IRepository<BookOrder> BookOrders
        {
            get
            {
                if (bookOrderRepo == null)
                    bookOrderRepo = new BookOrderRepo(_context);
                return bookOrderRepo;
            }
        }

        public IRepository<City> Cities
        {
            get
            {
                if (cityRepo == null)
                    cityRepo = new CityRepo(_context);
                return cityRepo;
            }
        }

        public IRepository<Address> Addresses
        {
            get
            {
                if (addressRepo == null)
                    addressRepo = new AddressRepo(_context);
                return addressRepo;
            }
        }

        public IRepository<Comment> Comments
        {
            get
            {
                if (commentRepo == null)
                    commentRepo = new CommentRepo(_context);
                return commentRepo;
            }
        }

        public IRepository<Genre> Genres
        {
            get
            {
                if (genreRepo == null)
                    genreRepo = new GenreRepo(_context);
                return genreRepo;
            }
        }

        public IRepository<Order> Orders
        {
            get
            {
                if (orderRepo == null)
                    orderRepo = new OrderRepo(_context);
                return orderRepo;
            }
        }

        public IRepository<Publisher> Publishers
        {
            get
            {
                if (publisherRepo == null)
                    publisherRepo = new PublisherRepo(_context);
                return publisherRepo;
            }
        }

        public IRepositoryUser Users
        {
            get
            {
                if (userRepo == null)
                    userRepo = new UserRepo(_context);
                return userRepo;
            }
        }

        public int Save()
        { 
            return _context.SaveChanges();
        }

    }
}
