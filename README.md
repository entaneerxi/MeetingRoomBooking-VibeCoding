# Meeting Room Booking System

A comprehensive web application for managing meeting room bookings built with C# .NET 8 and ASP.NET Core MVC Razor Pages.

![Meeting Room Booking System]()

## Features

- **Room Management**
  - Create, edit, and delete meeting rooms
  - Track room details including capacity, floor number, room number, and available equipment
  - Categorize rooms by amenities (projector, video conferencing)

- **Booking System**
  - Create, view, and manage room bookings
  - Book rooms for specific date and time slots
  - Prevent double bookings through scheduling validation
  - Track meeting details including title, description, number of attendees

- **Dashboard**
  - View room availability at a glance
  - See today's bookings and upcoming reservations
  - Monitor booking statistics and room utilization

- **Calendar View**
  - Visual calendar interface for room availability
  - Filter bookings by room
  - Easily identify booking status through color coding

## Technology Stack

- **Framework**: ASP.NET Core 8.0
- **Language**: C# 12
- **Frontend**: Razor Pages, Bootstrap 5, JavaScript
- **Database**: Microsoft SQL Server
- **ORM**: Entity Framework Core 9
- **Development Environment**: Visual Studio 2022

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-express-downloads)
- Visual Studio 2022 or later (recommended) or any code editor
- Entity Framework Core tools (`dotnet-ef`)

## Installation

1. **Clone the repository**

   ```
   git clone https://github.com/yourusername/MeetingRoomBooking.git
   cd MeetingRoomBooking
   ```

2. **Update the connection string**

   Edit `appsettings.json` and update the connection string to match your SQL Server instance:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MeetingRoomBooking;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```

3. **Install Entity Framework Tools (if not installed)**

   ```
   dotnet tool install --global dotnet-ef
   ```

4. **Apply migrations to create the database**

   ```
   dotnet-ef database update
   ```

5. **Run the application**

   ```
   dotnet run
   ```

6. **Access the application**

   Open a web browser and navigate to `https://localhost:7000` or `http://localhost:5000` (the actual ports may vary)

## Project Structure

- **MeetingRoomBooking/**
  - **Controllers/**: Contains all MVC controllers
    - `HomeController.cs`: Main dashboard controller
    - `RoomsController.cs`: Room management controller
    - `BookingsController.cs`: Booking management controller
  - **Models/**: Data models
    - `Room.cs`: Room entity
    - `Booking.cs`: Booking entity
  - **Data/**: Database context and configurations
    - `ApplicationDbContext.cs`: EF Core DbContext
  - **Views/**: Razor views
    - **Home/**: Dashboard views
    - **Rooms/**: Room management views
    - **Bookings/**: Booking management views
    - **Shared/**: Layout and shared views
  - **Migrations/**: EF Core database migrations
  - **wwwroot/**: Static assets (CSS, JavaScript, images)

## Database Schema

The application uses two main entities:

1. **Room**
   - RoomId (PK)
   - Name
   - Description
   - Capacity
   - HasProjector (boolean)
   - HasVideoConference (boolean)
   - FloorNumber
   - RoomNumber

2. **Booking**
   - BookingId (PK)
   - RoomId (FK)
   - BookedBy
   - Email
   - ContactNumber
   - Title
   - Description
   - StartTime
   - EndTime
   - CreatedDate
   - NumberOfAttendees
   - Status (Enum: Pending, Approved, Rejected, Cancelled)

## Usage

### Managing Rooms

1. Navigate to the Rooms section
2. Add new rooms with details like capacity, floor number, and available equipment
3. Edit or delete existing rooms

### Creating a Booking

1. Navigate to "Book a Room" or select a specific room
2. Choose the date and time slot
3. Fill in meeting details (title, description, number of attendees)
4. Submit the booking

### Viewing Bookings

1. Use the Dashboard to see today's bookings
2. Navigate to the Bookings section for a list of all bookings
3. Use the Calendar view for a visual representation of room availability

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgements

- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Bootstrap](https://getbootstrap.com/)
- [jQuery](https://jquery.com/)