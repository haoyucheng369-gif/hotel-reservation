using HotelReservation.Core.Domain.Common;

namespace HotelReservation.Core.Domain.Reservations;

public sealed record ReservationConfirmedEvent(
    ReservationId ReservationId,
    DateTimeOffset OccurredAt) : IDomainEvent;
