using HotelReservation.Core.Domain.Common;
using HotelReservation.Core.Domain.Guests;
using HotelReservation.Core.Domain.Reservations;
using HotelReservation.Core.Domain.Rooms;

namespace HotelReservation.Core.Tests.Domain.Reservations;

public class ReservationTests
{
    [Fact]
    public void Create_ShouldCreatePendingReservation()
    {
        var reservationId = new ReservationId(Guid.NewGuid());
        var roomId = new RoomId(Guid.NewGuid());
        var guestInfo = CreateGuestInfo();
        var period = CreatePeriod();
        var createdAt = new DateTimeOffset(2026, 7, 1, 10, 0, 0, TimeSpan.Zero);

        var reservation = Reservation.Create(reservationId, roomId, guestInfo, period, createdAt);

        Assert.Equal(reservationId, reservation.Id);
        Assert.Equal(roomId, reservation.RoomId);
        Assert.Equal(guestInfo, reservation.GuestInfo);
        Assert.Equal(period, reservation.Period);
        Assert.Equal(createdAt, reservation.CreatedAt);
        Assert.Equal(ReservationStatus.Pending, reservation.Status);
    }

    [Fact]
    public void Confirm_ShouldSetStatusToConfirmed_WhenReservationIsPending()
    {
        var reservation = CreateReservation();

        reservation.Confirm();

        Assert.Equal(ReservationStatus.Confirmed, reservation.Status);
    }

    [Fact]
    public void Cancel_ShouldSetStatusToCancelled_WhenReservationIsPending()
    {
        var reservation = CreateReservation();

        reservation.Cancel();

        Assert.Equal(ReservationStatus.Cancelled, reservation.Status);
    }

    [Fact]
    public void Cancel_ShouldSetStatusToCancelled_WhenReservationIsConfirmed()
    {
        var reservation = CreateReservation();
        reservation.Confirm();

        reservation.Cancel();

        Assert.Equal(ReservationStatus.Cancelled, reservation.Status);
    }

    [Fact]
    public void Confirm_ShouldThrow_WhenReservationIsCancelled()
    {
        var reservation = CreateReservation();
        reservation.Cancel();

        Assert.Throws<InvalidOperationException>(reservation.Confirm);
    }

    [Fact]
    public void Cancel_ShouldThrow_WhenReservationIsAlreadyCancelled()
    {
        var reservation = CreateReservation();
        reservation.Cancel();

        Assert.Throws<InvalidOperationException>(reservation.Cancel);
    }

    private static Reservation CreateReservation()
    {
        return Reservation.Create(
            new ReservationId(Guid.NewGuid()),
            new RoomId(Guid.NewGuid()),
            CreateGuestInfo(),
            CreatePeriod(),
            new DateTimeOffset(2026, 7, 1, 10, 0, 0, TimeSpan.Zero));
    }

    private static GuestInfo CreateGuestInfo()
    {
        return new GuestInfo("Haoyu", "Cheng", "guest@example.com");
    }

    private static DateRange CreatePeriod()
    {
        return new DateRange(
            new DateOnly(2026, 7, 1),
            new DateOnly(2026, 7, 5));
    }
}
