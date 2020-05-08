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
            db.Books.Create(toBook(book));

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
        }

        public void DeleteBook(int id)
        {
            throw new NotImplementedException();
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
                        _context.BookAuthor.Remove(line);
                    }
                }
                while (book.idAuthors.Any())//found the new 
                {
                    int el = book.idAuthors[0];
                    BookAuthor z = new BookAuthor()
                    {
                        IdBook = id,
                        IdAuthor = el
                    };
                    _context.BookAuthor.Add(z);
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
                        _context.BookGenre.Remove(line);
                    }
                }

                while (book.idGenres.Any())//found the new 
                {
                    int el = book.idGenres[0];
                    BookGenre z = new BookGenre()
                    {
                        IdBook = id,
                        IdGenre = el
                    };
                    _context.BookGenre.Add(z);
                    book.idGenres = book.idGenres.Where(val => val != el).ToArray();
                }

                _context.Book.Update(item);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest(ModelState);
            }
        }

        private Book toBook(BookModel book)
        {
            return new Book()
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
        IEnumerable<BookOrderModel> GetAllBookOrders()
        {

        }
        BookOrderModel GetBookOrder(int id)
        {

        }
        void CreateBookOrder(BookOrderModel item)
        {

        }
        void UpdateBookOrder(BookOrderModel item)
        {

        }
        void DeleteBookOrder(int id)
        {

        }
        #endregion
    }
}
