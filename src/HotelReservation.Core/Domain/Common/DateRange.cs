namespace HotelReservation.Core.Domain.Common;

public sealed record DateRange
{
    public DateRange(DateOnly checkIn, DateOnly checkOut)
    {
        if (checkOut <= checkIn)
        {
            throw new ArgumentException("Check-out date must be after check-in date.", nameof(checkOut));
        }

        CheckIn = checkIn;
        CheckOut = checkOut;
    }

    public DateOnly CheckIn { get; }

    public DateOnly CheckOut { get; }

    public bool Overlaps(DateRange other)
    {
        return CheckIn < other.CheckOut && other.CheckIn < CheckOut;
    }
}
