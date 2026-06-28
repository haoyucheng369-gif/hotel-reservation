using HotelReservation.Core.Domain.Common;
using HotelReservation.Core.Domain.Rooms;

namespace HotelReservation.Core.Domain.Reservations;

public sealed record ReservationCreatedEvent(
    ReservationId ReservationId,
    RoomId RoomId,
    DateRange Period,
    DateTimeOffset OccurredAt) : IDomainEvent;
