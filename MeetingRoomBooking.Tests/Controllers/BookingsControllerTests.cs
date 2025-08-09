using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeetingRoomBooking.Controllers;
using MeetingRoomBooking.Data;
using MeetingRoomBooking.Models;
using MeetingRoomBooking.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MeetingRoomBooking.Tests.Controllers
{
    public class BookingsControllerTests_Index
    {
        [Fact]
        public async Task Index_ReturnsAViewResult_WithListOfBookings()
        {
            // Arrange
            using var dbContext = DbContextHelper.CreateDbContextWithData();
            var controller = new BookingsController(dbContext);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Booking>>(viewResult.Model);
            Assert.Equal(3, model.Count()); // Based on seeded data in DbContextHelper
            // Check that all bookings are present by their titles
            Assert.Contains(model, b => b.Title == "Test Meeting 1");
            Assert.Contains(model, b => b.Title == "Test Meeting 2");
            Assert.Contains(model, b => b.Title == "Test Meeting 3");
        }
    }

    public class BookingsControllerTests_Details
    {
        [Fact]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            using var dbContext = DbContextHelper.CreateDbContextWithData();
            var controller = new BookingsController(dbContext);

            // Act
            var result = await controller.Details(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenBookingDoesNotExist()
        {
            // Arrange
            using var dbContext = DbContextHelper.CreateDbContextWithData();
            var controller = new BookingsController(dbContext);
            int nonExistentId = 999;

            // Act
            var result = await controller.Details(nonExistentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WithBookingModel()
        {
            // Arrange
            using var dbContext = DbContextHelper.CreateDbContextWithData();
            var controller = new BookingsController(dbContext);
            int existingId = 1; // From seeded data

            // Act
            var result = await controller.Details(existingId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Booking>(viewResult.Model);
            Assert.Equal(existingId, model.BookingId);
            Assert.Equal("Test Meeting 1", model.Title);
        }
    }

    public class BookingsControllerTests_Create_GET
    {
        [Fact]
        public void Create_ReturnsViewResult_WithDefaultValues()
        {
            // Arrange
            using var dbContext = DbContextHelper.CreateDbContextWithData();
            var controller = new BookingsController(dbContext);

            // Act
            var result = controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Booking>(viewResult.Model);
            
            // Default values should be set
            Assert.NotEqual(default, model.StartTime);
            Assert.NotEqual(default, model.EndTime);
            Assert.True(model.EndTime > model.StartTime);
            
            // ViewData should contain SelectList for Rooms
            Assert.NotNull(viewResult.ViewData["RoomId"]);
            var roomSelectList = Assert.IsType<SelectList>(viewResult.ViewData["RoomId"]);
            Assert.Equal(2, roomSelectList.Count()); // Based on seeded data
        }

        [Fact]
        public void Create_WithParameters_ReturnsViewResult_WithSpecifiedValues()
        {
            // Arrange
            using var dbContext = DbContextHelper.CreateDbContextWithData();
            var controller = new BookingsController(dbContext);
            string startDate = "2025-01-01";
            string startTime = "10:00";
            string endDate = "2025-01-01";
            string endTime = "11:30";
            int roomId = 2;

            // Act
            var result = controller.Create(startDate, startTime, endDate, endTime, roomId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Booking>(viewResult.Model);
            
            Assert.Equal(new DateTime(2025, 1, 1, 10, 0, 0), model.StartTime);
            Assert.Equal(new DateTime(2025, 1, 1, 11, 30, 0), model.EndTime);
            Assert.Equal(roomId, model.RoomId);
        }
    }

    public class BookingsControllerTests_Create_POST
    {
        [Fact]
        public async Task Create_Post_WithInvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            using var dbContext = DbContextHelper.CreateDbContextWithData();
            var controller = new BookingsController(dbContext);
            var booking = new Booking(); // Invalid model without required fields
            controller.ModelState.AddModelError("Title", "Required");

            // Act
            var result = await controller.Create(booking);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(booking, viewResult.Model);
            Assert.NotNull(viewResult.ViewData["RoomId"]);
        }

        [Fact]
        public async Task Create_Post_WithEndTimeBeforeStartTime_ReturnsViewWithError()
        {
            // Arrange
            using var dbContext = DbContextHelper.CreateDbContextWithData();
            var controller = new BookingsController(dbContext);
            var booking = new Booking
            {
                RoomId = 1,
                Title = "Test Meeting",
                Description = "Description is required",
                BookedBy = "Test User",
                Email = "test@example.com",
                ContactNumber = "1234567890",
                StartTime = DateTime.Now.AddHours(2),
                EndTime = DateTime.Now.AddHours(1), // End time before start time
                NumberOfAttendees = 5
            };

            // Act
            var result = await controller.Create(booking);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(booking, viewResult.Model);
            
            // The controller adds the error with an empty key or with "EndTime"
            Assert.True(controller.ModelState.Count > 0);
            Assert.Contains(controller.ModelState.Keys, key => 
                key == "EndTime" || key == "");
        }

        [Fact]
        public async Task Create_Post_WithTooManyAttendees_ReturnsViewWithError()
        {
            // Arrange
            using var dbContext = DbContextHelper.CreateDbContextWithData();
            var controller = new BookingsController(dbContext);
            var booking = new Booking
            {
                RoomId = 2, // Small room with capacity 6
                Title = "Test Meeting",
                Description = "Description is required",
                BookedBy = "Test User",
                Email = "test@example.com",
                ContactNumber = "1234567890",
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2),
                NumberOfAttendees = 10 // Too many for room capacity of 6
            };

            // Act
            var result = await controller.Create(booking);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(booking, viewResult.Model);
            Assert.True(controller.ModelState.ContainsKey("NumberOfAttendees"));
        }

        [Fact]
        public async Task Create_Post_WithOverlappingBooking_ReturnsViewWithError()
        {
            // Arrange
            using var dbContext = DbContextHelper.CreateDbContextWithData();
            var controller = new BookingsController(dbContext);
            
            // Get an existing booking from the test data
            var existingBooking = await dbContext.Bookings.FirstAsync();
            
            // Create a new booking that overlaps with the existing one
            var booking = new Booking
            {
                RoomId = existingBooking.RoomId,
                Title = "Overlapping Meeting",
                Description = "Description is required",
                BookedBy = "Test User",
                Email = "test@example.com",
                ContactNumber = "1234567890",
                StartTime = existingBooking.StartTime.AddMinutes(30), // Starts during existing booking
                EndTime = existingBooking.EndTime.AddHours(1),
                NumberOfAttendees = 5
            };

            // Act
            var result = await controller.Create(booking);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(booking, viewResult.Model);
            Assert.True(controller.ModelState.Count > 0);
        }

        [Fact]
        public async Task Create_Post_WithValidBooking_RedirectsToIndex()
        {
            // Arrange
            using var dbContext = DbContextHelper.CreateDbContextWithData();
            var controller = new BookingsController(dbContext);
            var booking = new Booking
            {
                RoomId = 1,
                Title = "Valid New Meeting",
                Description = "Description is required",
                BookedBy = "Test User",
                Email = "test@example.com",
                ContactNumber = "1234567890",
                StartTime = DateTime.Now.AddDays(10).AddHours(10), // Far in future to avoid conflicts
                EndTime = DateTime.Now.AddDays(10).AddHours(11),
                NumberOfAttendees = 5
            };

            // Act
            var result = await controller.Create(booking);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            
            // Verify the booking was added to the database
            var addedBooking = await dbContext.Bookings
                .OrderByDescending(b => b.BookingId)
                .FirstOrDefaultAsync();
                
            Assert.NotNull(addedBooking);
            Assert.Equal("Valid New Meeting", addedBooking.Title);
            Assert.Equal(BookingStatus.Pending, addedBooking.Status); // Should default to pending
        }
    }

    public class BookingsControllerTests_GetRoomCapacity
    {
        [Fact]
        public async Task GetRoomCapacity_WithValidRoomId_ReturnsRoomCapacity()
        {
            // Arrange
            using var dbContext = DbContextHelper.CreateDbContextWithData();
            var controller = new BookingsController(dbContext);
            int roomId = 1; // From seeded data

            // Act
            var result = await controller.GetRoomCapacity(roomId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var capacity = Assert.IsType<int>(jsonResult.Value);
            Assert.Equal(20, capacity); // Room 1 has capacity of 20
        }

        [Fact]
        public async Task GetRoomCapacity_WithInvalidRoomId_ReturnsNotFound()
        {
            // Arrange
            using var dbContext = DbContextHelper.CreateDbContextWithData();
            var controller = new BookingsController(dbContext);
            int nonExistentRoomId = 999;

            // Act
            var result = await controller.GetRoomCapacity(nonExistentRoomId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }

    public class BookingsControllerTests_GetBookings
    {
        [Fact]
        public async Task GetBookings_ReturnsJsonResult_WithBookingsForDate()
        {
            // Arrange
            using var dbContext = DbContextHelper.CreateDbContextWithData();
            var controller = new BookingsController(dbContext);
            
            // Get a date for which we have bookings in the test data
            var booking = await dbContext.Bookings.FirstAsync();
            var date = booking.StartTime.Date;

            // Act
            var result = await controller.GetBookings(date, null);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var bookings = Assert.IsAssignableFrom<IEnumerable<object>>(jsonResult.Value);
            Assert.NotEmpty(bookings);
            
            // Convert anonymous type to dictionary for testing
            var firstBooking = bookings.First();
            var props = firstBooking.GetType().GetProperties();
            var dict = props.ToDictionary(p => p.Name, p => p.GetValue(firstBooking));
            
            // Verify properties from the first result
            Assert.True(dict.ContainsKey("id"));
            Assert.True(dict.ContainsKey("title"));
            Assert.True(dict.ContainsKey("start"));
            Assert.True(dict.ContainsKey("end"));
            Assert.True(dict.ContainsKey("roomName"));
            Assert.True(dict.ContainsKey("backgroundColor"));
        }

        [Fact]
        public async Task GetBookings_WithRoomFilter_ReturnsFilteredBookings()
        {
            // Arrange
            using var dbContext = DbContextHelper.CreateDbContextWithData();
            var controller = new BookingsController(dbContext);
            
            // Get a date and room for which we have bookings
            var booking = await dbContext.Bookings.FirstAsync(b => b.RoomId == 1);
            var date = booking.StartTime.Date;
            int roomId = 1;

            // Act
            var result = await controller.GetBookings(date, roomId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var bookings = Assert.IsAssignableFrom<IEnumerable<object>>(jsonResult.Value);
            
            // Check all returned bookings are for the specified room
            foreach (var item in bookings)
            {
                var props = item.GetType().GetProperties();
                var dict = props.ToDictionary(p => p.Name, p => p.GetValue(item));
                
                // Get roomName and verify it's for room 1
                var roomName = dict["roomName"].ToString();
                Assert.Equal("Test Conference Room", roomName); // Room 1's name
            }
        }
    }
}