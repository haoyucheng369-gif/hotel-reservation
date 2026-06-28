using HotelReservation.Core.Domain.Common;
using HotelReservation.Core.Domain.Guests;
using HotelReservation.Core.Domain.Rooms;

namespace HotelReservation.Core.Domain.Reservations;

public sealed class Reservation
{
    private readonly List<IDomainEvent> _domainEvents = [];

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

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public static Reservation Create(
        ReservationId id,
        RoomId roomId,
        GuestInfo guestInfo,
        DateRange period,
        DateTimeOffset createdAt)
    {
        var reservation = new Reservation(id, roomId, guestInfo, period, createdAt);
        reservation.AddDomainEvent(new ReservationCreatedEvent(id, roomId, period, createdAt));
        return reservation;
    }

    public void Confirm()
    {
        if (Status == ReservationStatus.Cancelled)
        {
            throw new InvalidOperationException("Cancelled reservation cannot be confirmed.");
        }

        Status = ReservationStatus.Confirmed;
        AddDomainEvent(new ReservationConfirmedEvent(Id, DateTimeOffset.UtcNow));
    }

    public void Cancel()
    {
        if (Status == ReservationStatus.Cancelled)
        {
            throw new InvalidOperationException("Cancelled reservation cannot be cancelled again.");
        }

        Status = ReservationStatus.Cancelled;
        AddDomainEvent(new ReservationCancelledEvent(Id, DateTimeOffset.UtcNow));
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    private void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
