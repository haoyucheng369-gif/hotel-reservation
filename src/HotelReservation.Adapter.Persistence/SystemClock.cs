using HotelReservation.Core.Ports;

namespace HotelReservation.Adapter.Persistence;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
