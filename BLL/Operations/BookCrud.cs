using BLL.Interfaces;
using BLL.Models;
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
    public class BookCrud : IBookCrud
    {

        IdbRepos db;
        public BookCrud(IdbRepos repos)
        {
            db = repos;
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
            IEnumerable<Book> books= db.Books.GetList();
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
            throw new NotImplementedException();
        }

        public void UpdateBook(BookAdd item)
        {
            throw new NotImplementedException();
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
    }
}
