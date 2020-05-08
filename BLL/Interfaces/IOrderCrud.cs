using BLL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Interfaces
{
    interface IOrderCrud
    {
        IEnumerable<OrderView> GetAllOrderViews();
        OrderView GetOrder(int id);
        void CreateOrder(OrderModel item);
        void UpdateOrder(OrderModel item);
        void DeleteOrder(int id);
    }
}
