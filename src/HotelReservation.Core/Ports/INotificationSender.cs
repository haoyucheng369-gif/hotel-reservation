using HotelReservation.Core.Domain.Reservations;

namespace HotelReservation.Core.Ports;

public interface INotificationSender
{
    Task SendReservationConfirmedAsync(
        Reservation reservation,
        CancellationToken cancellationToken);
}
