using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Zadanie.Models;
using Zadanie.ViewModel;

namespace Zadanie.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationContext dbContext;
        ViewModelIndex viewModel = new ViewModelIndex();
        public HomeController(ApplicationContext _dbContext)
        {
            dbContext = _dbContext;

            viewModel.Providers = dbContext.Providers.Distinct().ToList();
            viewModel.OrdersNumberList = dbContext.Orders.Distinct().ToList();


            List<OrderItem> orderNames = new List<OrderItem>();
            foreach (var item in dbContext.OrderItem)
            {
                if (!orderNames.Any(c => c.Name == item.Name))
                {
                    orderNames.Add(item);
                }
            }
            viewModel.OrderItems = orderNames;

            List<OrderItem> orderUnit = new List<OrderItem>();
            foreach (var item in dbContext.OrderItem)
            {
                if (!orderUnit.Any(c => c.Unit.ToLower() == item.Unit.ToLower()))
                {
                    orderUnit.Add(item);
                }
            }
            viewModel.OrderItemsUnit = orderUnit;

        }


        public IActionResult Index()
        {
            viewModel.Orders = dbContext.Orders.ToList();

            #region Настройка дат
            viewModel.TimeIntervals = new TimeInterval();
            viewModel.TimeIntervals.StartDate = DateTime.Now - TimeSpan.FromDays(30);
            viewModel.TimeIntervals.StartDate -= TimeSpan.FromMilliseconds(viewModel.TimeIntervals.StartDate.Millisecond);
            viewModel.TimeIntervals.EndDate = DateTime.Now;
            viewModel.TimeIntervals.EndDate -= TimeSpan.FromMilliseconds(viewModel.TimeIntervals.EndDate.Millisecond);
            viewModel.TimeIntervals.EndDate += TimeSpan.FromSeconds(5);
            #endregion

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Index(ViewModelIndex vm)
        {

            var orders = dbContext.Orders.Where(order => order.Date >= vm.TimeIntervals.StartDate
            && order.Date <= vm.TimeIntervals.EndDate).ToList();

            if (vm.ProviderId != null)
            {
                orders = orders.Where(order => vm.ProviderId.Contains(order.ProviderId)).ToList();
            }

            if (vm.OrderNumber != null)
            {
                orders = orders.Where(order => vm.OrderNumber.Contains(order.Number)).ToList();
            }

            if (vm.OrderItemName != null)
            {
                var orderItems = dbContext.OrderItem.Where(orderItem => vm.OrderItemName.Contains(orderItem.Name));
                var list = new List<int>();
                foreach (var item in orderItems)
                {
                    list.Add(item.OrderId);
                }

                orders = orders.Where(order => list.Contains(order.Id)).ToList();
            }
            if (vm.OrderItemUnit != null)
            {
                var orderItems = dbContext.OrderItem.Where(orderItem => vm.OrderItemUnit.Contains(orderItem.Unit));
                var list = new List<int>();
                foreach (var item in orderItems)
                {
                    list.Add(item.OrderId);
                }

                orders = orders.Where(order => list.Contains(order.Id)).ToList();
            }

            viewModel.Orders = orders.Distinct();


            return View(viewModel);

            viewModel.OrderNumber.Clear();

            viewModel.ProviderId.Clear();
        }



        #region Работа с Order
        public ActionResult Details(int? id)
        {

            Order order = dbContext.Orders.Find(id);
            ViewData["textprovider"] = dbContext.Providers.Find(order.ProviderId).Name;
            ViewBag.OrderItem = dbContext.OrderItem.Where(i => i.OrderId == id);
            return View(order);
        }

        public IActionResult CreateOrder()
        {
            ViewBag.ProvList = dbContext.Providers.ToList();
            return View(new Order()
            {
                Date = DateTime.Now,
            });

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateOrder(Order order)
        {
            if (ModelState.IsValid)
            {
                order.Date = DateTime.Now;
                dbContext.Orders.Add(order);
                dbContext.SaveChanges();
                var id = dbContext.Orders.Where(ord => ord.ProviderId == order.ProviderId && ord.Number == order.Number).FirstOrDefault().Id;
                return RedirectToAction("Details", new { id = id });
            }
            return View(order);
        }
        public IActionResult EditOrder(int id)
        {
            ViewBag.ProvList = dbContext.Providers.ToList();
            var order = dbContext.Orders.Find(id);
            order.Date -= TimeSpan.FromMilliseconds(order.Date.Millisecond);
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditOrder(Order order)
        {
            if (ModelState.IsValid)
            {
                dbContext.ChangeTracker.Clear();
                dbContext.Orders.Update(order);
                dbContext.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }
        public IActionResult Delete(int id)
        {
            var order = dbContext.Orders.Find(id);
            var orderItems = dbContext.OrderItem.Where(item => item.OrderId == id);
            dbContext.OrderItem.RemoveRange(orderItems);
            dbContext.Orders.Remove(order);

            dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        #endregion


        #region Работа с OrderItem
        public IActionResult DeleteOrderItem(int id)
        {
            var orderItem = dbContext.OrderItem.Find(id);
            dbContext.OrderItem.Remove(orderItem);

            dbContext.SaveChanges();
            return RedirectToAction("Details", new { id = orderItem.OrderId });
        }

        [HttpGet]
        public IActionResult CreateOrderItem(int orderid)
        {
            return View(new OrderItem()
            {
                OrderId = orderid
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateOrderItem(OrderItem orderItem)
        {
            if (ModelState.IsValid)
            {
                dbContext.OrderItem.Add(orderItem);
                dbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(orderItem);
        }


        public IActionResult EditOrderItem(int id)
        {

            return View(dbContext.OrderItem.Find(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditOrderItem(OrderItem orderItem)
        {
            if (ModelState.IsValid)
            {
                dbContext.ChangeTracker.Clear();
                dbContext.OrderItem.Update(orderItem);
                dbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(orderItem);
        }

        #endregion

        #region Работа с Provider
        public IActionResult CreateProvider()
        {

            return View(new Provider());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateProvider(Provider provider)
        {
            if (ModelState.IsValid)
            {
                dbContext.Providers.Add(provider);
                dbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(provider);
        }

        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}