using HotelReservation.Core.Domain.Common;

namespace HotelReservation.Core.Domain.Reservations;

public sealed record ReservationCancelledEvent(
    ReservationId ReservationId,
    DateTimeOffset OccurredAt) : IDomainEvent;
