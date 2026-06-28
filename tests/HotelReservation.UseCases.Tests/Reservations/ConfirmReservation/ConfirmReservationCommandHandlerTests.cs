using HotelReservation.Core.Domain.Common;
using HotelReservation.Core.Domain.Guests;
using HotelReservation.Core.Domain.Reservations;
using HotelReservation.Core.Domain.Rooms;
using HotelReservation.Core.Ports;
using HotelReservation.Core.UseCases.Reservations.ConfirmReservation;

namespace HotelReservation.UseCases.Tests.Reservations.ConfirmReservation;

public class ConfirmReservationCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldFail_WhenReservationDoesNotExist()
    {
        var repository = new FakeReservationRepository();
        var handler = new ConfirmReservationCommandHandler(repository);
        var command = new ConfirmReservationCommand(new ReservationId(Guid.NewGuid()));

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ConfirmReservationErrors.ReservationNotFound, result.Error);
        Assert.False(repository.SaveChangesWasCalled);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenReservationIsCancelled()
    {
        var reservation = CreateReservation();
        reservation.Cancel();
        var repository = new FakeReservationRepository(reservation);
        var handler = new ConfirmReservationCommandHandler(repository);
        var command = new ConfirmReservationCommand(reservation.Id);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ConfirmReservationErrors.ReservationAlreadyCancelled, result.Error);
        Assert.False(repository.SaveChangesWasCalled);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenReservationIsPending()
    {
        var reservation = CreateReservation();
        var repository = new FakeReservationRepository(reservation);
        var handler = new ConfirmReservationCommandHandler(repository);
        var command = new ConfirmReservationCommand(reservation.Id);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(ReservationStatus.Confirmed, reservation.Status);
        Assert.True(repository.SaveChangesWasCalled);
    }

    private static Reservation CreateReservation()
    {
        return Reservation.Create(
            new ReservationId(Guid.NewGuid()),
            new RoomId(Guid.NewGuid()),
            new GuestInfo("Haoyu", "Cheng", "guest@example.com"),
            new DateRange(new DateOnly(2026, 7, 1), new DateOnly(2026, 7, 5)),
            new DateTimeOffset(2026, 7, 1, 10, 0, 0, TimeSpan.Zero));
    }

    private sealed class FakeReservationRepository : IReservationRepository
    {
        private readonly Reservation? _reservation;

        public FakeReservationRepository(Reservation? reservation = null)
        {
            _reservation = reservation;
        }

        public bool SaveChangesWasCalled { get; private set; }

        public Task<Reservation?> GetByIdAsync(
            ReservationId id,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(_reservation?.Id == id ? _reservation : null);
        }

        public Task<bool> HasOverlappingReservationAsync(
            RoomId roomId,
            DateRange period,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }

        public Task AddAsync(
            Reservation reservation,
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveChangesWasCalled = true;
            return Task.CompletedTask;
        }
    }
}
