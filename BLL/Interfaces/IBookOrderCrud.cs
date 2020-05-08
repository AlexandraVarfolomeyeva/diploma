using BLL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Interfaces
{
    interface IBookOrderCrud
    {
        IEnumerable<BookOrderModel> GetAllBookOrders();
        BookOrderModel GetBookOrder(int id);
        void CreateBookOrder(BookOrderModel item);
        void UpdateBookOrder(BookOrderModel item);
        void DeleteBookOrder(int id);
    }
}
