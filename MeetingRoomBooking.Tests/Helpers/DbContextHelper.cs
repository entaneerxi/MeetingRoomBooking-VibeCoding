using Microsoft.EntityFrameworkCore;
using MeetingRoomBooking.Data;
using MeetingRoomBooking.Models;
using System;

namespace MeetingRoomBooking.Tests.Helpers
{
    public static class DbContextHelper
    {
        public static ApplicationDbContext CreateDbContext()
        {
            // Create a new unique in-memory database for each test
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
                
            return new ApplicationDbContext(options);
        }

        public static ApplicationDbContext CreateDbContextWithData()
        {
            var context = CreateDbContext();
            
            // Add test data
            
            // Add rooms
            var room1 = new Room
            {
                RoomId = 1,
                Name = "Test Conference Room",
                Description = "Test room for unit tests",
                Capacity = 20,
                HasProjector = true,
                HasVideoConference = true,
                FloorNumber = 1,
                RoomNumber = "101"
            };

            var room2 = new Room
            {
                RoomId = 2,
                Name = "Small Meeting Room",
                Description = "Small test room",
                Capacity = 6,
                HasProjector = false,
                HasVideoConference = false,
                FloorNumber = 1,
                RoomNumber = "102"
            };
            
            context.Rooms.Add(room1);
            context.Rooms.Add(room2);
            context.SaveChanges();

            // Add bookings
            var now = DateTime.Now;
            var today = now.Date;
            
            context.Bookings.Add(new Booking
            {
                BookingId = 1,
                RoomId = 1,
                Title = "Test Meeting 1",
                Description = "Test description",
                BookedBy = "Test User 1",
                Email = "test1@example.com",
                ContactNumber = "1234567890",
                StartTime = today.AddHours(10),
                EndTime = today.AddHours(11),
                CreatedDate = now.AddDays(-1),
                NumberOfAttendees = 5,
                Status = BookingStatus.Approved
            });
            
            context.Bookings.Add(new Booking
            {
                BookingId = 2,
                RoomId = 2,
                Title = "Test Meeting 2",
                Description = "Another test description",
                BookedBy = "Test User 2",
                Email = "test2@example.com",
                ContactNumber = "0987654321",
                StartTime = today.AddHours(14),
                EndTime = today.AddHours(15),
                CreatedDate = now.AddDays(-2),
                NumberOfAttendees = 4,
                Status = BookingStatus.Pending
            });
            
            context.Bookings.Add(new Booking
            {
                BookingId = 3,
                RoomId = 1,
                Title = "Test Meeting 3",
                Description = "Third test description",
                BookedBy = "Test User 3",
                Email = "test3@example.com",
                ContactNumber = "5555555555",
                StartTime = today.AddDays(1).AddHours(10),
                EndTime = today.AddDays(1).AddHours(12),
                CreatedDate = now,
                NumberOfAttendees = 10,
                Status = BookingStatus.Pending
            });
            
            context.SaveChanges();
            
            return context;
        }
    }
}