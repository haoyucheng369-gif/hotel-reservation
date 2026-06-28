using HotelReservation.Core.Domain.Common;
using HotelReservation.Core.Domain.Guests;
using HotelReservation.Core.Domain.Rooms;

namespace HotelReservation.Core.Domain.Reservations;

public sealed class Reservation
{
    private Reservation(
        ReservationId id,
        RoomId roomId,
        GuestInfo guestInfo,
        DateRange period,
        DateTimeOffset createdAt)
    {
        Id = id;
        RoomId = roomId;
        GuestInfo = guestInfo;
        Period = period;
        CreatedAt = createdAt;
        Status = ReservationStatus.Pending;
    }

    public ReservationId Id { get; }

    public RoomId RoomId { get; }

    public GuestInfo GuestInfo { get; }

    public DateRange Period { get; }

    public ReservationStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; }

    public static Reservation Create(
        ReservationId id,
        RoomId roomId,
        GuestInfo guestInfo,
        DateRange period,
        DateTimeOffset createdAt)
    {
        return new Reservation(id, roomId, guestInfo, period, createdAt);
    }

    public void Confirm()
    {
        if (Status == ReservationStatus.Cancelled)
        {
            throw new InvalidOperationException("Cancelled reservation cannot be confirmed.");
        }

        Status = ReservationStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status == ReservationStatus.Cancelled)
        {
            throw new InvalidOperationException("Cancelled reservation cannot be cancelled again.");
        }

        Status = ReservationStatus.Cancelled;
    }
}
