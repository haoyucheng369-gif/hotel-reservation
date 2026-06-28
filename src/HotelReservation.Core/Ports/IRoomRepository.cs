using HotelReservation.Core.Domain.Common;
using HotelReservation.Core.Domain.Rooms;

namespace HotelReservation.Core.Ports;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(
        RoomId id,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Room>> SearchAvailableRoomsAsync(
        DateRange period,
        CancellationToken cancellationToken);
}
