using HotelReservation.Core.Domain.Common;
using HotelReservation.Core.Domain.Reservations;
using HotelReservation.Core.Domain.Rooms;
using HotelReservation.Core.Ports;

namespace HotelReservation.Adapter.Persistence.InMemory;

public sealed class InMemoryReservationRepository : IReservationRepository
{
    private readonly List<Reservation> _reservations = [];

    public Task<Reservation?> GetByIdAsync(
        ReservationId id,
        CancellationToken cancellationToken)
    {
        var reservation = _reservations.SingleOrDefault(reservation => reservation.Id == id);
        return Task.FromResult(reservation);
    }

    public Task<bool> HasOverlappingReservationAsync(
        RoomId roomId,
        DateRange period,
        CancellationToken cancellationToken)
    {
        var hasOverlap = _reservations.Any(reservation =>
            reservation.RoomId == roomId &&
            reservation.Status != ReservationStatus.Cancelled &&
            reservation.Period.Overlaps(period));

        return Task.FromResult(hasOverlap);
    }

    public Task AddAsync(
        Reservation reservation,
        CancellationToken cancellationToken)
    {
        _reservations.Add(reservation);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
