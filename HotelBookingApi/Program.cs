using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

// Entities
public class Room
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Type { get; set; }
    public bool IsAvailable { get; set; } = true;
}

public class Booking
{
    public int Id { get; set; }
    [Required]
    public string GuestName { get; set; }
    public int RoomId { get; set; }
    public Room Room { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
}

// DbContext
public class HotelContext : DbContext
{
    public HotelContext(DbContextOptions<HotelContext> options) : base(options) { }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Booking> Bookings { get; set; }
}

// DTO for booking request
public record CreateBookingDto(string GuestName, int RoomId, DateTime CheckInDate, DateTime CheckOutDate);

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Register DbContext (InMemory)
        builder.Services.AddDbContext<HotelContext>(opt =>
            opt.UseInMemoryDatabase("HotelDb"));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Seed initial rooms
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<HotelContext>();
            if (!db.Rooms.Any())
            {
                db.Rooms.AddRange(
                    new Room { Id = 1, Name = "101", Type = "Single", IsAvailable = true },
                    new Room { Id = 2, Name = "102", Type = "Double", IsAvailable = true },
                    new Room { Id = 3, Name = "103", Type = "Suite", IsAvailable = true }
                );
                db.SaveChanges();
            }
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Endpoints
        app.MapGet("/api/rooms", async (HotelContext db) =>
            await db.Rooms.ToListAsync());

        app.MapGet("/api/bookings", async (HotelContext db) =>
            await db.Bookings.Include(b => b.Room).ToListAsync());

        app.MapPost("/api/bookings", async (HotelContext db, CreateBookingDto dto) =>
        {
            var room = await db.Rooms.FindAsync(dto.RoomId);
            if (room == null) return Results.NotFound("Room not found.");
            if (!room.IsAvailable) return Results.BadRequest("Room is already booked.");

            var booking = new Booking
            {
                GuestName = dto.GuestName,
                RoomId = dto.RoomId,
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate
            };

            room.IsAvailable = false; // mark room unavailable
            db.Bookings.Add(booking);
            await db.SaveChangesAsync();

            return Results.Created($"/api/bookings/{booking.Id}", booking);
        });

        app.Run();
    }
}
