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
   public class BookOrderRepo : IRepository<BookOrder>
    {
        private BookingContext _context;

        public BookOrderRepo(BookingContext context)
        {
            _context = context;
        }

        public void Create(BookOrder item)
        {
            try
            {
                _context.BookOrder.Add(item);
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
                BookOrder item = _context.BookOrder.Find(id);
                if (item != null)
                {
                    _context.BookOrder.Remove(item);
                }
                else
                {
                    Log.WriteSuccess("BookOrder Repo delete()", "not found");
                }

            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        public BookOrder GetItem(int id)
        {
            try
            {
                return _context.BookOrder.Where(m => m.Id == id).Include(b => b.Book).Include(b => b.Order).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
        }

        public IEnumerable<BookOrder> GetList()
        {
            try
            {
                return _context.BookOrder.Include(v => v.Order).Include(b => b.Book);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
        }

        public void Update(BookOrder item)
        {
            try
            {
                _context.BookOrder.Update(item);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
    }
}
