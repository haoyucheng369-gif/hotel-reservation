using HotelReservation.Core.Domain.Common;

namespace HotelReservation.Core.Tests.Domain.Common;

public class DateRangeTests
{
    [Fact]
    public void Constructor_ShouldThrow_WhenCheckOutIsBeforeCheckIn()
    {
        var checkIn = new DateOnly(2026, 7, 5);
        var checkOut = new DateOnly(2026, 7, 1);

        Assert.Throws<ArgumentException>(() => new DateRange(checkIn, checkOut));
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenCheckOutEqualsCheckIn()
    {
        var date = new DateOnly(2026, 7, 1);

        Assert.Throws<ArgumentException>(() => new DateRange(date, date));
    }

    [Fact]
    public void Overlaps_ShouldReturnTrue_WhenRangesOverlap()
    {
        var first = new DateRange(
            new DateOnly(2026, 7, 1),
            new DateOnly(2026, 7, 5));
        var second = new DateRange(
            new DateOnly(2026, 7, 4),
            new DateOnly(2026, 7, 8));

        Assert.True(first.Overlaps(second));
        Assert.True(second.Overlaps(first));
    }

    [Fact]
    public void Overlaps_ShouldReturnFalse_WhenRangesAreAdjacent()
    {
        var first = new DateRange(
            new DateOnly(2026, 7, 1),
            new DateOnly(2026, 7, 5));
        var second = new DateRange(
            new DateOnly(2026, 7, 5),
            new DateOnly(2026, 7, 8));

        Assert.False(first.Overlaps(second));
        Assert.False(second.Overlaps(first));
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenDateRangesHaveSameValues()
    {
        var first = new DateRange(
            new DateOnly(2026, 7, 1),
            new DateOnly(2026, 7, 5));
        var second = new DateRange(
            new DateOnly(2026, 7, 1),
            new DateOnly(2026, 7, 5));

        Assert.Equal(first, second);
    }
}
