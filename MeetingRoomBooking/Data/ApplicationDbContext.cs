using Microsoft.EntityFrameworkCore;
using MeetingRoomBooking.Models;

namespace MeetingRoomBooking.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data for rooms
            modelBuilder.Entity<Room>().HasData(
                new Room
                {
                    RoomId = 1,
                    Name = "Conference Room A",
                    Description = "Large conference room with projector and video conferencing equipment.",
                    Capacity = 20,
                    HasProjector = true,
                    HasVideoConference = true,
                    FloorNumber = 1,
                    RoomNumber = "101"
                },
                new Room
                {
                    RoomId = 2,
                    Name = "Meeting Room B",
                    Description = "Medium-sized meeting room for team discussions.",
                    Capacity = 10,
                    HasProjector = true,
                    HasVideoConference = false,
                    FloorNumber = 1,
                    RoomNumber = "102"
                },
                new Room
                {
                    RoomId = 3,
                    Name = "Board Room",
                    Description = "Executive board room with full A/V equipment.",
                    Capacity = 15,
                    HasProjector = true,
                    HasVideoConference = true,
                    FloorNumber = 2,
                    RoomNumber = "201"
                },
                new Room
                {
                    RoomId = 4,
                    Name = "Small Meeting Room",
                    Description = "Small room for quick meetings and interviews.",
                    Capacity = 6,
                    HasProjector = false,
                    HasVideoConference = false,
                    FloorNumber = 2,
                    RoomNumber = "202"
                }
            );
        }
    }
}