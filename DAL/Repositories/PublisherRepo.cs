using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories
{
    public class PublisherRepo : IRepository<Publisher>
    {

        private BookingContext _context;

        public PublisherRepo(BookingContext context)
        {
            _context = context;
        }


        public void Create(Publisher item)
        {
            try
            {
                _context.Publisher.Add(item);
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
                Publisher item = _context.Publisher.Find(id);
                if (item != null)
                {
                    _context.Publisher.Remove(item);
                }
                else
                {
                    Log.WriteSuccess("Publisher Repo delete()", "not found");
                }

            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        public Publisher GetItem(int id)
        {
            try
            {
                return _context.Publisher.Find(id);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
        }

        public IEnumerable<Publisher> GetList()
        {
            try
            {
                return _context.Publisher.Include(v => v.Books);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
        }

        public void Update(Publisher item)
        {
            try
            {
                _context.Publisher.Update(item);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
    }
}
