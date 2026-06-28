namespace HotelReservation.Core.Ports;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
