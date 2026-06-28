using HotelReservation.Core.Domain.Hotels;
using HotelReservation.Core.Domain.Rooms;

namespace HotelReservation.Core.Tests.Domain.Rooms;

public class RoomTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_ShouldThrow_WhenRoomNumberIsEmpty(string roomNumber)
    {
        Assert.Throws<ArgumentException>(() => CreateRoom(roomNumber: roomNumber));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_ShouldThrow_WhenCapacityIsNotPositive(int capacity)
    {
        Assert.Throws<ArgumentException>(() => CreateRoom(capacity: capacity));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_ShouldThrow_WhenBasePriceIsNotPositive(decimal basePrice)
    {
        Assert.Throws<ArgumentException>(() => CreateRoom(basePrice: basePrice));
    }

    [Fact]
    public void Constructor_ShouldCreateActiveRoom_WhenValuesAreValid()
    {
        var roomId = new RoomId(Guid.NewGuid());
        var hotelId = new HotelId(Guid.NewGuid());

        var room = new Room(roomId, hotelId, "101", 2, 120m);

        Assert.Equal(roomId, room.Id);
        Assert.Equal(hotelId, room.HotelId);
        Assert.Equal("101", room.RoomNumber);
        Assert.Equal(2, room.Capacity);
        Assert.Equal(120m, room.BasePrice);
        Assert.True(room.IsActive);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        var room = CreateRoom();

        room.Deactivate();

        Assert.False(room.IsActive);
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue()
    {
        var room = CreateRoom();
        room.Deactivate();

        room.Activate();

        Assert.True(room.IsActive);
    }

    private static Room CreateRoom(
        string roomNumber = "101",
        int capacity = 2,
        decimal basePrice = 120m)
    {
        return new Room(
            new RoomId(Guid.NewGuid()),
            new HotelId(Guid.NewGuid()),
            roomNumber,
            capacity,
            basePrice);
    }
}
