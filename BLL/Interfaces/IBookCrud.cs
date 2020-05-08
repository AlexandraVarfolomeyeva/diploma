using BLL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Interfaces
{
    public interface IBookCrud
    {
        IEnumerable<BookView> GetAllBookViews();
        BookAdd GetBook(int id);
        void CreateBook(BookAdd item);
        void UpdateBook(BookAdd item);
        void DeleteBook(int id);
    }
}
