using BookShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShop.BLL
{
    public class BuyService
    {
        int discount;
        public BuyService(int disc)
        {
            this.discount = disc;
        }
        public IEnumerable<Order> GetDiscount (IEnumerable<Order> orders, int i)
        {
            orders.ToList()[i].SumOrder = orders.ToList()[i].SumOrder * (100 - discount) / 100;
            return orders;
        }
    }
}
