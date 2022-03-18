using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Repositories
{
   public class BookAuthorRepo : IRepository<BookAuthor>
    {
        private BookingContext _context;

        public BookAuthorRepo(BookingContext context)
        {
            _context = context;
        }

        public void Create(BookAuthor item)
        {
            try
            {
                _context.BookAuthor.Add(item);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        public void Delete(int id)
        {
            try
            {
                Genre item = _context.Genre.Find(id);
                if (item != null)
                {
                    _context.Genre.Remove(item);
                }
                else
                {
                    Log.WriteSuccess("BookAuthor Repo delete()", "not found");
                }

            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        public BookAuthor GetItem(int id)
        {
            try
            {
                return _context.BookAuthor.Where(m => m.Id == id).Include(b => b.Author).Include(b => b.Book).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
        }

        public IEnumerable<BookAuthor> GetList()
        {
            try
            {
                return _context.BookAuthor.Include(v => v.Author).Include(b => b.Book);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
        }

        public void Update(BookAuthor item)
        {
            try
            {
                _context.BookAuthor.Update(item);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
    }
}
