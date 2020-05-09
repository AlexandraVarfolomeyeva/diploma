using BLL.Interfaces;
using BLL.Models;
using DAL;
using DAL.Entities;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Operations
{
    //паттерн репозиторий
    //Unit of work
    public class DBCrud : IDBCrud
    {

        IdbRepos db;
        public DBCrud(IdbRepos repos)
        {
            db = repos;
        }

        #region Book methods
        public IEnumerable<BookModel> GetAllBooks()
        {
            return db.Books.GetList().Select(f => toBookModel(f));
        }

        public BookModel GetBookModel(int id)
        {
            return toBookModel(db.Books.GetItem(id));
        }

        public void CreateBook(BookAdd item)
        {
            BookModel book = new BookModel()
            {
                Year = item.Year,
                Title = item.Title,
                Stored = item.Stored,
                image = item.image,
                IdPublisher = item.Publisher,
                Cost = item.Cost,
                Weight = item.Weight,
                Content = item.Content,
                isDeleted = item.isDeleted
            };
            db.Books.Create(toBook(book, new Book()));

            for (int i = 0; i < item.idAuthors.Length; i++)
            {
                BookAuthor bookauthor = new BookAuthor()
                {
                    IdAuthor = item.idAuthors[i],
                    IdBook = book.Id
                };
                db.BookAuthors.Create(bookauthor);
            }
            for (int i = 0; i < item.idGenres.Length; i++)
            {
                BookGenre bookgenre = new BookGenre()
                {
                    IdGenre = item.idGenres[i],
                    IdBook = book.Id
                };
                db.BookGenres.Create(bookgenre);
            }
            db.Save();
            db.Books.GetList();
        }

        public void DeleteBook(int id)
        {
            try
            {
                Book item = db.Books.GetItem(id);
                if (item == null)
                {
                    Log.WriteSuccess(" BooksController.Delete", "Книга не найдена.");
                }
                //_context.Book.Remove(item);
                IEnumerable<BookOrder> lines = db.BookOrders.GetList().Where(l => l.IdBook == id);
                foreach (BookOrder i in lines)
                {
                    Order order = db.Orders.GetItem(i.IdOrder);
                    if (order.Active == 1)
                    {
                        order.Amount -= i.Amount;
                        order.SumOrder -= item.Cost * i.Amount;
                        db.Orders.Update(order);
                        db.BookOrders.Delete(i.Id);
                    }
                }
                item.isDeleted = true;
                db.Books.Update(item);
                 db.Save();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
    }

        public IEnumerable<BookView> GetAllBookViews()
        {
            IEnumerable<Book> books = db.Books.GetList();
            BookView[] bookViews = new BookView[books.Count()];
            int i = 0;
            foreach (Book item in books)
            {
                bookViews[i] = new BookView()
                {
                    Id = item.Id,
                    image = item.image,
                    Stored = item.Stored,
                    Title = item.Title,
                    Year = item.Year,
                    Cost = item.Cost,
                    Content = item.Content
                };
                Publisher publisher = db.Publishers.GetItem(item.IdPublisher);
                List<string> au = new List<string>();
                List<string> ge = new List<string>();
                bookViews[i].Publisher = publisher.Name;
                IEnumerable<BookAuthor> bookauthors = db.BookAuthors.GetList().Where(b => b.IdBook == item.Id);
                IEnumerable<BookGenre> bookgenres = db.BookGenres.GetList().Where(b => b.IdBook == item.Id);
                foreach (BookAuthor line in bookauthors)
                {
                    Author author = db.Authors.GetItem(line.IdAuthor);
                    au.Add(author.Name);
                }
                bookViews[i].Authors = au.ToArray();
                foreach (BookGenre line in bookgenres)
                {
                    Genre genre = db.Genres.GetItem(line.IdGenre);
                    ge.Add(genre.Name);
                }
                bookViews[i].Genres = ge.ToArray();
                i++;
            }
            IEnumerable<BookView> views = bookViews;
            return views;
        }

        public BookAdd GetBook(int id)
        {
            try
            {
                Book item = db.Books.GetItem(id);

                if (item == null)
                {
                    Log.WriteSuccess("DBCrud GetBook", "Книга не найдена.");
                    return null;
                }
                BookAdd b = new BookAdd()
                {
                    Id = item.Id,
                    Content = item.Content,
                    isDeleted = item.isDeleted,
                    Cost = item.Cost,
                    image = item.image,
                    Publisher = item.IdPublisher,
                    Stored = item.Stored,
                    Title = item.Title,
                    Weight = item.Weight,
                    Year = item.Year
                };
                List<int> au = new List<int>();
                List<string> aus = new List<string>();
                List<int> ge = new List<int>();
                List<string> ges = new List<string>();
                //IEnumerable<BookAuthor> bookauthors = db.BookAuthors.GetList().Where(f => f.IdBook == item.Id);
                //IEnumerable<BookGenre> bookgenres = db.BookGenres.GetList().Where(f => f.IdBook == item.Id);
                foreach (BookAuthor line in item.BookAuthors)
                {
                    Author author = line.Author;
                    //db.Authors.GetItem(line.IdAuthor);
                    au.Add(author.Id);
                    aus.Add(author.Name);
                }
                b.idAuthors = au.ToArray();
                b.Authors = aus.ToArray();
                foreach (BookGenre line in item.BookGenres)
                {
                    Genre genre = line.Genre;
                    //_context.Genre.Find(line.IdGenre);
                    ge.Add(genre.Id);
                    ges.Add(genre.Name);
                }
                b.idGenres = ge.ToArray();
                b.Genres = ges.ToArray();
                return b;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
        }

        public void UpdateBook(BookAdd book)
        {
            try
            {
                Book item = db.Books.GetItem(book.Id);
                if (item == null)
                {
                    Log.WriteSuccess("DBCrud UpdateBook", "Книга не найдена.");
                }
                item.Content = book.Content;
                item.Cost = book.Cost;
                item.image = book.image;
                item.Stored = book.Stored;
                item.Title = book.Title;
                item.Year = book.Year;
                item.Weight = book.Weight;
                item.isDeleted = book.isDeleted;
                item.IdPublisher = book.Publisher;

                IEnumerable<BookAuthor> bookauthors = db.BookAuthors.GetList().Where(b => b.IdBook == item.Id);
                IEnumerable<BookGenre> bookgenres = db.BookGenres.GetList().Where(b => b.IdBook == item.Id);
                foreach (BookAuthor line in bookauthors)
                {
                    IEnumerable<int> el = book.idAuthors.Where(a => a == line.IdAuthor);
                    if (el.Any())//found the same 
                    {
                        book.idAuthors = book.idAuthors.Where(val => val != el.FirstOrDefault()).ToArray();
                    }
                    else
                    {
                        db.BookAuthors.Delete(line.Id);
                    }
                }
                while (book.idAuthors.Any())//found new 
                {
                    int el = book.idAuthors[0];
                    BookAuthor z = new BookAuthor()
                    {
                        IdBook = book.Id,
                        IdAuthor = el
                    };
                    db.BookAuthors.Create(z);
                    book.idAuthors = book.idAuthors.Where(val => val != el).ToArray();
                }

                foreach (BookGenre line in bookgenres)
                {
                    IEnumerable<int> el = book.idGenres.Where(a => a == line.IdGenre);
                    if (el.Any())//found the same 
                    {
                        book.idGenres = book.idGenres.Where(val => val != el.FirstOrDefault()).ToArray();
                    }
                    else
                    {
                        db.BookGenres.Delete(line.Id);
                    }
                }

                while (book.idGenres.Any())//found the new 
                {
                    int el = book.idGenres[0];
                    BookGenre z = new BookGenre()
                    {
                        IdBook = book.Id,
                        IdGenre = el
                    };
                    db.BookGenres.Create(z);
                    book.idGenres = book.idGenres.Where(val => val != el).ToArray();
                }

                db.Books.Update(item);
                db.Save();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        public void UpdateBook(BookModel item)
        {
            Book b = db.Books.GetItem(item.Id);
            db.Books.Update(toBook(item, b));
            db.Save();
        }

        private Book toBook(BookModel book, Book b)
        {
            b.Content = book.Content;
                 b.Cost = book.Cost;
                 b.IdPublisher = book.IdPublisher;
                 b.isDeleted = book.isDeleted;
                 b.image = book.image;
                 b.Rated = book.Rated;
                 b.Score = book.Score;
                 b.Stored = book.Stored;
                 b.Title = book.Title;
                 b.Weight = book.Weight;
                 b.Year = book.Year;
            return b;
        }
        private BookModel toBookModel(Book book)
        {
            return new BookModel()
            {
                Id = book.Id,
                Content = book.Content,
                Cost = book.Cost,
                IdPublisher = book.IdPublisher,
                isDeleted = book.isDeleted,
                image = book.image,
                Rated = book.Rated,
                Score = book.Score,
                Stored = book.Stored,
                Title = book.Title,
                Weight = book.Weight,
                Year = book.Year
            };
        }
        #endregion

        #region BookOrders
        public IEnumerable<BookOrderModel> GetAllBookOrders()
        {
            IEnumerable<BookOrder> bookOrderModels = db.BookOrders.GetList();
            List<BookOrderModel> bm = new List<BookOrderModel>();
            foreach (BookOrder i in bookOrderModels)
            {
                bm.Add(toBookOrderModel(i));
            }
            return bm;
        }

        public BookOrderModel GetBookOrder(int id)
        {
            return toBookOrderModel(db.BookOrders.GetItem(id));
        }

        public void CreateBookOrder(BookOrderModel item)
        {
            db.BookOrders.Create(toBookOrder(item, new BookOrder()));
            db.Save();
            db.BookOrders.GetList();
        }

        public void UpdateBookOrder(BookOrderModel item)
        {
            BookOrder n = db.BookOrders.GetItem(item.Id);
            db.BookOrders.Update(toBookOrder(item,n));
            db.Save();
        }

        public void DeleteBookOrder(int id)
        {
            db.BookOrders.Delete(id);
            db.Save();
        }

        private BookOrder toBookOrder(BookOrderModel bm, BookOrder a)
        {
            a.IdBook = bm.IdBook;
            a.Amount = bm.Amount;
            a.IdOrder = bm.IdOrder;
            return a;
        }
        private BookOrderModel toBookOrderModel(BookOrder bm)
        {
            return new BookOrderModel()
            {
                Id = bm.Id,
                IdBook = bm.IdBook,
                Amount = bm.Amount,
                IdOrder = bm.IdOrder
            };
        }
        #endregion

        #region Orders
        public IEnumerable<OrderModel> GetAllOrders()
        {
            IEnumerable<Order> or = db.Orders.GetList();
            return or.Select(b => toOrderModel(b));
        }
        public OrderModel GetOrder(int id)
        {
            return toOrderModel(db.Orders.GetItem(id));
        }
        public void CreateOrder(OrderModel item)
        {
            db.Orders.Create(toOrder(item, new Order()));
            db.Save();
            db.Orders.GetList();
        }
        public void UpdateOrder(OrderModel item)
        {
            Order or = db.Orders.GetItem(item.Id);
            db.Orders.Update(toOrder(item,or));
            db.Save();
        }
        public void DeleteOrder(int id)
        {
            db.Orders.Delete(id);
            db.Save();
        }
        private Order toOrder(OrderModel o,Order or)
        {
            or.Weight = o.Weight;
            or.UserId = o.UserId;
            or.SumOrder = o.SumOrder;
            or.SumDelivery = o.SumDelivery;
            or.DateSubmit = o.DateSubmit;
            or.DateSent = o.DateSent;
            or.DateOrder = o.DateOrder;
            or.DateDelivery = o.DateDelivery;
            or.DateCancel = o.DateCancel;
            or.Amount = o.Amount;
            or.Active = o.Active;
            return or;
        }
        private  OrderModel toOrderModel(Order o)
        {
            return new OrderModel()
            {
                Id = o.Id,
                Active = o.Active,
                DateCancel = o.DateCancel,
                DateDelivery = o.DateDelivery,
                DateOrder = o.DateOrder,
                DateSent = o.DateSent,
                Amount = o.Amount,
                DateSubmit = o.DateSubmit,
                SumDelivery = o.SumDelivery,
                SumOrder = o.SumOrder,
                UserId = o.UserId,
                Weight = o.Weight
                //BookOrders = o.BookOrders.Select(i=>toBookOrderModel(i)).ToList(),
                //User = toUserModel(o.User)
            };
        }
        #endregion

        #region BookAuthors
        public IEnumerable<BookAuthorModel> GetAllBookAuthors()
        {
            List<BookAuthorModel> bam = new List<BookAuthorModel>();
            IEnumerable<BookAuthor> ba = db.BookAuthors.GetList();
            foreach (BookAuthor b in ba)
            {
                bam.Add(toBookAuthorModel(b));
            }
            return bam;
        }

        public BookAuthorModel GetBookAuthor(int id)
        {
            return toBookAuthorModel(db.BookAuthors.GetItem(id));
        }

        public void CreateBookAuthor(BookAuthorModel item)
        {
            db.BookAuthors.Create(toBookAuthor(item, new BookAuthor()));
            db.Save();
            db.BookAuthors.GetList();
        }

        public void UpdateBookAuthor(BookAuthorModel item)
        {
            BookAuthor b = db.BookAuthors.GetItem(item.Id);
            db.BookAuthors.Update(toBookAuthor(item,b));
            db.Save();
        }

        public void DeleteBookAuthor(int id)
        {
            db.BookAuthors.Delete(id);
            db.Save();
        }

      private  BookAuthor toBookAuthor(BookAuthorModel bam, BookAuthor b)
        {
                b.IdAuthor = bam.IdAuthor;
                b.IdBook = bam.IdBook;
                 return b;
        }
      private  BookAuthorModel toBookAuthorModel(BookAuthor ba)
        {
            return new BookAuthorModel()
            {
                Id = ba.Id,
                IdAuthor = ba.IdAuthor,
                IdBook = ba.IdBook
            };
        }
        #endregion

        #region Authors
        public IEnumerable<AuthorModel> GetAllAuthors()
        {
            List<AuthorModel> am = new List<AuthorModel>();
            IEnumerable<Author> authors = db.Authors.GetList();
            foreach (Author a in authors)
            {
                am.Add(toAuthorModel(a));
            }
            return am;
        }

        public AuthorModel GetAuthor(int id)
        {
            return toAuthorModel(db.Authors.GetItem(id));
        }

        public void CreateAuthor(AuthorModel item)
        {
            db.Authors.Create(toAuthor(item, new Author()));
            db.Save();
            db.Authors.GetList();
        }

        public void UpdateAuthor(AuthorModel item)
        {
            Author n = db.Authors.GetItem(item.Id);
            db.Authors.Update(toAuthor(item,n));
            db.Save();
        }

        public void DeleteAuthor(int id)
        {
            db.Authors.Delete(id);
            db.Save();
        }

       private Author toAuthor(AuthorModel am, Author b)
        {
            b.Name = am.Name;
           return b;
        }
       private AuthorModel toAuthorModel(Author am)
        {
            return new AuthorModel()
            {
                Id = am.Id,
                Name = am.Name
            };
        }
        #endregion

        #region BookGenres -- look up as a model 
        public IEnumerable<BookGenreModel> GetAllBookGenres()
        {
            IEnumerable<BookGenre> bookGenres = db.BookGenres.GetList();
            //List<BookGenreModel> bm = new List<BookGenreModel>();
            //foreach (BookGenre bg in bookGenres)
            //{
            //    bm.Add(toBookGenreModel(bg));
            //}
            return bookGenres.Select(b => toBookGenreModel(b));
            ////return bm;
        }

        public BookGenreModel GetBookGenre(int id)
        {
            return toBookGenreModel(db.BookGenres.GetItem(id));
        }

        public void CreateBookGenre(BookGenreModel item)
        {
            db.BookGenres.Create(toBookGenre(item, new BookGenre()));
            db.Save();
            db.BookGenres.GetList();
        }

        public void UpdateBookGenre(BookGenreModel item)
        {
            db.BookGenres.Update(toBookGenre(item,db.BookGenres.GetItem(item.Id)));
            db.Save();
        }

        public void DeleteBookGenre(int id)
        {
            BookGenre o = db.BookGenres.GetItem(id);
            if (o != null)
                db.BookGenres.Delete(id);
            db.Save();
        }
        private BookGenre toBookGenre(BookGenreModel bg, BookGenre b)
        {
           b.IdBook = bg.IdBook;
           b.IdGenre = bg.IdGenre;
           return b;
        }
        private BookGenreModel toBookGenreModel(BookGenre bg)
        {
            return new BookGenreModel()
            {
                Id = bg.Id,
                IdBook = bg.IdBook,
                IdGenre = bg.IdGenre
            };
        }
        #endregion

        #region Genres
        public IEnumerable<GenreModel> GetAllGenres()
        {
            IEnumerable<Genre> g = db.Genres.GetList();
            return g.Select(h=>toGenreModel(h));
        }

        public GenreModel GetGenre(int id)
        {
            return toGenreModel(db.Genres.GetItem(id));
        }

        public void CreateGenre(GenreModel item)
        {
            db.Genres.Create(toGenre(item, new Genre()));
            db.Save();
        }

        public void UpdateGenre(GenreModel item)
        {
            db.Genres.Update(toGenre(item, db.Genres.GetItem(item.Id)));
            db.Save();
        }

        public void DeleteGenre(int id)
        {
            Genre o = db.Genres.GetItem(id);
            if (o != null)
                db.Genres.Delete(id);
            db.Save();
        }

        private Genre toGenre(GenreModel g, Genre b)
        {
            b.Name = g.Name;
            return b;
        }
      private  GenreModel toGenreModel(Genre g)
        {
            return new GenreModel()
            {
                Id = g.Id,
                Name = g.Name
            };
        }
        #endregion

        #region Cities
        public IEnumerable<CityModel> GetAllCities()
        {
            IEnumerable<City> cities = db.Cities.GetList();
            return cities.Select(h => toCityModel(h));
        }

        public CityModel GetCity(int id)
        {
            return toCityModel(db.Cities.GetItem(id));
        }

        public void CreateCity(CityModel item)
        {
            db.Cities.Create(toCity(item, new City()));
            db.Save();
        }

        public void UpdateCity(CityModel item)
        {
            db.Cities.Update(toCity(item,db.Cities.GetItem(item.Id)));
            db.Save();
        }

        public void DeleteCity(int id)
        {
            City o = db.Cities.GetItem(id);
            if (o != null)
                db.Cities.Delete(id);
            db.Save();
        }

       private City toCity(CityModel c, City b)
        {
            b.DeliverySum = c.DeliverySum;
            b.DeliveryTime = c.DeliveryTime;
            b.Name = c.Name;
           return b;
        }
       private CityModel toCityModel(City c)
        {
            return new CityModel()
            {
                Id = c.Id,
                DeliverySum = c.DeliverySum,
                DeliveryTime = c.DeliveryTime,
                Name = c.Name
            };
        }
        #endregion

        #region Comments
        public IEnumerable<CommentModel> GetAllComments()
        {
            IEnumerable<Comment> comments = db.Comments.GetList();
            return comments.Select(c=>toCommentModel(c));
        }

        public CommentModel GetComment(int id)
        {
            return toCommentModel(db.Comments.GetItem(id));
        }

        public void CreateComment(CommentModel item)
        {
            db.Comments.Create(toComment(item, new Comment()));
            db.Save();
        }

        public void UpdateComment(CommentModel item)
        {
            db.Comments.Update(toComment(item, db.Comments.GetItem(item.Id)));
            db.Save();
        }

        public void DeleteComment(int id)
        {
            Comment o = db.Comments.GetItem(id);
            if (o != null)
                db.Comments.Delete(id);
            db.Save();
        }

      private  Comment toComment(CommentModel c, Comment f)
        {

            f.Content = c.Content;
            f.DateComment = c.DateComment;

            f.IdBook = c.IdBook;
            f.UserName = c.UserName;
            return f;
   
        }
      private  CommentModel toCommentModel(Comment c)
        {
            return new CommentModel()
            {
                Content = c.Content,
                DateComment = c.DateComment,
                Id = c.Id,
                IdBook = c.IdBook,
                UserName = c.UserName
            };
        }
        #endregion

        #region Publishers
        public IEnumerable<PublisherModel> GetAllPublishers()
        {
            IEnumerable<Publisher> publishers = db.Publishers.GetList();
            return publishers.Select(f=>toPublisherModel(f));
        }
        public PublisherModel GetPublisher(int id)
        {
            return toPublisherModel(db.Publishers.GetItem(id));
        }
        public void CreatePublisher(PublisherModel item)
        {
            db.Publishers.Create(toPublisher(item, new Publisher()));
            db.Save();
        }
        public void UpdatePublisher(PublisherModel item)
        {
            db.Publishers.Update(toPublisher(item, db.Publishers.GetItem(item.Id)));
            db.Save();
        }
        public void DeletePublisher(int id)
        {
            Publisher o = db.Publishers.GetItem(id);
            if (o != null)
                db.Publishers.Delete(id);
            db.Save();
        }
        private Publisher toPublisher(PublisherModel o, Publisher b)
        {
            b.Name = o.Name;
            return b;
        }
        private PublisherModel toPublisherModel(Publisher o)
        {
            return new PublisherModel()
            {
                Id = o.Id,
                Name = o.Name
            };
        }
        #endregion

        #region Users
        public IEnumerable<UserModel> GetAllUsers()
        {
            IEnumerable<User> users = db.Users.GetList();
            return users.Select(f => toUserModel(f));
        }
        public UserModel GetUser(string id)
        {
            return toUserModel(db.Users.GetItem(id));
        }
        public void CreateUser(UserModel item)
        {
            db.Users.Create(toUser(item, new User()));
            db.Save();
        }
        public void UpdateUser(UserModel item)
        {
            db.Users.Update(toUser(item, db.Users.GetItem(item.Id)));
            db.Save();
        }
        public void DeleteUser(string id)
        {
            User o = db.Users.GetItem(id);
            if (o != null)
                db.Users.Delete(id);
            db.Save();
        }
        private User toUser(UserModel u, User d)
        {
            d.Address = u.Address;
                d.Fio = u.Fio;
                d.Email = u.Email;
                d.IdCity = u.IdCity;
                d.PhoneNumber=u.PhoneNumber;
            d.UserName = u.UserName;
            return d;
        }
        private UserModel toUserModel(User u)
        {
            return new UserModel()
            {
                Id = u.Id,
                Address = u.Address,
                Fio = u.Fio,
                Email = u.Email,
                IdCity = u.IdCity,
                PhoneNumber = u.PhoneNumber,
                UserName = u.UserName
            };
        }
        #endregion

    }
}
