using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MeetingRoomBooking.Data;
using MeetingRoomBooking.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingRoomBooking.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            return View(await _context.Bookings
                .Include(b => b.Room)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "Name");
            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,RoomId,BookedBy,Email,ContactNumber,Title,Description,StartTime,EndTime,NumberOfAttendees")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                // Check if the room is available during the requested time
                bool isRoomAvailable = await IsRoomAvailable(booking.RoomId, booking.StartTime, booking.EndTime);
                if (!isRoomAvailable)
                {
                    ModelState.AddModelError("", "The room is not available during the selected time.");
                    ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "Name", booking.RoomId);
                    return View(booking);
                }

                // Validate that EndTime is after StartTime
                if (booking.EndTime <= booking.StartTime)
                {
                    ModelState.AddModelError("EndTime", "End time must be after start time.");
                    ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "Name", booking.RoomId);
                    return View(booking);
                }

                // Check if number of attendees exceeds room capacity
                var room = await _context.Rooms.FindAsync(booking.RoomId);
                if (room != null && booking.NumberOfAttendees > room.Capacity)
                {
                    ModelState.AddModelError("NumberOfAttendees", $"The number of attendees exceeds the room capacity ({room.Capacity}).");
                    ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "Name", booking.RoomId);
                    return View(booking);
                }

                booking.CreatedDate = DateTime.Now;
                booking.Status = BookingStatus.Pending;

                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "Name", booking.RoomId);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "Name", booking.RoomId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,RoomId,BookedBy,Email,ContactNumber,Title,Description,StartTime,EndTime,NumberOfAttendees,Status,CreatedDate")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Check if the room is available during the requested time
                bool isRoomAvailable = await IsRoomAvailable(booking.RoomId, booking.StartTime, booking.EndTime, booking.BookingId);
                if (!isRoomAvailable)
                {
                    ModelState.AddModelError("", "The room is not available during the selected time.");
                    ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "Name", booking.RoomId);
                    return View(booking);
                }

                // Validate that EndTime is after StartTime
                if (booking.EndTime <= booking.StartTime)
                {
                    ModelState.AddModelError("EndTime", "End time must be after start time.");
                    ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "Name", booking.RoomId);
                    return View(booking);
                }

                // Check if number of attendees exceeds room capacity
                var room = await _context.Rooms.FindAsync(booking.RoomId);
                if (room != null && booking.NumberOfAttendees > room.Capacity)
                {
                    ModelState.AddModelError("NumberOfAttendees", $"The number of attendees exceeds the room capacity ({room.Capacity}).");
                    ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "Name", booking.RoomId);
                    return View(booking);
                }

                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "Name", booking.RoomId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Bookings/Calendar
        public IActionResult Calendar()
        {
            ViewData["Rooms"] = new SelectList(_context.Rooms, "RoomId", "Name");
            return View();
        }

        // POST: Bookings/GetBookings
        [HttpPost]
        public async Task<IActionResult> GetBookings(DateTime date, int? roomId)
        {
            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .Where(b => b.StartTime.Date == date.Date &&
                           (roomId == null || b.RoomId == roomId))
                .Select(b => new
                {
                    id = b.BookingId,
                    title = b.Title,
                    roomName = b.Room.Name,
                    start = b.StartTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = b.EndTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    bookedBy = b.BookedBy,
                    status = b.Status.ToString(),
                    backgroundColor = b.Status == BookingStatus.Approved ? "#28a745" :
                                     b.Status == BookingStatus.Pending ? "#ffc107" :
                                     b.Status == BookingStatus.Rejected ? "#dc3545" : "#6c757d"
                })
                .ToListAsync();

            return Json(bookings);
        }

        // Method to check if the room is available during the requested time
        private async Task<bool> IsRoomAvailable(int roomId, DateTime startTime, DateTime endTime, int? excludeBookingId = null)
        {
            var conflictingBookings = await _context.Bookings
                .Where(b => b.RoomId == roomId &&
                           b.Status != BookingStatus.Rejected &&
                           b.Status != BookingStatus.Cancelled &&
                           (excludeBookingId == null || b.BookingId != excludeBookingId) &&
                           (
                               // Check if the new booking overlaps with existing bookings
                               (startTime >= b.StartTime && startTime < b.EndTime) ||
                               (endTime > b.StartTime && endTime <= b.EndTime) ||
                               (startTime <= b.StartTime && endTime >= b.EndTime)
                           ))
                .AnyAsync();

            return !conflictingBookings;
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}