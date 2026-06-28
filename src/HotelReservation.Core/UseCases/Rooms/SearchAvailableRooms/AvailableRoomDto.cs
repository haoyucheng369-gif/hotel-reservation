using HotelReservation.Core.Domain.Rooms;

namespace HotelReservation.Core.UseCases.Rooms.SearchAvailableRooms;

public sealed record AvailableRoomDto(
    RoomId Id,
    string RoomNumber,
    int Capacity,
    decimal BasePrice);
