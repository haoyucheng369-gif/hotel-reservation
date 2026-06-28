using HotelReservation.Core.Domain.Common;
using HotelReservation.Core.Domain.Guests;
using HotelReservation.Core.Domain.Hotels;
using HotelReservation.Core.Domain.Reservations;
using HotelReservation.Core.Domain.Rooms;
using HotelReservation.Core.Ports;
using HotelReservation.Core.UseCases.Reservations.CreateReservation;

namespace HotelReservation.UseCases.Tests.Reservations.CreateReservation;

public class CreateReservationCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldFail_WhenRoomDoesNotExist()
    {
        var roomRepository = new FakeRoomRepository();
        var reservationRepository = new FakeReservationRepository();
        var handler = CreateHandler(roomRepository, reservationRepository);
        var command = CreateCommand();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(CreateReservationErrors.RoomNotFound, result.Error);
        Assert.Empty(reservationRepository.AddedReservations);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenRoomAlreadyBooked()
    {
        var roomId = new RoomId(Guid.NewGuid());
        var roomRepository = new FakeRoomRepository();
        roomRepository.Add(CreateRoom(roomId));
        var reservationRepository = new FakeReservationRepository
        {
            HasOverlappingReservation = true
        };
        var handler = CreateHandler(roomRepository, reservationRepository);
        var command = CreateCommand(roomId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(CreateReservationErrors.RoomAlreadyBooked, result.Error);
        Assert.Empty(reservationRepository.AddedReservations);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenRoomIsAvailable()
    {
        var roomId = new RoomId(Guid.NewGuid());
        var now = new DateTimeOffset(2026, 7, 1, 10, 0, 0, TimeSpan.Zero);
        var roomRepository = new FakeRoomRepository();
        roomRepository.Add(CreateRoom(roomId));
        var reservationRepository = new FakeReservationRepository();
        var handler = CreateHandler(roomRepository, reservationRepository, now);
        var command = CreateCommand(roomId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(default, result.Value);
        var reservation = Assert.Single(reservationRepository.AddedReservations);
        Assert.Equal(result.Value, reservation.Id);
        Assert.Equal(roomId, reservation.RoomId);
        Assert.Equal(ReservationStatus.Pending, reservation.Status);
        Assert.Equal(now, reservation.CreatedAt);
        Assert.True(reservationRepository.SaveChangesWasCalled);
    }

    private static CreateReservationCommandHandler CreateHandler(
        IRoomRepository roomRepository,
        IReservationRepository reservationRepository,
        DateTimeOffset? now = null)
    {
        return new CreateReservationCommandHandler(
            roomRepository,
            reservationRepository,
            new FixedClock(now ?? new DateTimeOffset(2026, 7, 1, 10, 0, 0, TimeSpan.Zero)));
    }

    private static CreateReservationCommand CreateCommand(RoomId? roomId = null)
    {
        return new CreateReservationCommand(
            roomId ?? new RoomId(Guid.NewGuid()),
            "Haoyu",
            "Cheng",
            "guest@example.com",
            new DateOnly(2026, 7, 1),
            new DateOnly(2026, 7, 5));
    }

    private static Room CreateRoom(RoomId roomId)
    {
        return new Room(roomId, new HotelId(Guid.NewGuid()), "101", 2, 120m);
    }

    private sealed class FakeRoomRepository : IRoomRepository
    {
        private readonly Dictionary<RoomId, Room> _rooms = [];

        public Task<Room?> GetByIdAsync(RoomId id, CancellationToken cancellationToken)
        {
            _rooms.TryGetValue(id, out var room);
            return Task.FromResult(room);
        }

        public Task<IReadOnlyList<Room>> SearchAvailableRoomsAsync(
            DateRange period,
            CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<Room>>(_rooms.Values.ToList());
        }

        public void Add(Room room)
        {
            _rooms[room.Id] = room;
        }
    }

    private sealed class FakeReservationRepository : IReservationRepository
    {
        public List<Reservation> AddedReservations { get; } = [];

        public bool HasOverlappingReservation { get; init; }

        public bool SaveChangesWasCalled { get; private set; }

        public Task<Reservation?> GetByIdAsync(
            ReservationId id,
            CancellationToken cancellationToken)
        {
            return Task.FromResult<Reservation?>(null);
        }

        public Task<bool> HasOverlappingReservationAsync(
            RoomId roomId,
            DateRange period,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(HasOverlappingReservation);
        }

        public Task AddAsync(
            Reservation reservation,
            CancellationToken cancellationToken)
        {
            AddedReservations.Add(reservation);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveChangesWasCalled = true;
            return Task.CompletedTask;
        }
    }

    private sealed class FixedClock(DateTimeOffset utcNow) : IClock
    {
        public DateTimeOffset UtcNow { get; } = utcNow;
    }
}
