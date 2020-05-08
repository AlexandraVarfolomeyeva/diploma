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
   public class AuthorRepo : IRepository<Author>
    {
        private BookingContext _context;

        public AuthorRepo(BookingContext context)
        {
            _context = context;
        }

        public void Create(Author item)
        {
            try
            {
                _context.Author.Add(item);
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
                Author item = _context.Author.Find(id);
                if (item != null)
                {
                    _context.Author.Remove(item);
                }
                else
                {
                    Log.WriteSuccess("Author Repo delete()", "not found");
                }

            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        public Author GetItem(int id)
        {
            try
            {
                return _context.Author.Where(m => m.Id == id).Include(b => b.BookAuthors).ThenInclude(b => b.Book).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
        }

        public IEnumerable<Author> GetList()
        {
            try
            {
                return _context.Author.Include(v => v.BookAuthors).ThenInclude(b => b.Book);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
        }

        public void Update(Author item)
        {
            try
            {
                _context.Author.Update(item);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
    }
}
