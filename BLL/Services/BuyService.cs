using BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace diploma.BLL
{
    public class BuyService
    {
        int discount;
        public BuyService(int disc)
        {
            this.discount = disc;
        }
        public IEnumerable<OrderModel> GetDiscount (IEnumerable<OrderModel> orders, int i)
        {
            orders.ToList()[i].SumOrder = orders.ToList()[i].SumOrder * (100 - discount) / 100;
            return orders;
        }
    }
}
