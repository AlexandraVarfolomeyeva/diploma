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
    public class UserRepo : IRepositoryUser
    {
        private BookingContext _context;

        public UserRepo(BookingContext context)
        {
            _context = context;
        }

        public void Create(User item)
        {
            try
            {
                _context.User.Add(item);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        public void Delete(string id)
        {
            try
            {
                User usr = _context.User.Where(k=>k.Id == id).FirstOrDefault();
                if (usr != null) {
                    _context.User.Remove(usr);
                } else
                {
                    Log.WriteSuccess("User Repo delete()","not found");
                }
                
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        public User GetItem(string id)
        {
            try
            {
               return _context.User.Where(m => m.Id == id).Include(c=>c.City).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
        }

        public IEnumerable<User> GetList()
        {
            try
            {
                return _context.User.Include(v=>v.City);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
        }

        public void Update(User item)
        {
            try
            {
                _context.User.Update(item);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
    }
}
