using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories
{
    public class OrderRepo : IRepository<Order>
    {
        private BookingContext _context;

        public OrderRepo(BookingContext context)
        {
            _context = context;
        }

        public void Create(Order item)
        {
            try
            {
                _context.Order.Add(item);
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
                Order o = _context.Order.Find(id);
                if (o != null)
                {
                    _context.Order.Remove(o);
                }
                else
                {
                    Log.WriteSuccess("Order Repo delete()", "not found");
                }

            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        public Order GetItem(int id)
        {
            try
            {
                return _context.Order.Find(id);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
        }

        public IEnumerable<Order> GetList()
        {
            try
            {
                return _context.Order.Include(o=>o.BookOrders).ThenInclude(b=>b.Book).Include(b=>b.User).ThenInclude(v=>v.City);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
        }

        public void Update(Order item)
        {
            try
            {
                _context.Order.Update(item);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
    }
}
