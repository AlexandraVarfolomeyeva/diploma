using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories
{
    public class CityRepo : IRepository<City>
    {
        private BookingContext _context;

        public CityRepo(BookingContext context)
        {
            _context = context;
        }

        public void Create(City item)
        {
            try
            {
                _context.City.Add(item);
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
                City item = _context.City.Find(id);
                if (item != null)
                {
                    _context.City.Remove(item);
                }
                else
                {
                    Log.WriteSuccess("City Repo delete()", "not found");
                }

            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        public City GetItem(int id)
        {
            try
            {
                return _context.City.Find(id);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
        }

        public IEnumerable<City> GetList()
        {
            try
            {
                return _context.City.Include(v => v.Addresses);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
        }

        public void Update(City item)
        {
            try
            {
                _context.City.Update(item);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
    }
}
