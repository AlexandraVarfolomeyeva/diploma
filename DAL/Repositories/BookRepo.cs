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
   public class BookRepo : IRepository<Book>
    {
        private BookingContext _context;

        public BookRepo(BookingContext context)
        {
            _context = context;
        }

        public void Create(Book item)
        {
            try { _context.Book.Add(item);
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
                Book item = _context.Book.Find(id);
                if (item == null)
                {
                    Log.WriteSuccess(" Books repository ", "Книга не найдена.");
                }
                IEnumerable<BookOrder> lines = _context.BookOrder.Where(l => l.IdBook == id);
                foreach (BookOrder i in lines)
                {
                    Order order = _context.Order.Find(i.IdOrder);
                    if (order.Active == 1)
                    {
                        order.Amount -= i.Amount;
                        order.SumOrder -= item.Cost * i.Amount;
                        _context.Order.Update(order);
                        _context.BookOrder.Remove(i);
                    }
                }
                item.isDeleted = true;
                _context.Book.Update(item);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        public Book GetItem(int id)
        {
            try
            {
              Book item =  _context.Book.Where(m => m.Id == id).Include(b=>b.BookAuthors).ThenInclude(v=>v.Author).Include(b=>b.BookGenres).ThenInclude(v=>v.Genre).Include(b=>b.BookOrders).ThenInclude(v=>v.Order).FirstOrDefault();
                if (item == null)
                {
                    Log.WriteSuccess("Book repository ", "Книга не найдена.");
                }
                return item;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
        }

        public IEnumerable<Book> GetList()
        {
            try
            {
                return _context.Book.Where(book => book.isDeleted == false).Include(b=>b.Comments).Include(b => b.BookAuthors).ThenInclude(v => v.Author).Include(b => b.BookGenres).ThenInclude(v => v.Genre).Include(b => b.BookOrders).ThenInclude(v => v.Order);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
        }

        public void Update(Book item)
        {
            try { _context.Book.Update(item);
            } catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
    }
}
