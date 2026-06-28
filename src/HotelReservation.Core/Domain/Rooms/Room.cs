using HotelReservation.Core.Domain.Hotels;

namespace HotelReservation.Core.Domain.Rooms;

public sealed class Room
{
    public Room(RoomId id, HotelId hotelId, string roomNumber, int capacity, decimal basePrice)
    {
        if (string.IsNullOrWhiteSpace(roomNumber))
        {
            throw new ArgumentException("Room number is required.", nameof(roomNumber));
        }

        if (capacity <= 0)
        {
            throw new ArgumentException("Capacity must be positive.", nameof(capacity));
        }

        if (basePrice <= 0)
        {
            throw new ArgumentException("Base price must be positive.", nameof(basePrice));
        }

        Id = id;
        HotelId = hotelId;
        RoomNumber = roomNumber;
        Capacity = capacity;
        BasePrice = basePrice;
        IsActive = true;
    }

    public RoomId Id { get; }

    public HotelId HotelId { get; }

    public string RoomNumber { get; }

    public int Capacity { get; }

    public decimal BasePrice { get; }

    public bool IsActive { get; private set; }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
