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
   public class BookGenreRepo : IRepository<BookGenre>
    {
        private BookingContext _context;

        public BookGenreRepo(BookingContext context)
        {
            _context = context;
        }

        public void Create(BookGenre item)
        {
            try
            {
                _context.BookGenre.Add(item);
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
                BookGenre item = _context.BookGenre.Find(id);
                if (item != null)
                {
                    _context.BookGenre.Remove(item);
                }
                else
                {
                    Log.WriteSuccess("BookGenre Repo delete()", "not found");
                }

            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        public BookGenre GetItem(int id)
        {
            try
            {
                return _context.BookGenre.Where(m=>m.Id==id).Include(m=>m.Genre).Include(m=>m.Book).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
        }

        public IEnumerable<BookGenre> GetList()
        {
            try
            {
                return _context.BookGenre.Include(v => v.Book).Include(b => b.Genre);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
        }

        public void Update(BookGenre item)
        {
            try
            {
                _context.BookGenre.Update(item);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
    }
}
