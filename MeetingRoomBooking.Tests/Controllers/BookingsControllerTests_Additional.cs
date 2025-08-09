using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeetingRoomBooking.Controllers;
using MeetingRoomBooking.Data;
using MeetingRoomBooking.Models;
using MeetingRoomBooking.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MeetingRoomBooking.Tests.Controllers
{
    public class BookingsControllerTests_Edit
    {
        [Fact]
        public async Task Edit_Get_WithNullId_ReturnsNotFound()
        {
            // Arrange
            using var dbContext = DbContextHelper.CreateDbContextWithData();
            var controller = new BookingsController(dbContext);

            // Act
            var result = await controller.Edit(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Get_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var dbContext = DbContextHelper.CreateDbContextWithData();
            var controller = new BookingsController(dbContext);
            int nonExistentId = 999;

            // Act
            var result = await controller.Edit(nonExistentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Get_WithValidId_ReturnsViewWithBooking()
        {
            // Arrange
            using var dbContext = DbContextHelper.CreateDbContextWithData();
            var controller = new BookingsController(dbContext);
            int existingId = 1; // From seeded data

            // Act
            var result = await controller.Edit(existingId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Booking>(viewResult.Model);
            Assert.Equal(existingId, model.BookingId);
            Assert.Equal("Test Meeting 1", model.Title);
            Assert.NotNull(viewResult.ViewData["RoomId"]);
        }

        [Fact]
        public async Task Edit_Post_WithInvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            using var dbContext = DbContextHelper.CreateDbContextWithData();
            var controller = new BookingsController(dbContext);
            var booking = new Booking { BookingId = 1, Description = "Required" }; // Invalid model
            controller.ModelState.AddModelError("Title", "Required");

            // Act
            var result = await controller.Edit(1, booking);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(booking, viewResult.Model);
            Assert.NotNull(viewResult.ViewData["RoomId"]);
        }

        [Fact]
        public async Task Edit_Post_WithMismatchedId_ReturnsNotFound()
        {
            // Arrange
            using var dbContext = DbContextHelper.CreateDbContextWithData();
            var controller = new BookingsController(dbContext);
            int bookingId = 1;
            int differentId = 2;
            var booking = new Booking { BookingId = differentId, Description = "Required" };

            // Act
            var result = await controller.Edit(bookingId, booking);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_WithValidBooking_UpdatesBookingAndRedirectsToIndex()
        {
            // For the Edit test, we need to create a separate database context since we're testing Entity Framework's update behavior
            var dbContextForSetup = DbContextHelper.CreateDbContextWithData();
            var bookingId = 1;
            var existingBooking = await dbContextForSetup.Bookings.FindAsync(bookingId);
            
            // Create a new context for the controller
            using var dbContext = DbContextHelper.CreateDbContextWithData();
            var controller = new BookingsController(dbContext);
            
            // Create a detached booking object to update
            var updatedBooking = new Booking
            {
                BookingId = bookingId,
                RoomId = existingBooking.RoomId,
                Title = "Updated Meeting Title",
                BookedBy = "Updated User",
                Email = existingBooking.Email,
                ContactNumber = existingBooking.ContactNumber,
                Description = "Updated description",
                StartTime = existingBooking.StartTime.AddHours(1),
                EndTime = existingBooking.EndTime.AddHours(1),
                NumberOfAttendees = existingBooking.NumberOfAttendees,
                Status = BookingStatus.Approved,
                CreatedDate = existingBooking.CreatedDate
            };

            // Act
            var result = await controller.Edit(bookingId, updatedBooking);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            
            // Verify booking was updated in database
            var bookingFromDb = await dbContext.Bookings.FindAsync(bookingId);
            Assert.Equal("Updated Meeting Title", bookingFromDb.Title);
            Assert.Equal("Updated User", bookingFromDb.BookedBy);
            Assert.Equal("Updated description", bookingFromDb.Description);
            Assert.Equal(BookingStatus.Approved, bookingFromDb.Status);
        }
    }

    public class BookingsControllerTests_Delete
    {
        [Fact]
        public async Task Delete_Get_WithNullId_ReturnsNotFound()
        {
            // Arrange
            using var dbContext = DbContextHelper.CreateDbContextWithData();
            var controller = new BookingsController(dbContext);

            // Act
            var result = await controller.Delete(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Get_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var dbContext = DbContextHelper.CreateDbContextWithData();
            var controller = new BookingsController(dbContext);
            int nonExistentId = 999;

            // Act
            var result = await controller.Delete(nonExistentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Get_WithValidId_ReturnsViewWithBooking()
        {
            // Arrange
            using var dbContext = DbContextHelper.CreateDbContextWithData();
            var controller = new BookingsController(dbContext);
            int existingId = 1; // From seeded data

            // Act
            var result = await controller.Delete(existingId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Booking>(viewResult.Model);
            Assert.Equal(existingId, model.BookingId);
            Assert.Equal("Test Meeting 1", model.Title);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesBookingAndRedirectsToIndex()
        {
            // Arrange
            using var dbContext = DbContextHelper.CreateDbContextWithData();
            var controller = new BookingsController(dbContext);
            int existingId = 1; // From seeded data
            
            // Verify booking exists before delete
            var bookingBeforeDelete = await dbContext.Bookings.FindAsync(existingId);
            Assert.NotNull(bookingBeforeDelete);

            // Act
            var result = await controller.DeleteConfirmed(existingId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            
            // Verify booking was removed from database
            var bookingAfterDelete = await dbContext.Bookings.FindAsync(existingId);
            Assert.Null(bookingAfterDelete);
        }

        [Fact]
        public async Task DeleteConfirmed_WithInvalidId_StillRedirectsToIndex()
        {
            // Arrange
            using var dbContext = DbContextHelper.CreateDbContextWithData();
            var controller = new BookingsController(dbContext);
            int nonExistentId = 999;

            // Act
            var result = await controller.DeleteConfirmed(nonExistentId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }
    }

    public class BookingsControllerTests_Calendar
    {
        [Fact]
        public void Calendar_ReturnsViewResult_WithRoomsInViewData()
        {
            // Arrange
            using var dbContext = DbContextHelper.CreateDbContextWithData();
            var controller = new BookingsController(dbContext);

            // Act
            var result = controller.Calendar();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.ViewData["Rooms"]);
        }
    }
}