using HotelReservation.Core.Domain.Common;
using HotelReservation.Core.Domain.Reservations;
using HotelReservation.Core.Domain.Rooms;

namespace HotelReservation.Core.Ports;

public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(
        ReservationId id,
        CancellationToken cancellationToken);

    Task<bool> HasOverlappingReservationAsync(
        RoomId roomId,
        DateRange period,
        CancellationToken cancellationToken);

    Task AddAsync(
        Reservation reservation,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
