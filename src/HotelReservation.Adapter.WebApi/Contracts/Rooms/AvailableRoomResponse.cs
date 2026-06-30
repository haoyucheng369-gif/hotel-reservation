namespace HotelReservation.Adapter.WebApi.Contracts.Rooms;

public sealed record AvailableRoomResponse(
    Guid Id,
    string RoomNumber,
    int Capacity,
    decimal BasePrice);
