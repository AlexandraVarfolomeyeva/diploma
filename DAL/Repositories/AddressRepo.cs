using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories
{
    public class AddressRepo : IRepository<Address>
    {
        private BookingContext _context;

        public AddressRepo(BookingContext context)
        {
            _context = context;
        }

        public void Create(Address item)
        {
            try
            {
                _context.Address.Add(item);
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
                Address item = _context.Address.Find(id);
                if (item != null)
                {
                    _context.Address.Remove(item);
                }
                else
                {
                    Log.WriteSuccess("Address Repo delete()", "not found");
                }

            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        public Address GetItem(int id)
        {
            try
            {
                return _context.Address.Find(id);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
        }

        public IEnumerable<Address> GetList()
        {
            try
            {
                return _context.Address.Include(v => v.City);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
        }

        public void Update(Address item)
        {
            try
            {
                _context.Address.Update(item);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
    }
}
