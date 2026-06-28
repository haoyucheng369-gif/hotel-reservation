using HotelReservation.Core.Domain.Common;
using HotelReservation.Core.Domain.Hotels;
using HotelReservation.Core.Domain.Rooms;
using HotelReservation.Core.Ports;
using HotelReservation.Core.UseCases.Rooms.SearchAvailableRooms;

namespace HotelReservation.UseCases.Tests.Rooms.SearchAvailableRooms;

public class SearchAvailableRoomsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldFail_WhenDateRangeIsInvalid()
    {
        var repository = new FakeRoomRepository();
        var handler = new SearchAvailableRoomsQueryHandler(repository);
        var query = new SearchAvailableRoomsQuery(
            new DateOnly(2026, 7, 5),
            new DateOnly(2026, 7, 1));

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(SearchAvailableRoomsErrors.InvalidDateRange, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldReturnAvailableRooms()
    {
        var firstRoom = CreateRoom("101", 2, 120m);
        var secondRoom = CreateRoom("102", 4, 180m);
        var repository = new FakeRoomRepository([firstRoom, secondRoom]);
        var handler = new SearchAvailableRoomsQueryHandler(repository);
        var query = new SearchAvailableRoomsQuery(
            new DateOnly(2026, 7, 1),
            new DateOnly(2026, 7, 5));

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Collection(
            result.Value!,
            room =>
            {
                Assert.Equal(firstRoom.Id, room.Id);
                Assert.Equal(firstRoom.RoomNumber, room.RoomNumber);
                Assert.Equal(firstRoom.Capacity, room.Capacity);
                Assert.Equal(firstRoom.BasePrice, room.BasePrice);
            },
            room =>
            {
                Assert.Equal(secondRoom.Id, room.Id);
                Assert.Equal(secondRoom.RoomNumber, room.RoomNumber);
                Assert.Equal(secondRoom.Capacity, room.Capacity);
                Assert.Equal(secondRoom.BasePrice, room.BasePrice);
            });
    }

    private static Room CreateRoom(string roomNumber, int capacity, decimal basePrice)
    {
        return new Room(
            new RoomId(Guid.NewGuid()),
            new HotelId(Guid.NewGuid()),
            roomNumber,
            capacity,
            basePrice);
    }

    private sealed class FakeRoomRepository(IReadOnlyList<Room>? rooms = null) : IRoomRepository
    {
        private readonly IReadOnlyList<Room> _rooms = rooms ?? [];

        public Task<Room?> GetByIdAsync(RoomId id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_rooms.SingleOrDefault(room => room.Id == id));
        }

        public Task<IReadOnlyList<Room>> SearchAvailableRoomsAsync(
            DateRange period,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(_rooms);
        }
    }
}
