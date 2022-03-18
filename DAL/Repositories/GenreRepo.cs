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
    public class GenreRepo : IRepository<Genre>
    {
        private BookingContext _context;

        public GenreRepo(BookingContext context)
        {
            _context = context;
        }

        public void Create(Genre item)
        {
            try
            {
                _context.Genre.Add(item);
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
                    Log.WriteSuccess("Genre Repo delete()", "not found");
                }

            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        public Genre GetItem(int id)
        {
            try
            {
                return _context.Genre.Where(m => m.Id == id).Include(b=>b.BookGenres).ThenInclude(b=>b.Book).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
        }

        public IEnumerable<Genre> GetList()
        {
            try
            {
                return _context.Genre.Include(v=>v.BookGenres).ThenInclude(b=>b.Book);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
        }

        public void Update(Genre item)
        {
            try
            {
                _context.Genre.Update(item);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
    }
}
