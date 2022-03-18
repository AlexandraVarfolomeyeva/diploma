using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories
{
    public class CommentRepo : IRepository<Comment>
    {
        private BookingContext _context;

        public CommentRepo(BookingContext context)
        {
            _context = context;
        }

        public void Create(Comment item)
        {
            try
            {
                _context.Comment.Add(item);
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
                Comment item = _context.Comment.Find(id);
                if (item != null)
                {
                    _context.Comment.Remove(item);
                }
                else
                {
                    Log.WriteSuccess("Comment Repo delete()", "not found");
                }

            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        public Comment GetItem(int id)
        {
            try
            {
                return _context.Comment.Find(id);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
        }

        public IEnumerable<Comment> GetList()
        {
            try
            {
                return _context.Comment.Include(v => v.Book);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
        }

        public void Update(Comment item)
        {
            try
            {
                _context.Comment.Update(item);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
    }
}
