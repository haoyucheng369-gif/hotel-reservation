namespace HotelReservation.Adapter.WebApi.Contracts.Reservations;

public sealed record CreateReservationRequest(
    Guid RoomId,
    string GuestFirstName,
    string GuestLastName,
    string GuestEmail,
    DateOnly CheckIn,
    DateOnly CheckOut);
