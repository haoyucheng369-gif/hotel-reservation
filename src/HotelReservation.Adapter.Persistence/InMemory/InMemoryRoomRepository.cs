using HotelReservation.Core.Domain.Common;
using HotelReservation.Core.Domain.Hotels;
using HotelReservation.Core.Domain.Rooms;
using HotelReservation.Core.Ports;

namespace HotelReservation.Adapter.Persistence.InMemory;

public sealed class InMemoryRoomRepository : IRoomRepository
{
    private readonly List<Room> _rooms;
    private readonly IReservationRepository _reservationRepository;

    public InMemoryRoomRepository(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
        var hotelId = new HotelId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));
        _rooms =
        [
            new Room(new RoomId(Guid.Parse("11111111-1111-1111-1111-111111111111")), hotelId, "101", 2, 120m),
            new Room(new RoomId(Guid.Parse("22222222-2222-2222-2222-222222222222")), hotelId, "102", 2, 135m),
            new Room(new RoomId(Guid.Parse("33333333-3333-3333-3333-333333333333")), hotelId, "201", 4, 210m)
        ];
    }

    public Task<Room?> GetByIdAsync(
        RoomId id,
        CancellationToken cancellationToken)
    {
        var room = _rooms.SingleOrDefault(room => room.Id == id);
        return Task.FromResult(room);
    }

    public async Task<IReadOnlyList<Room>> SearchAvailableRoomsAsync(
        DateRange period,
        CancellationToken cancellationToken)
    {
        var availableRooms = new List<Room>();

        foreach (var room in _rooms.Where(room => room.IsActive))
        {
            var hasOverlap = await _reservationRepository.HasOverlappingReservationAsync(
                room.Id,
                period,
                cancellationToken);

            if (!hasOverlap)
            {
                availableRooms.Add(room);
            }
        }

        return availableRooms;
    }
}
