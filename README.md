**Hotel Booking API**

A simple RESTful API built with ASP.NET Core (.NET 8), Entity Framework Core (InMemory provider), and Swagger.  
This project allows users to:
- List available rooms
- Book a room for a guest
- View all bookings

How to run:
git clone <your-repository-link>
cd HotelBookingApi
dotnet restore
dotnet run

The API will start and show something like this:
Now listening on: http://localhost:5298
Application started. Press Ctrl+C to shut down.

Type this on browser:
http://localhost:5298/swagger

In the Swagger UI, you can test endpoints:
GET /api/rooms - shows rooms (all start as available).
GET /api/bookings - shows bookings (empty at first).
POST /api/bookings - create a booking. Example JSON:
{
  "guestName": "John Doe",
  "roomId": 1,
  "checkInDate": "2025-05-01",
  "checkOutDate": "2025-05-03"
}

After this, if /api/rooms is called again, room 101 will be IsAvailable = false.


**Design Decisions**
I kept the design straightforward by using ASP.NET Core Minimal APIs instead of full controllers. This keeps the code short, easy to follow, and well-suited for a small project.

I also added basic validation so that users cannot book a non-existent room or one that is already unavailable. This ensures the booking process works correctly.

The booking request data is handled separately from the database model. This separation makes the code clearer, easier to maintain, and prevents unwanted changes to the database.

Swagger was enabled as well, making it easy to test the endpoints in the browser.
