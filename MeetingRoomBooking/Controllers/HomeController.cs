using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MeetingRoomBooking.Data;
using MeetingRoomBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MeetingRoomBooking.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            
            // Get today's bookings
            var todayBookings = await _context.Bookings
                .Include(b => b.Room)
                .Where(b => b.StartTime.Date == today)
                .OrderBy(b => b.StartTime)
                .ToListAsync();
            
            // Get booking statistics
            var stats = new 
            {
                TotalRooms = await _context.Rooms.CountAsync(),
                TotalBookings = await _context.Bookings.CountAsync(),
                TodayBookings = todayBookings.Count,
                UpcomingBookings = await _context.Bookings
                    .Where(b => b.StartTime > DateTime.Now)
                    .CountAsync()
            };
            
            ViewData["Stats"] = stats;
            ViewData["TodayBookings"] = todayBookings;
            
            return View();
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
