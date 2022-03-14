using Microsoft.AspNetCore.Mvc.Rendering;
using Zadanie.Models;

namespace Zadanie.ViewModel
{
    public class ViewModelIndex
    {
        public IEnumerable<Order> Orders { get; set; }
        public IEnumerable<Order> OrdersNumberList { get; set; }
        public IEnumerable<OrderItem> OrderItems { get; set; }
        public IEnumerable<OrderItem> OrderItemsUnit { get; set; }
        public IEnumerable<Provider> Providers { get; set; }
        public TimeInterval TimeIntervals { get; set; }
        public List<int> ProviderId { get; set; }
        public List<string> OrderNumber { get; set; }
        public List<string> OrderItemName { get; set; }

        public List<string> OrderItemUnit { get; set; }



    }
}
