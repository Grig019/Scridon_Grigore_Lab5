using Microsoft.AspNetCore.Mvc;
using Scridon_Grigore_Lab2.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Scridon_Grigore_Lab2.Data;
using Scridon_Grigore_Lab2.Models.LibraryViewModels;

namespace Scridon_Grigore_Lab2.Controllers
{
    public class HomeController : Controller
    {
        private readonly LibraryContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(LibraryContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }
      
   

        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Statistics()
        {
            IQueryable<OrderGroup> data =
                from order in _context.Orders
                group order by order.OrderDate into dateGroup
                select new OrderGroup()
                {
                    OrderDate = dateGroup.Key,
                    BookCount = dateGroup.Count()
                };

            return View(await data.AsNoTracking().ToListAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}